using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class UnitTest 
{
    private Game game;
    private Map map;
	private Player[] players;
	private PlayerUI[] gui;
    private GameObject unitPrefab;

    [UnityTest]
    public IEnumerator MoveToFriendlyFromNull_UnitInCorrectSector() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving from null
        unit.SetSector(null);
        sectorA.SetUnit(null);
        unit.SetOwner(playerA);
        sectorA.SetOwner(playerA);

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.GetSector() == sectorA);
        Assert.IsTrue(sectorA.GetUnit() == unit);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToNeutral_UnitInCorrectSector() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Player playerA = players[0];

        // test moving from one sector to another
        unit.SetSector(sectorA);
        unit.SetOwner(playerA);
        sectorA.SetUnit(unit);
        sectorB.SetUnit(null);
        sectorA.SetOwner(playerA);
        sectorB.SetOwner(playerA);

        unit.MoveTo(sectorB);
        Assert.IsTrue(unit.GetSector() == sectorB);
        Assert.IsTrue(sectorB.GetUnit() == unit);
        Assert.IsNull(sectorA.GetUnit());

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToFriendly_UnitInCorrectSector() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving into a friendly sector (no level up)
        unit.SetLevel(1);
        unit.SetSector(null);
        sectorA.SetUnit(null);
        unit.SetOwner(playerA);
        sectorA.SetOwner(playerA);

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.GetLevel() == 1);

        yield return null;
    }

    public IEnumerator MoveToHostile_UnitInCorrectSectorAndLevelUp() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];
        Player playerB = players[1];

        // test moving into a non-friendly sector (level up)
        unit.SetLevel(1);
        unit.SetSector(null);
        sectorA.SetUnit(null);
        unit.SetOwner(playerA);
        sectorA.SetOwner(playerB);

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.GetLevel() == 2);
        Assert.IsTrue(sectorA.GetOwner() == unit.GetOwner());

        yield return null;
    }

    [UnityTest]
    public IEnumerator SwapPlaces_UnitsInCorrectNewSectors() {

        Setup();

        Unit unitA = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Unit unitB = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Player player = players[0];

        // places players unitA in sectorA
        unitA.SetOwner(player);
        unitA.SetSector(sectorA);
        sectorA.SetUnit(unitA); 

        // places players unitB in sectorB
        unitB.SetOwner(player);
        unitB.SetSector(sectorB);
        sectorB.SetUnit(unitB); 

        unitA.SwapPlacesWith(unitB);
        Assert.IsTrue(unitA.GetSector() == sectorB); // unitA in sectorB
        Assert.IsTrue(sectorB.GetUnit() == unitA); // sectorB has unitA
        Assert.IsTrue(unitB.GetSector() == sectorA); // unitB in sectorA
        Assert.IsTrue(sectorA.GetUnit() == unitB); // sectorA has unitB

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelIncreasesByOne() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();

        // ensure LevelUp increments level as expected
        unit.SetLevel(1);
        unit.LevelUp();
        Assert.IsTrue(unit.GetLevel() == 2);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelDoesNotPastFive() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();

        // ensure LevelUp does not increment past 5
        unit.SetLevel(5);
        unit.LevelUp();
        Assert.IsTrue(unit.GetLevel() == 5);

        yield return null;
    }
        
    [UnityTest]
    public IEnumerator SelectAndDeselect_SelectedTrueWhenSelectedFalseWhenDeselected() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sector = map.sectors[0];

        unit.SetSector(sector);
        unit.SetSelected(false);

        unit.Select();
        Assert.IsTrue(unit.IsSelected());

        unit.Deselect();
        Assert.IsFalse(unit.IsSelected());

        yield return null;
    }


    [UnityTest]
    public IEnumerator DestroySelf_UnitNotInSectorAndNotInPlayersUnitsList() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sector = map.sectors[0];
        Player player = players[0];

        unit.SetSector(sector);
        sector.SetUnit(unit);

        unit.SetOwner(player);
        player.units.Add(unit);

        unit.DestroySelf();

        Assert.IsNull(sector.GetUnit()); // unit not on sector 
        Assert.IsFalse(player.units.Contains(unit)); // unit not in list of players units

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

        // establish references from each player to the game
        foreach (Player player in players)
        {
            player.SetGame(game);
        }

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

        // extract the unit prefab from the player class
        unitPrefab = players[0].GetUnitPrefab();
    }
}