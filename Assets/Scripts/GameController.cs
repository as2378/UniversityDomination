using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public PlayerController[] players;
	public GameObject gameMap;

    public PlayerController currentPlayer;
    public TurnState turnState;
    public bool gameFinished = false;

    public enum TurnState { Move1, Move2, EndOfTurn, NULL };

    public void CreatePlayers(int numberOfPlayers){

        // mark the specified number of players as human
        // and set Player 1 as the current player


        // ensure that the specified number of players
        // is at least 2 and does not exceed 4
        if (numberOfPlayers < 2)
            numberOfPlayers = 2;

        if (numberOfPlayers > 4) 
            numberOfPlayers = 4;

        // mark the specified number of players as human
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players[i].isHuman = true;
        }
    }

	public void InitializeMap () {

        // initialize all sectors, allocate players to landmarks,
        // and spawn units


		// get an array of all sectors
        SectorController[] sectors = gameMap.GetComponentsInChildren<SectorController> ();

		// initialize each sector
        foreach (SectorController sector in sectors)
		{
            sector.Initialize ();
		}

		// get a list of all sectors containing landmarks
        List<SectorController> landmarkedSectors = new List<SectorController>();
		foreach (SectorController sector in sectors)
		{
			if (sector.hasLandmark)
			{
                landmarkedSectors.Add (sector);
			}
		}

		// randomly allocate sectors to players
        foreach (PlayerController player in players) 
		{
			bool playerAllocated = false;
			do {
				// choose a landmarked sector at random
                int sectorIndex = Random.Range (0, landmarkedSectors.Count);
				
                // if the sector is not yet allocated, allocate the player
                if (((SectorController) landmarkedSectors[sectorIndex]).owner == null)
				{
                    player.Capture(landmarkedSectors[sectorIndex]);
					playerAllocated = true;
				}

            // if the chosen sector is already allocated, try again
			} while (!playerAllocated);
		}

		// spawn units for each player
        foreach (PlayerController player in players)
        {
            player.SpawnUnits();
        }

	}

    public bool NoUnitSelected() {
        
        // return true if no unit is selected, false otherwise


        // scan through each player
        foreach (PlayerController player in players)
        {
            // scan through each unit of each player
            foreach (UnitController unit in player.units)
            {
                // if a selected unit is found, return false
                if (unit.isSelected == true)
                    return false;
            }
        }

        // otherwise, return true
        return true;
    }

    private void NextPlayer() {

        // set the current player to the next player in the order


        // deactivate the current player
        currentPlayer.isActive = false;

        // find the index of the current player
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == currentPlayer)
            {
                // set the next player's index
                int nextPlayerIndex = i + 1;

                // if end of player list is reached, loop
                if (nextPlayerIndex == players.Length)
                {
                    currentPlayer = players[0];
                    players[0].isActive = true;
                }

                // otherwise, set the next player as the current player
                else
                {
                    currentPlayer = players[nextPlayerIndex];
                    players[nextPlayerIndex].isActive = true;
                    break;
                }
            }
        }
    }
       
    public void NextTurnState() {

        // change the game state to the next in the order,
        // or to initial game state if turn is completed

        switch (turnState)
        {
            case TurnState.Move1:
                turnState = TurnState.Move2;
                break;
            case TurnState.Move2:
                turnState = TurnState.EndOfTurn;
                break;
            case TurnState.EndOfTurn:
                turnState = TurnState.Move1;
                break;
            default:
                break;
        }
    }

    public void EndTurn() {

        // end the current turn

        turnState = TurnState.EndOfTurn;
    }

    public PlayerController GetWinner() {

        // return the winning player, or null if no winner yet

        PlayerController winner = null;

        // scan through each player
        foreach (PlayerController player in players)
        {
            // if the player hasn't been eliminated
            if (!player.IsEliminated())
            {
                // if this is the first player found that hasn't been eliminated,
                // assign the player as the winner
                if (winner == null)
                    winner = player;

                // if another player still hasn't been eliminated,
                // then the game isn't over, so return null
                else
                    return null;
            }
        }

        // if only one player hasn't been eliminated, then return it as the winner
        return winner;
    }

    public void EndGame() {
        gameFinished = true;
        currentPlayer.isActive = false;
        currentPlayer = null;
        turnState = TurnState.NULL;
        Debug.Log("GAME FINISHED");
    }

	void Start () {
        
        // initialize the game


        // create a specified number of human players
        // *** currently hard-wired to 2 for testing ***
        CreatePlayers(2);

        // initialize the map
        InitializeMap();

        // initialize the game state
        turnState = TurnState.Move1;

        // set Player 1 as the current player
        currentPlayer = players[0];
        players[0].isActive = true;

	}


    private IEnumerator GameLoop ()
    {
        yield return StartCoroutine(PhaseOne());

        yield return StartCoroutine(PhaseTwo());

        // ...

        /*
        if ( << no winner >> )
        {
            << end game >>
        }
        else
        {
            // restart game loop
            StartCoroutine (GameLoop());
        }
        
        */
    }

    private IEnumerator PhaseOne(){
        yield return 0;
    }

    private IEnumerator PhaseTwo(){
        yield return 0;
    }

    // ...

	// Update is called once per frame
    void Update () {
		
        if (turnState == TurnState.EndOfTurn)
        {
            if (GetWinner() == null)
            {
                // start the next player's turn
                NextPlayer();
                NextTurnState();

                while (currentPlayer.IsEliminated())
                    NextPlayer();

                // spawn units for the next player
                currentPlayer.SpawnUnits();
            }
            else
                if (!gameFinished)
                    EndGame();
        }
	}

}
