using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ADDITION: 12/02/18
 * MovementLR will control the starting/finishing of the dropper PVC minigame.
 * This class also allows the user to control the catcher using the left/right arrow keys.
 * All attributes & methods within this class are new to assessment 3.
 */
public class MovementLR : MonoBehaviour
{
    public float movementSpeed = 10;
    public int BookScore;
    public int BeerScore;
    public int lastpersonBeerScore;
    public int lastpersonBookScore;
    private GameObject mainCamera;
    private GameObject DropperCamera;
    public bool stopped;
    public int itemCount = 0;
    public float timer;
    public Unit AddingScore;

	private Vector3 startingPosition;
	  
	/**
	 * Start():
	 * This method is run at the start of the game and it shows the maincamera, hides the dropper camera
	 * and assigns initial values to stopped and startingPosition.
	 */
    public void Start()
    {
	    mainCamera = GameObject.Find("Main Camera");
        DropperCamera = GameObject.Find("DropperCamera");
        mainCamera.SetActive(true);
        DropperCamera.SetActive(false);
		stopped = true;
		startingPosition = transform.position;
    }

	/**
	 * StartDropperGame(Unit unit):
	 * This initializes the minigame. It resets values from the previous time the minigame was played, hides the gui elements
	 * and then invokes SpawnItem unit itemCount becomes greater than 20.
	 * ADDITION: 14/02/18
	 */
    public void StartDropperGame(Unit unit)
    {
		transform.position = startingPosition;
        AddingScore = unit;
		itemCount = 0;
        timer = 0;
        BeerScore = 0;
        BookScore = 0;
        mainCamera.SetActive(false);
		GameObject.Find ("GUI").GetComponent<Canvas> ().enabled = false;
        DropperCamera.SetActive(true);
		stopped = false;
     
		InvokeRepeating("SpawnItem", 2.0f, Random.Range(1.0f, 1.75f));
    }

	/**
	 * StopDropperGame():
	 * This hides the dropper game and reshows the main game. It also transfers the current book/beer scores to the lastpersonBook/Beer
	 * score attributes.
	 * ADDITION: 14/02/18
	 */
    public void StopDropperGame()
    {
        lastpersonBookScore = BookScore;
        lastpersonBeerScore = BeerScore;

        mainCamera.SetActive(true);
		GameObject.Find ("GUI").GetComponent<Canvas> ().enabled = true;
        DropperCamera.SetActive(false);
        stopped = true;
    }

	/**
	 * SpawnItem():
	 * Creates either a Bin, Book or Beer item which contain a Dropper class component. They are spawned in a random
	 * position at the top of the screen.
	 * If itemCount > 20, this method will cause the minigame to end.
	 */
    void SpawnItem()
    {
		itemCount++;
        int selector = Random.Range(0, 4);
		string itemType = "Bin";
        if (selector == 0)
        {
			itemType = "Book";
        }
        if (selector == 1)
        {
			itemType = "Beer";
        }
		Instantiate (Resources.Load (itemType),
			new Vector3 (Random.Range (-300, 300), 240, 250),
			Quaternion.identity);
		
		if (itemCount > 20)
        {
            CancelInvoke();
            StopDropperGame();
			AddingScore.addScoreFromDropper(GetBeer(),GetBook());
        }
    }

   /**
    * Update():
    * Called once per frame and is used to control the horizontal movement of the catcher using the arrow keys.
    */
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }
    }
    public int GetBeer()
    {
        int ConvertedScore= Mathf.RoundToInt(BeerScore/2);
        return ConvertedScore;
    }
    public int GetBook()
    {
        int ConvertedScore = Mathf.RoundToInt(BookScore / 2);
        return ConvertedScore;
    }
    public int GetlastBeer()
    {
        int ConvertedScore = Mathf.RoundToInt(lastpersonBeerScore / 2);
        return ConvertedScore;
    }
    public int GetlastBook()
    {
        int ConvertedScore = Mathf.RoundToInt(lastpersonBookScore / 2);
        return ConvertedScore;
    }
}
