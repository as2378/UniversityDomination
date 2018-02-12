using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Menu : MonoBehaviour {
    [SerializeField] private GameObject InGame;
    [SerializeField] private GameObject loadButton;

	void Start () {
        /*
         * ADDITION: 11/02/2018
         * The if statement decides if the load button has to be showed.
         * It's showed if the savedGame.gd exists in Application.persistentDataPath 
         * */
        if(File.Exists(Path.Combine(Application.persistentDataPath, "savedGame.gd"))) {
            loadButton.SetActive(true);
        }
	}

    /*
     * ADDITION: 11/02/2018
     * When the start button in Menu UI is clicked, this method
     * is invoked and a new game is initialized.
     * */
	public void StartButtonClicked() {
        GameObject.Find("GameManager").GetComponent<Game>().Initialize();
        InGame.SetActive(true);
        GameObject.Find("OutGame").SetActive(false);
        gameObject.SetActive(false);
	}

    /*
     * ADDITION: 11/02/2018
     * When the load button in Menu UI is clicked, this method
     * is inoked and the saved game is loaded.
     * */
    public void LoadButtonClicked() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Path.Combine(Application.persistentDataPath, "savedGame.gd"), FileMode.Open);

        Game game = (Game)bf.Deserialize(file);
        file.Close();

        Game currentGame = GameObject.Find("GameManager").GetComponent<Game>();
        currentGame = game;
        currentGame.Initialize();

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
    public void SaveButtonClicked() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Path.Combine(Application.persistentDataPath, "savedGame.gd"));
        bf.Serialize(file, GameObject.Find("Map").GetComponent<Map>());
        file.Close();

        gameObject.SetActive(false);
    }
}
