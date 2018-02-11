using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public Player[] players;
	public GameObject gameMap;
    public Player currentPlayer;

	public Color[] playerColours = new Color[]{new Color(0.8f,0,0),new Color(0.7f,0,0.95f),new Color(0.8f,0.8f,0),new Color(0,0.8f,0)}; //ADDITION: added in order to assign generated players a colour.
	public GameObject[] unitPrefabs;		// ADDITION: added in order to assign generated players units.

    //This is used for delaying the PVC's spawn
    private int numberOfTurns = 1;

    private bool PVCExists = false;

    public enum TurnState { Move1, Move2, EndOfTurn, NULL };
    [SerializeField] private TurnState turnState;
    [SerializeField] private bool gameFinished = false;
    [SerializeField] private bool testMode = false;
    [SerializeField] private int delayPVCBy = 10; // turns

    public TurnState GetTurnState() {
        return turnState;
    }

    public void SetTurnState(TurnState turnState) {
        this.turnState = turnState;
    }

    public bool IsFinished() {
        return gameFinished;
    }

    public void EnableTestMode() {
        testMode = true;
    }

    public void DisableTestMode() {
        testMode = false;
    }


	/*
	 * CHANGED: 31/01/18
	 * Added the functionality to generate new players, without doing it manually in the inspector.
	 * This allows us to have two classes; Player and NonHumanPlayer, which will help when programming the AI.
	 */
    public void CreatePlayers(int numberOfPlayers){
		this.players = new Player[4]; //ADDITION

        // ensure that the specified number of players
        // is at least 2 and does not exceed 4
        if (numberOfPlayers < 2)
            numberOfPlayers = 2;

        if (numberOfPlayers > 4) 
            numberOfPlayers = 4;

		for (int i = 0; i < 4; i++) 
		{
			//Creates a player GameObject and gives it either a Player script or a NonHumanPlayer script.
			GameObject newPlayerGameObject = new GameObject ();
			newPlayerGameObject.transform.parent = this.gameObject.transform;
			newPlayerGameObject.name = "Player" + (i+1);
			if (i < numberOfPlayers) 
			{
				newPlayerGameObject.AddComponent<Player> ();
				this.players [i] = newPlayerGameObject.GetComponent<Player> ();
				this.players [i].SetHuman (true);
			} 
			else 
			{
				newPlayerGameObject.AddComponent<NonHumanPlayer> ();
				this.players [i] = newPlayerGameObject.GetComponent<NonHumanPlayer> ();
				this.players [i].SetHuman (false);
			}

			//Updates the properties of the Player class. 
			Player newPlayer = newPlayerGameObject.GetComponent<Player> ();
			PlayerUI newUI = GameObject.Find ("GUI/Player" + (i + 1) + "UI").GetComponent<PlayerUI> ();
			newPlayer.SetUnitPrefab (unitPrefabs [i]);
			newPlayer.SetColor (playerColours [i]);
			newPlayer.SetGui (newUI);
			newPlayer.SetGame (this);

			//Initialize the newPlayer's GUI.
			newUI.Initialize (newPlayer, i + 1);
		}
		this.currentPlayer = this.players [0];
		/*
        // mark the specified number of players as human
		for (int i = 0; i < numberOfPlayers; i++)
		{
			players[i].SetHuman(true);
		}

		// give all players a reference to this game
		// and initialize their GUIs
		for (int i = 0; i < 4; i++)
		{
			players[i].SetGame(this);
			players[i].GetGui().Initialize(players[i], i + 1);
		}
		*/
    }

    /*
     * ADDITION: 27/01/18
     * Added the checks to ensure there is at most 1 PVC on the map.
     * */
	public void InitializeMap() {

        // initialize all sectors, allocate players to landmarks,
        // and spawn units


		// get an array of all sectors
        Sector[] sectors = gameMap.GetComponentsInChildren<Sector>();

		// initialize each sector
        foreach (Sector sector in sectors)
		{
            sector.Initialize();
		}
            
		// get an array of all sectors containing landmarks
        Sector[] landmarkedSectors = GetLandmarkedSectors(sectors);
            
        // ensure there are at least as many landmarks as players
        if (landmarkedSectors.Length < players.Length)
        {
            throw new System.Exception("Must have at least as many landmarks as players; only " + landmarkedSectors.Length.ToString() + " landmarks found for " + players.Length.ToString() + " players.");
        }


		// randomly allocate sectors to players
        foreach (Player player in players) 
		{
			bool playerAllocated = false;
            while (!playerAllocated) {
                
				// choose a landmarked sector at random
                int randomIndex = Random.Range (0, landmarkedSectors.Length);
				
                // if the sector is not yet allocated, allocate the player
                if (((Sector) landmarkedSectors[randomIndex]).GetOwner() == null)
				{
                    player.Capture(landmarkedSectors[randomIndex]);
					playerAllocated = true;
				}

                // retry until player is allocated
			}
		}

		// spawn units for each player
        foreach (Player player in players)
        {
            player.SpawnUnits();
        }

        // Ensure there is at most 1 PVC on the map
        int numberOfPVC = CountPVC();

        if(numberOfPVC == 1) {
            PVCExists = true;
        }
        else if(numberOfPVC > 1) {
            throw new System.Exception("There can be at most 1 PVC on the map at any time.");
        }
	}

    private Sector[] GetLandmarkedSectors(Sector[] sectors) {

        // return a list of all sectors that contain landmarks from the given array

        List<Sector> landmarkedSectors = new List<Sector>();
        foreach (Sector sector in sectors)
        {
            if (sector.GetLandmark() != null)
            {
                landmarkedSectors.Add(sector);
            }
        }

        return landmarkedSectors.ToArray();
    }

    public bool NoUnitSelected() {
        
        // return true if no unit is selected, false otherwise


        // scan through each player
        foreach (Player player in players)
        {
            // scan through each unit of each player
            foreach (Unit unit in player.units)
            {
                // if a selected unit is found, return false
                if (unit.IsSelected() == true)
                    return false;
            }
        }

        // otherwise, return true
        return true;
    }

    /*
     * ADDITION: 27/01/18
     * Added PVC spawning. At the start of each turn the game checks if
     * it's time to randomly spawn the PVC.
     */
    public void NextPlayer() {

        // set the current player to the next player in the order


        // deactivate the current player
        currentPlayer.SetActive(false);
		currentPlayer.GetGui().Deactivate();

        // find the index of the current player
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == currentPlayer)
            {
                // set the next player's index
                int nextPlayerIndex = i + 1;

                // if end of player list is reached, loop back to the first player
                if (nextPlayerIndex == players.Length)
                {
                    currentPlayer = players[0];
                    players[0].SetActive(true);
					players[0].GetGui().Activate();
                }

                // otherwise, set the next player as the current player
                else
                {
                    currentPlayer = players[nextPlayerIndex];
                    players[nextPlayerIndex].SetActive(true);
					players[nextPlayerIndex].GetGui().Activate();
                    break;
                }
            }
        }

        numberOfTurns += 1;
        SpawnPVC();
    }

    /*
     * ADDITION: 27/01/18
     * This method is called every time NextPlayer() is invoked.
     * 
     * The method will spawn the PVC randomly on the map if
     * numberOfTurns == delayPVCBy
     * */
    private void SpawnPVC() {
        if(PVCExists) {
            return;
        }

        if(numberOfTurns == delayPVCBy) {
            Sector[] sectors = gameMap.GetComponentsInChildren<Sector>();
            bool spawned = false;

            while(!spawned) {
                int randomIndex = Random.Range(0, sectors.Length);
                Sector sector = sectors[randomIndex];

                if(sector.GetLandmark() == null && sector.GetOwner() == null) {
                    sector.SpawnPVC();
                    spawned = true;
                }
            }            
        }
    }

    /*
     * ADDITION: 27/01/18
     * CountPVC() will return the number of PVCs on the map
     * */
    private int CountPVC() {
        Sector[] sectors = gameMap.GetComponentsInChildren<Sector>();
        int counter = 0;

        foreach(Sector sector in sectors) {
            if(sector.GetPVC()) {
                counter++;
            }
        }

        return counter;
    }
       
    public void NextTurnState() {

        // change the turn state to the next in the order,
        // or to initial turn state if turn is completed

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

		UpdateGUI();
    }

    public void EndTurn() {

        // end the current turn

        turnState = TurnState.EndOfTurn;
    }

    public Player GetWinner() {

        // return the winning player, or null if no winner yet

        Player winner = null;

        // scan through each player
        foreach (Player player in players)
        {
            // if the player hasn't been eliminated
            if (!player.IsEliminated())
            {
                // if this is the first player found that hasn't been eliminated,
                // assume the player is the winner
                if (winner == null)
                    winner = player;

                // if another player that was not eliminated was already,
                // found, then return null
                else
                    return null;
            }
        }

        // if only one player hasn't been eliminated, then return it as the winner
        return winner;
    }

    public void EndGame() {
        gameFinished = true;
        currentPlayer.SetActive(false);
        currentPlayer = null;
        turnState = TurnState.NULL;
        Debug.Log("GAME FINISHED");
    }

	public void UpdateGUI() {

		// update all players' GUIs
		for (int i = 0; i < 4; i++) {
			players [i].GetGui ().UpdateDisplay ();
		}
	}
        
    public void Initialize () {
        
        // initialize the game


        // create a specified number of human players
        // *** currently hard-wired to 2 for testing ***
        CreatePlayers(2);

        // initialize the map and allocate players to landmarks
        InitializeMap();

        // initialize the turn state
        turnState = TurnState.Move1;

        // set Player 1 as the current player
        currentPlayer = players[0];
		currentPlayer.GetGui().Activate();
        players[0].SetActive(true);

		// update GUIs
		UpdateGUI();

	}

	/*
	 * ADDITION: 01/02/18
	 * Checks to see if the current player is a non-human player, and if so, calls its makeMove method.
	 * This has been added to allow for the AI functionality.
	 */
	public void nonHumanPlayerTurn()
	{
		if (this.currentPlayer != null) 
		{
			if (this.currentPlayer.GetType () == typeof(NonHumanPlayer)) 
			{
				NonHumanPlayer compPlayer = (NonHumanPlayer)this.currentPlayer;
				compPlayer.makeMove (this.turnState);
			}
		}
	}
        
	/*
	 * CHANGED: 01/02/18
	 * Removed duplication. Calls UpdateAccessible instead.
	 */
	void Update () {			
		// if test mode is not enabled
		if (!testMode)
		{       
			this.UpdateAccessible ();
		}
	}

	/*
	 * CHANGED: 01/02/18
	 * Added a call to nonHumanPlayerTurn(), so that when it is the AI's turn, its makeMove method is invoked.
	 */
	public void UpdateAccessible () {
		// at the end of each turn, check for a winner and end the game if
		// necessary; otherwise, start the next player's turn
		// can be called by other classes (for testing)

		if (turnState == TurnState.EndOfTurn)
		{
			// if there is no winner yet
			if (GetWinner() == null)
			{
				// start the next player's turn
				NextPlayer();
				NextTurnState();

				// skip eliminated players
				while (currentPlayer.IsEliminated())
					NextPlayer();

				// spawn units for the next player
				currentPlayer.SpawnUnits();
				nonHumanPlayerTurn ();
			}
			else
				if (!gameFinished)
					EndGame();
		}
    }

    /*
     * ADDITION: 11/02/2018
     * Passes the turn to the next player.
     * Invoked when the PassTurnButton is clicked.
     */
    public void PassTurn()
    {
        Sector[] sectors = gameMap.GetComponentsInChildren<Sector>();

        foreach (Sector sector in sectors)
        {
            Unit unit = sector.GetUnit();

            if (unit != null && unit.IsSelected())
            {
                unit.Deselect();
            }
        }

        EndTurn();
    }
}
