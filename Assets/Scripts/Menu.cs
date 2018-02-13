using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Menu : MonoBehaviour {
    [SerializeField] private GameObject InGame;
	[SerializeField] private GameObject OutGame;
    [SerializeField] private GameObject loadButton;
	[SerializeField] private GameObject newGameMenu;
	[SerializeField] private GameObject endGame;

    void Start()
    {
        /*
         * ADDITION: 11/02/2018
         * The if statement decides if the load button has to be showed.
         * It's showed if the savedGame.gd exists in Application.persistentDataPath 
         * */
        SavingNLoading savingNLoading = GameObject.Find("GameManager").GetComponent<SavingNLoading>();

        if (File.Exists(savingNLoading.GetPath()))
        {
            loadButton.SetActive(true);
        }
    }

    /*
     * ADDITION: 11/02/2018
     * When the start button in Menu UI is clicked, this method
     * is invoked and a new game is initialized.
     * */
    public void StartButtonClicked() {
		int numberOfPlayers = (int) GameObject.Find ("PlayerSlider").GetComponent<Slider> ().value;
        Game game = GameObject.Find("GameManager").GetComponent<Game>();
        game.SetNumberOfPlayers(numberOfPlayers);
        game.Initialize();

        InGame.SetActive(true);
        OutGame.SetActive(false);
        newGameMenu.SetActive(false);
        gameObject.SetActive(false);
	}

    /*
     * ADDITION: 11/02/2018
     * When the load button in Menu UI is clicked, this method
     * is inoked and the saved game is loaded.
     * */
    public void LoadButtonClicked()
    {
        SavingNLoading savingNLoading = GameObject.Find("GameManager").GetComponent<SavingNLoading>();
        SavingNLoading.SavedGame savedGame = savingNLoading.loadGame();

        Game currentGame = GameObject.Find("GameManager").GetComponent<Game>();
        currentGame.InitializeSectors();
        currentGame.players = new Player[savedGame.players.Count];

        // Restore the values of all the fields of Game
        currentGame.numberOfTurns = savedGame.numberOfTurns;
        currentGame.PVCExists = savedGame.PVCExists;
        currentGame.SetTurnState(savedGame.turnState);

        int playerCount = 0;

        foreach (KeyValuePair<string, SavingNLoading.SavedPlayer> savedPlayer in savedGame.players)
        {
            // Restore the player's state
            GameObject playerObject = new GameObject();
            playerObject.transform.parent = currentGame.gameObject.transform;
            playerObject.name = savedPlayer.Value.name;

            if (savedPlayer.Value.human)
            {
                playerObject.AddComponent<Player>();
                currentGame.players[playerCount] = playerObject.GetComponent<Player>();
                currentGame.players[playerCount].SetHuman(true);
            }
            else
            {
                playerObject.AddComponent<NonHumanPlayer>();
                currentGame.players[playerCount] = playerObject.GetComponent<NonHumanPlayer>();
                currentGame.players[playerCount].SetHuman(false);
            }

            Player player = playerObject.GetComponent<Player>();
            PlayerUI ui = GameObject.Find("GUI/" + savedPlayer.Value.name + "UI").GetComponent<PlayerUI>();
            player.SetUnitPrefab(currentGame.unitPrefabs[playerCount]);
            player.SetColor(
                new Color(
                    savedPlayer.Value.color[0],
                    savedPlayer.Value.color[1],
                    savedPlayer.Value.color[2],
                    savedPlayer.Value.color[3]
                )
            );
            player.SetGui(ui);
            player.SetGame(currentGame);
            ui.Initialize(player, playerCount + 1);

            player.SetBeer(savedPlayer.Value.beer);
            player.SetKnowledge(savedPlayer.Value.knowledge);
            player.SetActive(savedPlayer.Value.active);

            if (savedPlayer.Value.active)
            {
                player.GetGui().Activate();
            }

            // Restore the player's sectors
            foreach (KeyValuePair<string, bool> savedSector in savedPlayer.Value.sectors)
            {
                Sector sector = GameObject.Find(savedSector.Key).GetComponent<Sector>();
                player.ownedSectors.Add(sector);
                sector.SetOwner(player);

                if (savedSector.Value)
                {
                    sector.SpawnPVC();
                }
            }

            player.GetGui().UpdateDisplay();

            // Restore the player's units
            foreach (SavingNLoading.SavedUnit savedUnit in savedPlayer.Value.units)
            {
                Unit newUnit = Instantiate(player.GetUnitPrefab()).GetComponent<Unit>();

                Sector sector = GameObject.Find(savedUnit.sector).GetComponent<Sector>();
                newUnit.Initialize(player, sector);

                for (int i = 0; i < savedUnit.level - 1; i++)
                {
                    newUnit.LevelUp();
                }

                player.units.Add(newUnit);
                sector.SetUnit(newUnit);
            }

            if (savedGame.currentPlayer == savedPlayer.Value.name)
            {
                currentGame.currentPlayer = player;
            }

            playerCount++;
        }

        InGame.SetActive(true);
        GameObject.Find("OutGame").SetActive(false);
        gameObject.SetActive(false);
    }

    /*
     * ADDITION: 11/02/2018
     * */
    public void ContinueButtonClicked() {
        gameObject.SetActive(false);
    }

    /*
     * ADDITION: 11/02/2018
     * When the save button is clicked, this method
     * is invoked and the game is saved in Application.persistentDataPath
     * */
    public void SaveButtonClicked()
    {
        SavingNLoading savingNLoading = GameObject.Find("GameManager").GetComponent<SavingNLoading>();
        savingNLoading.saveGame();

        gameObject.SetActive(false);
    }

    /*
	 * ADDITION: 12/02/18
	 * Click functionality for the NewGame button. This button is used to open the number of players menu.
	 */
    public void NewGameButtonClicked(){
		this.newGameMenu.SetActive (true);
		OutGame.SetActive (false);
	}

	/*
	 * ADDITION: 12/02/18
	 * Exit button functionality to allow the user to exit the game once it has finished.
	 */
	public void ExitButtonClicked(){
		Application.Quit ();
	}

	public void ShowGameOverMenu(){
		this.InGame.SetActive (false);
		this.OutGame.SetActive (false);
		this.endGame.SetActive (true);
	}
}
