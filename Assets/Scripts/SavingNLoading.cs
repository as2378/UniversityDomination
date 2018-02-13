using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class SavingNLoading : MonoBehaviour
{
    public class SavedGame
    {
        public string currentPlayer;
        public int numberOfTurns;
        public bool PVCExists;
        public Game.TurnState turnState;

        public Dictionary<string, SavedPlayer> players = new Dictionary<string, SavedPlayer>();
    }

    public class SavedPlayer
    {
        public string name;
        public int beer;
        public int knowledge;
        public List<float> color = new List<float>();
        public bool human;
        public bool active;
        
        public Dictionary<string, bool> sectors = new Dictionary<string, bool>();
        public List<SavedUnit> units = new List<SavedUnit>();
    }

    public class SavedUnit
    {
        public string sector;
        public int level;
    }

    public string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, "savedGame.gd");
    }

    public void saveGame()
    {
        Game currentGame = GameObject.Find("GameManager").GetComponent<Game>();
        SavedGame savedGame = new SavedGame();

        // Save the values of the fields of Game
        savedGame.currentPlayer = currentGame.currentPlayer.name;
        savedGame.numberOfTurns = currentGame.numberOfTurns;
        savedGame.PVCExists = currentGame.PVCExists;
        savedGame.turnState = currentGame.GetTurnState();
        
        for(int i = 0; i < currentGame.players.Length; i++)
        {
            Player player = currentGame.players[i];
            SavedPlayer playerState = new SavedPlayer();

            // Save the properties directly related to the player
            playerState.name = player.name;
            playerState.beer = player.GetBeer();
            playerState.knowledge = player.GetKnowledge();

            playerState.color.Add(player.GetColor().r);
            playerState.color.Add(player.GetColor().g);
            playerState.color.Add(player.GetColor().b);
            playerState.color.Add(player.GetColor().a);

            playerState.human = player.IsHuman();
            playerState.active = player.IsActive();

            // Save the player's owned sectors
            foreach(Sector sector in player.ownedSectors)
            {
                playerState.sectors.Add(sector.name, sector.GetPVC());
            }

            // Save the player's units
            foreach(Unit unit in player.units)
            {
                SavedUnit playerUnit = new SavedUnit();

                playerUnit.sector = unit.GetSector().name;
                playerUnit.level = unit.GetLevel();

                playerState.units.Add(playerUnit);
            }

            savedGame.players.Add(player.name, playerState);
        }

        // Save it on the system
        File.WriteAllText(GetPath(), JsonConvert.SerializeObject(savedGame));
    }

    public SavedGame loadGame()
    {
        string json = File.ReadAllText(GetPath());

        return JsonConvert.DeserializeObject<SavedGame>(json);
    }
}
