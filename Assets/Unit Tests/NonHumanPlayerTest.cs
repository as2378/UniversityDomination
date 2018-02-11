using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
/*
 * NonHumanPlayerTest:
 * Tests the NonHumanPlayer methods, ensuring that the game's AI works correctly.
 * 
 * ADDITION: 10/02/18
 */
public class NonHumanPlayerTest {
	private Game game;
	private Map map;
	private PlayerUI[] gui;

	/**
	 * MakeMove_executesValidMove():
	 * Tests if the NonHumanPlayer correctly makes a turn through calling the method makeMove.
	 * This tests if the unit moves from its origin using two movements by asserting that the number of 
	 * ownedSectors increases by 2 and the unit's sector differs from before the turn was made.
	 * 
	 * ADDITION: 10/02/18
	 */
	[UnityTest]
	public IEnumerator MakeMove_executesValidMove(){
		Setup ();
		Player nonHumanPlayer = game.players [2];
		game.currentPlayer = nonHumanPlayer;	//Set current player to the NonHumanPlayer
		game.currentPlayer.SetActive(true);		//Activate player

		foreach (Player player in game.players) {
			if (player != nonHumanPlayer) {
				for(int i = 0; i < player.units.Count; i++){	//remove units from other players.
					player.units[i].DestroySelf();
				}
			}
		}

		int expectedNumberOfSectorsOwned = nonHumanPlayer.ownedSectors.Count + 2; //expected that the NonHumanPlayer will make a move into an unowned sector.
		Sector unitsOriginSector = nonHumanPlayer.units[0].GetSector();

		NonHumanPlayer computerPlayer = (NonHumanPlayer)nonHumanPlayer;
		computerPlayer.makeMove (game.GetTurnState ());

		Assert.AreEqual (expectedNumberOfSectorsOwned, nonHumanPlayer.ownedSectors.Count);
		Assert.AreNotSame (unitsOriginSector, nonHumanPlayer.units [0].GetSector ());

		yield return null;
	}

	/**
	 * FindBestMove_OneBestMoveThreeBadMoves()
	 * Tests if findBestMove returns the best possible move when given an array of possible moves where one
	 * contains a move which is better than the rest.
	 * This test provides FindBestMove with an array of sectors owned by the NonHumanPlayer, which do not contain
	 * units, the PVC or landmarks, however, element 1 of the array is owned by an 'enemy' player (which is better).
	 * 
	 * ADDITION: 10/02/18
	 */
	[UnityTest]
	public IEnumerator FindBestMove_OneBestMoveThreeBadMoves(){
		NonHumanPlayer nonHumanPlayer = new GameObject ("nonHumanPlayer").AddComponent<NonHumanPlayer> ();
		Unit newUnit = new GameObject ("Unit").AddComponent<Unit> ();
		newUnit.SetOwner (nonHumanPlayer);

		Sector[] possibleMoves = new Sector[4];
		Sector expectedBestMove = null;
		for (int i = 0; i < 4; i++) 
		{
			if (i == 1) {
				Player enemyPlayer = new GameObject("EnemyPlayer").AddComponent<Player>();
				possibleMoves [i] = createSector (enemyPlayer, null, null, null, false); //create a sector with a move score better than the others.
				expectedBestMove = possibleMoves [i];
			} else {
				possibleMoves [i] = createSector (nonHumanPlayer, null, null, null, false); //create a sector with a score of 0.
			}
			possibleMoves [i].name = "New_Sector #" + i;
		}

		KeyValuePair<Sector,int> bestMove = nonHumanPlayer.findBestMove (newUnit, possibleMoves);
		Sector bestSector = bestMove.Key;

		Assert.AreSame(expectedBestMove,bestSector); //Tests if the best move has been chosen.

		yield return null;
	}

	/**
	 * FindBestMove_AllMovesSameScore()
	 * Tests if findBestMove returns any one of the possible moves when given an array of possible moves which
	 * have the same 'score'.
	 * This test provides FindBestMove with an array of sectors owned by the NonHumanPlayer, which do not contain
	 * units, the PVC or landmarks.
	 * 
	 * ADDITION: 10/02/18
	 */
	[UnityTest]
	public IEnumerator FindBestMove_AllMovesSameScore(){
		NonHumanPlayer nonHumanPlayer = new GameObject ("nonHumanPlayer").AddComponent<NonHumanPlayer> ();
		Unit newUnit = new GameObject ("Unit").AddComponent<Unit> ();
		newUnit.SetOwner (nonHumanPlayer);

		Sector[] possibleMoves = new Sector[4];
		for (int i = 0; i < 4; i++) 
		{
			possibleMoves [i] = createSector (nonHumanPlayer,null,null,null,false);
			possibleMoves [i].name = "New_Sector #" + i;
		}

		KeyValuePair<Sector,int> bestMove = nonHumanPlayer.findBestMove (newUnit, possibleMoves);
		Sector bestSector = bestMove.Key;

		Assert.IsNotNull (bestSector); //Tests if a vaild move has been chosen.
		Assert.Contains (bestSector, possibleMoves); //Tests if the sector chosen is one of the possible moves.

		yield return null;
	}

	/**
	 * FindBestMove_NoPossibleMoves():
	 * This tests if a pair <null,-1> is returned by findBestMove when given an empty array of possible moves.
	 * 
	 * ADDITION: 10/02/18
	 */
	[UnityTest]
	public IEnumerator FindBestMove_NoPossibleMoves(){
		NonHumanPlayer nonHumanPlayer = new GameObject ("nonHumanPlayer").AddComponent<NonHumanPlayer> ();
		Unit newUnit = new GameObject ("Unit").AddComponent<Unit> ();
		newUnit.SetOwner (nonHumanPlayer);

		KeyValuePair<Sector,int> bestMove = nonHumanPlayer.findBestMove(newUnit, new Sector[0]); //No valid moves
		Assert.IsNull (bestMove.Key);			//The best move should be null.
		Assert.AreEqual (-1, bestMove.Value);	//The score of the best move should be -1.

		yield return null;
	}

	/**
	 * createSector():
	 * Used to generate new sectors with the attributes given in the parameters of this function.
	 * This method is required for a number of the NonHumanPlayer tests.
	 * Returns: the sector created.
	 * 
	 * ADDITION: 10/02/18
	 */
	private Sector createSector(Player owner, Unit unit, Map sectorMap, Landmark landmark, bool containsPVC){
		Sector newSector = new GameObject ().AddComponent<Sector> ();
		newSector.SetUnit (unit);
		newSector.SetMap (sectorMap);
		newSector.SetLandmark (landmark);
		if (containsPVC) 
		{
			newSector.SpawnPVC ();
		}
		newSector.gameObject.AddComponent<MeshRenderer> ();
		newSector.SetOwner (owner);

		return newSector;
	}

	/**
	 * Setup():
	 * Used to load a basic version of the game for testing.
	 * 
	 * ADDITION: 10/02/18
	 */
	private void Setup(){
		// initialize the game, map, and players with any references needed
		// the "GameManager" asset contains a copy of the GameManager object
		// in the 4x4 Test, but its script lacks references to players & the map
		game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager")).GetComponent<Game>();

		// the "Map" asset is a copy of the 4x4 Test map, complete with
		// adjacent sectors and landmarks at (0,1), (1,3), (2,0), and (3,2),
		// but its script lacks references to the game & sectors
		map = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Map")).GetComponent<Map>();

		// the "GUI" asset contains the PlayerUI object for each Player
		gui = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GUI")).GetComponentsInChildren<PlayerUI>();
		gui [0].gameObject.transform.parent.gameObject.name = "GUI";

		// establish references from game to players & map
		// game.players = players;
		game.gameMap = map.gameObject;

		// establish references from map to game & sectors (from children)
		map.game = game;
		map.sectors = map.gameObject.GetComponentsInChildren<Sector>();

		game.Initialize (); 	//Initialize the game.

		game.EnableTestMode(); // enable game's test mode
	}

	// ADDITION: 10/02/18	- Clears all objects in the scene after a test has run.
	[TearDown] 
	public void ClearSceneAfterTest(){
		GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
		foreach (GameObject gameObject in objects) {
			GameObject.Destroy (gameObject);
		}
	}
}
