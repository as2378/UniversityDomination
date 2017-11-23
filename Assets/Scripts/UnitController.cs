using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour {

    public PlayerController owner;
    public SectorController sector;
	public int level;
	public Color color;

    public bool isSelected = false;


    public void Initialize(PlayerController player, SectorController sector) {

        // initialize the unit to be owned by the specified 
        // player and in the specified sector


        // set the owner, level, and color of the unit
        owner = player;
        level = 1;
        color = player.color;

        // set the material color to the player color
        GetComponent<Renderer>().material.color = color;

        // place the unit in the sector
        MoveTo(sector);

    }

    public void MoveTo(SectorController targetSector) {

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
        targetSector.unit = this;

        // set the unit's transform to be a child of
        // the target sector's transform
        transform.SetParent(targetSector.transform);

        // align the transform to the sector
        transform.position = targetSector.transform.position;
        transform.Translate(new Vector3(0, 1, 0));


        // if the target sector belonged to a different 
        // player than the unit, capture it and level up
        if (targetSector.owner != this.owner)
        {
            // level up
            LevelUp();

            // capture the target sector for the owner of this unit
            owner.Capture(targetSector);
        }

    }

    public void SwapPlacesWith(UnitController otherUnit){

        // switch the sectors of this unit and another unit


        // store a copy of the other unit's previous sector
        //UnitController tempUnit = this;
        SectorController tempSector = otherUnit.sector;

        // move the selected unit into this sector
        otherUnit.MoveTo(this.sector);

        // move this unit into the other unit's previous sector
        this.MoveTo(tempSector);
    }

	public void LevelUp() {

        // level up the unit, capping at Level 5

        if (level < 5)
            level++;
	}

    public void Select() {

        // select the unit and highlight the sectors adjacent to it

        isSelected = true;
        sector.ApplyHighlightAdjacent();
    }

    public void Deselect() {

        // deselect the unit and unhighlight the sectors adjacent to it

        isSelected = false;
        sector.RevertHighlightAdjacent();
    }

    public void DestroySelf() {

        // safely destroy the unit by removing it from its owner's
        // list of units before destroying

        owner.units.Remove(this);
        Destroy(this.gameObject);
    }


    /*
    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }
    */
}
