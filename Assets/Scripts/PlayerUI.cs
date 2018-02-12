using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //ADDITION: 31/01/18

[System.Serializable]
public class PlayerUI : MonoBehaviour {

	/*
	// Use this for initialization
	void Start () {
		
	} 
	*/

	[SerializeField] private Player player;
	[SerializeField] private Text header;
	[SerializeField] private Text headerHighlight;
	[SerializeField] private Text percentOwned;
	[SerializeField] private Text beer;
	[SerializeField] private Text knowledge;
	[SerializeField] private int numberOfSectors;
	private Color defaultHeaderColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

	/*
	 * CHANGED: 31/01/18
	 * Added an AI label, which will tell the users if the player is human or an AI.
	 * Also, added 'using UnityEngine.UI' to shorten the code below.
	 */
	public void Initialize(Player player, int player_id) {

		this.player = player;

		header = transform.Find("Header").GetComponent<Text>();
		headerHighlight = transform.Find("HeaderHighlight").GetComponent<Text>();
		percentOwned = transform.Find("PercentOwned_Value").GetComponent<Text>();
		beer = transform.Find("Beer_Value").GetComponent<Text>();
		knowledge = transform.Find("Knowledge_Value").GetComponent<Text>();
		numberOfSectors = player.GetGame().gameMap.GetComponent<Map>().sectors.Length;

		header.text = "Player " + player_id.ToString();
		headerHighlight.text = header.text;
		headerHighlight.color = player.GetColor();

		//ADDITION ------------------------------------------------------
		Transform aiLabel = transform.Find ("AI_Label");
		if (aiLabel != null && player.GetType () != typeof(NonHumanPlayer)) 
		{
			aiLabel.GetComponent<Text> ().text = "";
		}
		//---------------------------------------------------------------
	}

	public void UpdateDisplay() {

		percentOwned.text = Mathf.Round(100 * player.ownedSectors.Count / numberOfSectors).ToString() + "%";
		beer.text = player.GetBeer().ToString();
		knowledge.text = player.GetKnowledge().ToString();

	}

	public void Activate() {

		header.color = player.GetColor();

	}

	public void Deactivate() {

		header.color = defaultHeaderColor;

	}
}
