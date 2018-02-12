using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField] private Player owner;
    [SerializeField] private Sector sector;
    [SerializeField] private int level;
    [SerializeField] private Color color;
    [SerializeField] private bool selected = false;

	[SerializeField] private Material level1Material;
	[SerializeField] private Material level2Material;
	[SerializeField] private Material level3Material;
	[SerializeField] private Material level4Material;
	[SerializeField] private Material level5Material;


    public Player GetOwner() {
        return owner;
    }

    public void SetOwner(Player owner) {
        this.owner = owner;
    }

    public Sector GetSector() {
        return sector;
    }

    public void SetSector(Sector sector) {
        this.sector = sector;
    }

    public int GetLevel() {
        return level;
    }

    public void SetLevel(int level) {
        this.level = level;
    }

    public Color GetColor() {
        return color;
    }

    public void SetColor(Color color) {
        this.color = color;
    }

    public bool IsSelected() {
        return selected;
    }

    public void SetSelected(bool selected) {
        this.selected = selected;
    }



    public void Initialize(Player player, Sector sector) {

        // initialize the unit to be owned by the specified 
        // player and in the specified sector


        // set the owner, level, and color of the unit
        owner = player;
        level = 1;
        color = Color.white;

        // set the material color to the player color
        GetComponent<Renderer>().material.color = color;

        // place the unit in the sector
        MoveTo(sector);

    }

    public void MoveTo(Sector targetSector) {

        // move the unit into the target sector, capturing it
        // and levelling up if necessary


        // clear the unit's current sector
        if (this.sector != null)
        {
            this.sector.ClearUnit();
        }   

        // set the unit's sector to the target sector
        // and the target sector's unit to the unit
        this.sector = targetSector;
        targetSector.SetUnit(this);
		Transform targetTransform = targetSector.transform.Find ("Units").transform;

        // set the unit's transform to be a child of
        // the target sector's transform
        transform.SetParent(targetTransform);

        // align the transform to the sector
        transform.position = targetTransform.position;


        // if the target sector belonged to a different 
        // player than the unit, capture it and level up
        if (targetSector.GetOwner() != this.owner)
        {
            // level up
            LevelUp();

            // capture the target sector for the owner of this unit
            owner.Capture(targetSector);
        }

    }

    public void SwapPlacesWith(Unit otherUnit) {

        // switch the sectors of this unit and another unit


        // swap the sectors' references to the units
        this.sector.SetUnit(otherUnit);
        otherUnit.sector.SetUnit(this);


        // get the index of this unit's sector in the map's list of sectors
        int tempSectorIndex = -1;
        for (int i = 0; i < this.owner.GetGame().gameMap.GetComponent<Map>().sectors.Length; i++)
        {
            if (this.sector == this.owner.GetGame().gameMap.GetComponent<Map>().sectors[i])
                tempSectorIndex = i;
        }

        // swap the units' references to their sectors
        this.sector = otherUnit.sector;
        otherUnit.sector = this.owner.GetGame().gameMap.GetComponent<Map>().sectors[tempSectorIndex] ;


        // realign transforms for each unit
		this.transform.SetParent(this.sector.transform.Find("Units").transform);
		this.transform.position = this.sector.transform.Find("Units").position;

		otherUnit.transform.SetParent(otherUnit.sector.transform.Find("Units").transform);
		otherUnit.transform.position = otherUnit.sector.transform.Find("Units").position;
        
    }

	public void LevelUp() {

        // level up the unit, capping at Level 5

		if (level < 5) {

			// increase level
			level++;

			// change texture to reflect new level
			switch (level) 
			{
			case 2:
				this.gameObject.GetComponent<MeshRenderer> ().material = level2Material;
				break;
			case 3:
				this.gameObject.GetComponent<MeshRenderer> ().material = level3Material;
				break;
			case 4:
				this.gameObject.GetComponent<MeshRenderer> ().material = level4Material;
				break;
			case 5:
				this.gameObject.GetComponent<MeshRenderer> ().material = level5Material;
				break;
			default:
				this.gameObject.GetComponent<MeshRenderer> ().material = level1Material;
				break;
			}

			// set material color to match owner color
			GetComponent<Renderer>().material.color = color;

		}
		
	}

    public void Select() {

        // select the unit and highlight the sectors adjacent to it

        selected = true;
        sector.ApplyHighlightAdjacent();
    }

    public void Deselect() {

        // deselect the unit and unhighlight the sectors adjacent to it

        selected = false;
        sector.RevertHighlightAdjacent();
    }

    public void DestroySelf() {

        // safely destroy the unit by removing it from its owner's
        // list of units before destroying

        sector.ClearUnit();
        owner.units.Remove(this);
        Destroy(this.gameObject);
    }
        
}
