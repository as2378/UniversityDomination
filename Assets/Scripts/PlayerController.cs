using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public List <SectorController> ownedSectors;
	public List <UnitController> units;
	public int beer = 0;
	public int knowledge = 0;
	public Color color;
    public bool isHuman;
    public GameObject unitPrefab;
    public Transform unitTransform;

    public GameController game;
    public bool isActive = false;


    public void Capture(SectorController sector) {

        // capture the given sector


        // store a copy of the sector's previous owner
        PlayerController previousOwner = sector.owner;

        // add the sector to the list of owned sectors
        ownedSectors.Add(sector);

        // remove the sector from the previous owner's
        // list of sectors
        if (previousOwner != null)
            previousOwner.ownedSectors.Remove(sector);

        // set the sector's owner to this player
        sector.SetOwner(this);

        // if the sector contains a landmark
        if (sector.hasLandmark)
        {
            LandmarkController landmark = sector.GetComponentInChildren<LandmarkController>();

            // remove the landmark's resource bonus from the previous
            // owner and add it to this player
            if (landmark.resourceType == LandmarkController.ResourceType.Beer)
            {
                this.beer += landmark.amount;
                if (previousOwner != null)
                    previousOwner.beer -= landmark.amount;
            }
            else if (landmark.resourceType == LandmarkController.ResourceType.Knowledge)
            {
                this.knowledge += landmark.amount;
                if (previousOwner != null)
                    previousOwner.knowledge -= landmark.amount;
            }
        }
    }

    public void SpawnUnits() {

        // spawn a unit at each unoccupied landmark


        // scan through each owned sector
		foreach (SectorController sector in ownedSectors) 
		{
            // if the sector contains a landmark and is unoccupied
            if (sector.hasLandmark && sector.unit == null)
            {
                // instantiate a new unit at the sector
                UnitController newUnit = Instantiate(unitPrefab).GetComponent<UnitController>();

                // initialize the new unit
                newUnit.Initialize(this, sector);

                // add the new unit to the player's list of units and 
                // the sector's unit parameters
                units.Add(newUnit);
                sector.unit = newUnit;
                Debug.Log("      Unit spawned at sector " + sector.id.ToString());
            }
		}
	}

    public bool IsEliminated() {

        // returns true if the player is eliminated, false otherwise;
        // a player is considered eliminated if it has no units left
        // and does not own a landmark

        if (units.Count == 0 && !OwnsLandmark())
            return true;
        else
            return false;
    }

    private bool OwnsLandmark() {

        // returns true if the player owns at least one landmark,
        // false otherwise


        // scan through each owned sector
        foreach (SectorController sector in ownedSectors)
        {
            // if a landmarked sector is found, return true
            if (sector.hasLandmark)
                return true;
        }

        // otherwise, return false
        return false;
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
