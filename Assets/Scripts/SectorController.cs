using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorController : MonoBehaviour {

    public UnitController unit;
    public PlayerController owner;
    public SectorController[] adjacentSectors;
    public int id;
	public bool hasLandmark;

    public MapController map;


	public void SetOwner (PlayerController player) {

        // set the owner of the sector to the specified player,
        // and change the sector's color to match the new owner


		// set sector owner to the given player
		owner = player;

		// set sector color to the color of the given player
		// or gray if null
        if (player == null) {
            gameObject.GetComponent<Renderer> ().material.color = Color.gray;
        } else {
            gameObject.GetComponent<Renderer> ().material.color = player.color;
		}
	}

	public void Initialize () {

        // initialize the sector by setting its owner and unit to null
        // and determining if it contains a landmark or not


		// reset owner
		SetOwner(null);

		// clear unit
		unit = null;

        // determine if landmarked
        if (gameObject.GetComponentInChildren<LandmarkController>() != null)
            hasLandmark = true;
        else
            hasLandmark = false;
	}

    public void ApplyHighlight(float amount) {

        // highlight a sector by increasing its RGB values by a specified amount

        Renderer renderer = GetComponent<Renderer>();
        Color currentColor = renderer.material.color;
        Color offset = new Vector4(amount, amount, amount, 1);
        Color newColor = currentColor + offset;

        renderer.material.color = newColor;
    }

    public void RevertHighlight(float amount) {

        // unhighlight a sector by decreasing its RGB values by a specified amount

        Renderer renderer = GetComponent<Renderer>();
        Color currentColor = renderer.material.color;
        Color offset = new Vector4(amount, amount, amount, 1);
        Color newColor = currentColor - offset;

        renderer.material.color = newColor;
    }

    public void ApplyHighlightAdjacent() {

        // highlight each sector adjacent to this one

        foreach (SectorController adjacentSector in adjacentSectors)
        {
            adjacentSector.ApplyHighlight(0.2f);
        }
    }

    public void RevertHighlightAdjacent() {

        // unhighlight each sector adjacent to this one
        
        foreach (SectorController adjacentSector in adjacentSectors)
        {
            adjacentSector.RevertHighlight(0.2f);
        }
    }

    public void ClearUnit() {

        // clear this sector of any unit

        unit = null;
        Debug.Assert(unit == null);
    }

    public void MoveIntoUnoccupiedSector(UnitController unit, SectorController sector)
    {
        // move the selected unit into this sector
        unit.MoveTo(sector);
        Debug.Log("unit moved into unoccupied sector");

        // advance turn state
        sector.map.game.NextTurnState();

    }

    public void SwapUnits(UnitController unit1, UnitController unit2)
    {
        // swap the two units
        unit1.SwapPlacesWith(unit2);
        Debug.Log("two friendly units swapped");

        // advance turn state
        map.game.NextTurnState();

    }

    public void MoveIntoHostileSector(UnitController attackingUnit, UnitController defendingUnit, SectorController targetSector)
    {

        // start and resolve a conflict
        

        // if the attacking unit wins
        if (Conflict(attackingUnit, defendingUnit))
        {
            // destroy defending unit
            defendingUnit.DestroySelf();

            // move the attacking unit into this sector
            attackingUnit.MoveTo(targetSector);
            Debug.Log("attacking unit successfully moved into sector");
        }

        // if the defending unit wins
        else
        {
            // destroy attacking unit
            attackingUnit.DestroySelf();
            Debug.Log("attacking unit destroyed");
        }

        // end the turn
        targetSector.map.game.EndTurn();
    }

    void OnMouseUpAsButton () {

        // when this sector is clicked, determine the context
        // and act accordingly


        // if this sector contains a unit and belongs to the
        // current active player, and if no unit is selected
        if (unit != null && owner.isActive && map.game.NoUnitSelected())
        {
            // select this sector's unit
            unit.Select();
        }

        // if this sector's unit is already selected
        else if (unit != null && unit.isSelected)
        {
            // deselect this sector's unit           
            unit.Deselect();
        }

        // if this sector is adjacent to the sector containing
        // the selected unit
        else if (AdjacentSelectedUnit() != null)
        {
            // get the selected unit
            UnitController selectedUnit = AdjacentSelectedUnit();

            // deselect the selected unit
            selectedUnit.Deselect();

            // if this sector is unoccupied
            if (unit == null)
            {

                MoveIntoUnoccupiedSector(selectedUnit, this);
                /*
                // move the selected unit into this sector
                selectedUnit.MoveTo(this);
                Debug.Log("unit moved into unoccupied sector");

                // advance turn state
                map.game.NextTurnState();
                */
            }

            // if the sector is occupied by a friendly unit
            else if (unit.owner == selectedUnit.owner)
            {
                SwapUnits(this.unit, selectedUnit);
                /*
                // swap the two units
                this.unit.SwapPlacesWith(selectedUnit);
                Debug.Log("two friendly units swapped");

                // advance turn state
                map.game.NextTurnState();
                */
            }

            // if the sector is occupied by a hostile unit
            else if (unit.owner != selectedUnit.owner)
            {
                MoveIntoHostileSector(selectedUnit, this.unit, this);
                /*
                // start and resolve a conflict

                // name the units for ease of reading code
                UnitController attackingUnit = selectedUnit;
                UnitController defendingUnit = unit;

                // if the attacking unit wins
                if (Conflict(attackingUnit, defendingUnit))
                {
                    // destroy defending unit
                    defendingUnit.DestroySelf();

                    // move the attacking unit into this sector
                    attackingUnit.MoveTo(this);
                    Debug.Log("attacking unit successfully moved into sector");
                }

                // if the defending unit wins
                else
                {
                    // destroy attacking unit
                    attackingUnit.DestroySelf();
                    Debug.Log("attacking unit destroyed");
                }

                // end the turn
                map.game.EndTurn();
                */
            }
        }
    }


    private UnitController AdjacentSelectedUnit() {

        // return the selected unit if it is adjacent to this sector
        // return null otherwise


        // scan through each adjacent sector
        foreach (SectorController adjacentSector in adjacentSectors)
        {
            // if the adjacent sector contains the selected unit,
            // return the selected unit
            if (adjacentSector.unit != null && adjacentSector.unit.isSelected)
                return adjacentSector.unit;
        }

        // otherwise, return null
        return null;
    }

    private bool Conflict(UnitController attackingUnit, UnitController defendingUnit) {

        // return 'true' if attacking unit wins;
        // return 'false' if defending unit wins


        /*
         * Conflict resolution is done by comparing a random roll 
         * from each unit involved. The roll is weighted based on
         * the unit's level and the amount of the associated 
         * resource the unit's owner has. Beer is associated with
         * attacking, and Knowledge is associated with defending.
         * 
         * The formula is:
         * 
         *     roll = [ a random integer with a lowerbound of 1
         *              and an upperbound of 4 + the unit's level ] 
         *           + [ the amount of the associated resource the
         *               unit's owner has ]
         * 
         * In the event of a tie, the defending unit wins the conflict
         */

        // calculate the rolls of each unit
        int attackingUnitRoll = Random.Range(1, (5 + attackingUnit.level)) + attackingUnit.owner.beer;
        int defendingUnitRoll = Random.Range(1, (5 + defendingUnit.level)) + defendingUnit.owner.knowledge;

        return (attackingUnitRoll > defendingUnitRoll);
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
