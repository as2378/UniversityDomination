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
        Landmark landmark = landmarkedSector.GetComponentInChildren<Landmark>();
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        landmarkedSector.SetLandmark(true); // sector 1 is a landmark
        landmark.SetResourceType(Landmark.ResourceType.Beer); // landmark is type beer
        int attackerBeerBeforeCapture = playerA.GetBeer();

        playerB.Capture(landmarkedSector);
        int defenderBeerBeforeCapture = landmarkedSector.GetOwner().GetBeer();
        Player previousOwner = landmarkedSector.GetOwner();

        playerA.Capture(landmarkedSector);
        Assert.IsTrue(landmarkedSector.IsLandmark()); // just double checking sector is landmark 
        Assert.AreSame(landmarkedSector.GetOwner(), playerA); // owner stored in sector
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector)); // sector is stored as owned by the player
        // increase in correct resource

        Assert.IsTrue(attackerBeerBeforeCapture + landmark.GetAmount() == playerA.GetBeer()); // attacker has gained the correct bonus
        Assert.IsTrue(defenderBeerBeforeCapture - landmark.GetAmount() == previousOwner.GetBeer()); // defender has lost the correct bonus

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_BothPlayersKnowledgeAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        Landmark landmark = landmarkedSector.GetComponentInChildren<Landmark>();
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        landmarkedSector.SetLandmark(true); // sector 1 is a landmark
        landmark.SetResourceType(Landmark.ResourceType.Knowledge); // landmark is type knowledge
        int attackerKnowledgeBeforeCapture = playerA.GetKnowledge();

        playerB.Capture(landmarkedSector);
        int defenderKnowledgeBeforeCapture = landmarkedSector.GetOwner().GetKnowledge();
        Player previousOwner = landmarkedSector.GetOwner();

        playerA.Capture(landmarkedSector);
        Assert.IsTrue(landmarkedSector.IsLandmark()); // just double checking sector is landmark 
        Assert.AreSame(landmarkedSector.GetOwner(), playerA); // owner stored in sector
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector)); // sector is stored as owned by the player
        // increase in correct resource

        Assert.IsTrue(attackerKnowledgeBeforeCapture + landmark.GetAmount() == playerA.GetKnowledge()); // attacker has gained the correct bonus
        Assert.IsTrue(defenderKnowledgeBeforeCapture - landmark.GetAmount() == previousOwner.GetKnowledge()); // defender has lost the correct bonus

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_NeutralLandmarkPlayerBeerAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        Landmark landmark = landmarkedSector.GetComponentInChildren<Landmark>();
        Player playerA = game.players[0];
        landmarkedSector.SetLandmark(true); // sector 1 is a landmark
        landmark.SetResourceType(Landmark.ResourceType.Beer); // landmark is type beer
        int oldBeer = playerA.GetBeer();

        playerA.Capture(landmarkedSector);
        Assert.IsTrue(landmarkedSector.IsLandmark()); // just double checking sector is landmark 
        Assert.AreSame(landmarkedSector.GetOwner(), playerA); // owner stored in sector
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector)); // sector is stored as owned by the player
        // increase in correct resource
        Assert.IsTrue(playerA.GetBeer() - oldBeer == landmark.GetAmount()); // attacker has gained the correct bonus
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_NeutralLandmarkPlayerKnowledgeAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        Landmark landmark = landmarkedSector.GetComponentInChildren<Landmark>();
        Player playerA = game.players[0];
        landmarkedSector.SetLandmark(true); // sector 1 is a landmark
        landmark.SetResourceType(Landmark.ResourceType.Knowledge); // landmark is type knowledge
        int oldKnowledge = playerA.GetKnowledge();

        playerA.Capture(landmarkedSector);
        Assert.IsTrue(landmarkedSector.IsLandmark()); // just double checking sector is landmark 
        Assert.AreSame(landmarkedSector.GetOwner(), playerA); // owner stored in sector
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector)); // sector is stored as owned by the player
        Assert.IsTrue(playerA.GetKnowledge() - oldKnowledge == landmark.GetAmount()); // attacker has gained the correct bonus

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_SpawnedWhenLandmarkOwned() {
        
        Setup();

        Sector landmarkedSector = map.sectors[1]; 
        Sector nonLandmarkSector = map.sectors[0]; // sector 0 is not a landmark
        Player playerA = game.players[0];
        landmarkedSector.SetLandmark(true); // sector 1 is a landmark
        landmarkedSector.SetUnit(null);

        Assert.IsEmpty(playerA.units);
        playerA.Capture(nonLandmarkSector);
        playerA.SpawnUnits();
        Assert.IsEmpty(playerA.units); // no unit spawned because playerA has no landmarks

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();
        Assert.IsTrue(playerA.units.Contains(landmarkedSector.GetUnit())); // unit now spawned because playerA has a landmark

        landmarkedSector.GetUnit().SetLevel(5);
        playerA.SpawnUnits();
        Assert.IsTrue(landmarkedSector.GetUnit().GetLevel() == 5); // unit on landmark is not replaced when SpawnUnits called, i.e. no new unit spawned

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
            if (playerA.ownedSectors[i].IsLandmark() == true)
            {
                //game.players[0].ownedSectors[i].SetOwner(game.players[1]);
                playerA.ownedSectors[i].SetLandmark(false); // player[0] no longer has landmarks
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