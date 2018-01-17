using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PlayerTest 
{
    private Game game;
    private Map map;
	private Player[] players;
	private PlayerUI[] gui;

    [UnityTest]
    public IEnumerator CaptureSector_ChangesOwner() {
        
        Setup();
        game.InitializeMap();

        Player previousOwner = map.sectors[0].GetOwner();
     //   bool run = false; // used to decide whether to check previous players sector list (if no previous owner, do not look in list)

       // if (map.sectors[0].GetOwner() != null)
       // {            
       //     run = true;
       // }

        game.players[0].Capture(map.sectors[0]);
        Assert.AreSame(map.sectors[0].GetOwner(), game.players[0]); // owner stored in sector
        Assert.IsTrue(game.players[0].ownedSectors.Contains(map.sectors[0])); // sector is stored as owned by the player

		if (/*run == true*/previousOwner != null) // if sector had previous owner
        {
            Assert.IsFalse(previousOwner.ownedSectors.Contains(map.sectors[0])); // sector has been removed from previous owner list
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_BothPlayersBeerAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.GetLandmark();
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        playerB.Capture(landmarkedSector);

        // ensure 'landmarkedSector' is a landmark of type Beer
        Assert.IsNotNull(landmarkedSector.GetLandmark());
        landmark.SetResourceType(Landmark.ResourceType.Beer);

        // get beer amounts for each player before capture
        int attackerBeerBeforeCapture = playerA.GetBeer();
        int defenderBeerBeforeCapture = playerB.GetBeer();
        Player previousOwner = landmarkedSector.GetOwner();

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.GetOwner(), playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are transferred correctly
        Assert.IsTrue(attackerBeerBeforeCapture + landmark.GetAmount() == playerA.GetBeer());
        Assert.IsTrue(defenderBeerBeforeCapture - landmark.GetAmount() == previousOwner.GetBeer());

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_BothPlayersKnowledgeAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.GetLandmark();
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        playerB.Capture(landmarkedSector);

        // ensure 'landmarkedSector' is a landmark of type Knowledge
        Assert.IsNotNull(landmarkedSector.GetLandmark());
        landmark.SetResourceType(Landmark.ResourceType.Knowledge);

        // get knowledge amounts for each player before capture
        int attackerKnowledgeBeforeCapture = playerA.GetKnowledge();
        int defenderKnowledgeBeforeCapture = playerB.GetKnowledge();
        Player previousOwner = landmarkedSector.GetOwner();

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.GetOwner(), playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are transferred correctly
        Assert.IsTrue(attackerKnowledgeBeforeCapture + landmark.GetAmount() == playerA.GetKnowledge());
        Assert.IsTrue(defenderKnowledgeBeforeCapture - landmark.GetAmount() == previousOwner.GetKnowledge());

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_NeutralLandmarkPlayerBeerAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.GetLandmark();
        Player playerA = game.players[0];

        // ensure 'landmarkedSector' is a landmark of type Beer
        Assert.IsNotNull(landmarkedSector.GetLandmark());
        landmark.SetResourceType(Landmark.ResourceType.Beer);

        // get player beer amount before capture
        int oldBeer = playerA.GetBeer();

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.GetOwner(), playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are gained correctly
        Assert.IsTrue(playerA.GetBeer() - oldBeer == landmark.GetAmount());
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_NeutralLandmarkPlayerKnowledgeAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.GetLandmark();
        Player playerA = game.players[0];

        // ensure 'landmarkedSector' is a landmark of type Knowledge
        Assert.IsNotNull(landmarkedSector.GetLandmark());
        landmark.SetResourceType(Landmark.ResourceType.Knowledge);

        // get player knowledge amount before capture
        int oldKnowledge = playerA.GetKnowledge();

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.GetOwner(), playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are gained correctly
        Assert.IsTrue(playerA.GetKnowledge() - oldKnowledge == landmark.GetAmount());

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_SpawnedWhenLandmarkOwnedAndUnoccupied() {
        
        Setup();

        Sector landmarkedSector = map.sectors[1]; 
        Player playerA = game.players[0];

        // ensure that 'landmarkedSector' is a landmark and does not contain a unit
        landmarkedSector.Initialize();
        landmarkedSector.SetUnit(null);
        Assert.IsNotNull(landmarkedSector.GetLandmark());

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure a unit has been spawned for playerA in landmarkedSector
        Assert.IsTrue(playerA.units.Contains(landmarkedSector.GetUnit()));

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_NotSpawnedWhenLandmarkOwnedAndOccupied() {

        Setup();

        Sector landmarkedSector = map.sectors[1]; 
        Player playerA = game.players[0];

        // ensure that 'landmarkedSector' is a landmark and contains a Level 5 unit
        landmarkedSector.Initialize();
        landmarkedSector.SetUnit(MonoBehaviour.Instantiate(playerA.GetUnitPrefab()).GetComponent<Unit>());
        landmarkedSector.GetUnit().SetLevel(5);
        landmarkedSector.GetUnit().SetOwner(playerA);
        Assert.IsNotNull(landmarkedSector.GetLandmark());

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure a Level 1 unit has not spawned over the Level 5 unit already in landmarkedSector
        Assert.IsTrue(landmarkedSector.GetUnit().GetLevel() == 5);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_NotSpawnedWhenLandmarkNotOwned() {

        Setup();

        Sector landmarkedSector = map.sectors[1]; 
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        landmarkedSector.SetUnit(null);

        // ensure that 'landmarkedSector' is a landmark and does not contain a unit
        landmarkedSector.Initialize();
        landmarkedSector.SetUnit(null);
        Assert.IsNotNull(landmarkedSector.GetLandmark());

        playerB.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure no unit is spawned at landmarkedSector
        Assert.IsNull(landmarkedSector.GetUnit());

        yield return null;
    }

    [UnityTest]
    public IEnumerator IsEliminated_PlayerWithNoUnitsAndNoLandmarksEliminated() {
        
        Setup();
        game.InitializeMap();

        Player playerA = game.players[0];

        Assert.IsFalse(playerA.IsEliminated()); // not eliminated because they have units

        for (int i = 0; i < playerA.units.Count; i++)
        {
            playerA.units[i].DestroySelf(); // removes units
        }
        Assert.IsFalse(playerA.IsEliminated()); // not eliminated because they still have a landmark

        // player[0] needs to lose their landmark
        for (int i = 0; i < playerA.ownedSectors.Count; i++)
        {
            if (playerA.ownedSectors[i].GetLandmark() != null)
            {
                playerA.ownedSectors[i].SetLandmark(null); // player[0] no longer has landmarks
            }
        }
        Assert.IsTrue(playerA.IsEliminated());

        yield return null;
    }


    private void Setup() {
        
        // initialize the game, map, and players with any references needed
        // the "GameManager" asset contains a copy of the GameManager object
        // in the 4x4 Test, but its script lacks references to players & the map
        game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager")).GetComponent<Game>();

        // the "Map" asset is a copy of the 4x4 Test map, complete with
        // adjacent sectors and landmarks at (0,1), (1,3), (2,0), and (3,2),
        // but its script lacks references to the game & sectors
        map = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Map")).GetComponent<Map>();

        // the "Players" asset contains 4 prefab Player game objects; only
        // references not in its script is each player's color
        players = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Players")).GetComponentsInChildren<Player>();

		// the "GUI" asset contains the PlayerUI object for each Player
		gui = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GUI")).GetComponentsInChildren<PlayerUI>();

		// the "Scenery" asset contains the camera and light source of the 4x4 Test
		// can uncomment to view scene as tests run, but significantly reduces speed
		//MonoBehaviour.Instantiate(Resources.Load<GameObject>("Scenery"));

        // establish references from game to players & map
        game.players = players;
        game.gameMap = map.gameObject;
        game.EnableTestMode();

        // establish references from map to game & sectors (from children)
        map.game = game;
        map.sectors = map.gameObject.GetComponentsInChildren<Sector>();

        // establish references to SSB 64 colors for each player
        players[0].SetColor(Color.red);
        players[1].SetColor(Color.blue);
        players[2].SetColor(Color.yellow);
        players[3].SetColor(Color.green);

		// establish references to a PlayerUI and Game for each player & initialize GUI
		for (int i = 0; i < players.Length; i++) 
		{
			players[i].SetGui(gui[i]);
			players[i].SetGame(game);
			players[i].GetGui().Initialize(players[i], i + 1);
		}
    }
}