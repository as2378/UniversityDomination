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


	/**
	 * Initialize(Player player, Sector sector):
	 * Initialize the unit to be owned by the specified player and in the specified sector.
	 * It's level is also set to 1.
	 */
    public void Initialize(Player player, Sector sector) {
        // set the owner, level, and color of the unit
        owner = player;
        level = 1;
        color = Color.white;

        // set the material color to the player color
        GetComponent<Renderer>().material.color = color;

        // place the unit in the sector
        MoveTo(sector);
    }

	/**
	 * MoveTo(Sector targetSector):
	 * This method move the unit into the target sector, capturing it and levelling up if necessary.
	 * It also detects if the targetSector contains a PVC and calls the PVC minigame if the targetSector
	 * is not owned by the unit's owner and the owner is human.
	 */
    public void MoveTo(Sector targetSector) {
		if (this.sector != null) // clear the unit's current sector
        {
            this.sector.ClearUnit();
        }   

		this.sector = targetSector; //set the unit's sector to the target sector.
		targetSector.SetUnit(this); //set the target sector's unit to the unit
		Transform targetTransform = targetSector.transform.Find ("Units").transform;

		transform.SetParent(targetTransform); //set the unit's transform to be a child of the target sector's transform  
		transform.position = targetTransform.position; // align the transform to the sector
        
		if (targetSector.GetOwner() != this.owner) // if the target sector belonged to a different player than the unit, capture it and level up
        {
			//ADDITION: 13/02/18 this will allow the beer/knowledge scores of the previous owner to decrease.
			if (targetSector.GetPVC() == true && targetSector.GetOwner() != null)
            {
                GameObject Catcher = GameObject.Find("Catcher");
                MovementLR CatcherMovement = Catcher.GetComponent<MovementLR>();
                int currentBeer = targetSector.GetOwner().GetBeer();
                int currentBook = targetSector.GetOwner().GetKnowledge();
                targetSector.GetOwner().SetBeer(currentBeer - CatcherMovement.GetlastBeer());
                targetSector.GetOwner().SetKnowledge(currentBook - CatcherMovement.GetlastBook());
            }
   
			LevelUp();
			owner.Capture(targetSector); // capture the target sector for the owner of this unit

			//ADDITION: 13/02/18  This detects if the target sector contains a PVC, and if so runs the minigame.
			if (targetSector.GetPVC() == true)
			{
				GameObject Catcher = GameObject.Find("Catcher");
				MovementLR CatcherMovement = Catcher.GetComponent<MovementLR>();

				if (this.owner.GetType () == typeof(NonHumanPlayer)) //ADDITION: 14/02/18 added a check if the AI captures a PVC
				{ 
					CatcherMovement.lastpersonBeerScore = 4;	//Give the AI 2 beer & 2 books for catching the PVC.
					CatcherMovement.lastpersonBookScore = 4;
					owner.SetBeer (owner.GetBeer () + 2);
					owner.SetKnowledge (owner.GetKnowledge () + 2);
				} 
				else 
				{
					CatcherMovement.StartDropperGame(this);  //Start the minigame if player is human.
				}      
			}
        }
    }

	/**
	 * addScoreFromDropper:
	 * Increases the owner's beer & knowledge scores by beerScore & bookScore respectively.
	 * This is called when the dropper PVC minigame ends.
	 * ADDITION: 14/02/18
	 */
	public void addScoreFromDropper(int beerScore, int bookScore)
    {
        int currentBeer = this.owner.GetBeer();
        int currentBook = this.owner.GetKnowledge();
		this.owner.SetBeer(currentBeer + beerScore);
		this.owner.SetKnowledge(currentBook + bookScore);

		owner.GetGui ().UpdateDisplay ();
    }

	/**
	 * SwapPlacesWith(Unit otherUnit):
	 * Switch the sectors of this unit and otherUnit unit.
	 */
    public void SwapPlacesWith(Unit otherUnit) {
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

	/**
	 * LevelUp()
	 * Increases the level of the unit by 1, capping it at level 5.
	 * Updates the unit's material to match.
	 */
	public void LevelUp() {
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

	/**
	 * Select():
	 * Selects the unit and highlights the sectors adjacent to it.
	 */
    public void Select() {
        selected = true;
        sector.ApplyHighlightAdjacent();
    }

	/**
	 * Deselect():
	 * Deselects the unit and unhighlights the sectors adjacent to it.
	 */
    public void Deselect() {
        selected = false;
        sector.RevertHighlightAdjacent();
    }

	/**
	 * DestroySelf():
	 * safely destroy the unit by removing it from its owner's list of units before destroying
	 */
    public void DestroySelf() {
        sector.ClearUnit();
        owner.units.Remove(this);
        Destroy(this.gameObject);
    }     
}
