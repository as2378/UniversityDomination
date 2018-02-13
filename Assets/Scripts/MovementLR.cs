using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLR : MonoBehaviour
{
    public float movementSpeed = 10;
    public int BookScore;
    public int BeerScore;
    public int lastpersonBeerScore;
    public int lastpersonBookScore;
    private GameObject mainCamera;
    private GameObject DropperCamera;
    private GameObject GUI;
    // Use this for initialization
    public void Start()
    {

        mainCamera = GameObject.Find("Main Camera");
        GUI = GameObject.Find("GUI");
        DropperCamera = GameObject.Find("DropperCamera");
        mainCamera.SetActive(true);
        GUI.SetActive(true);
        DropperCamera.SetActive(false);

    }

    public int StartDropperGame()
    {
        mainCamera.SetActive(false);
        GUI.SetActive(true);
        DropperCamera.SetActive(false);
        for (int i = 0; i < 20; i++)
        {
            float spawn = Random.Range(0.3f, 1.75f);
            InvokeRepeating("SpawnBeer", 2.0f, spawn);

        }
        lastpersonBookScore = BookScore;
        lastpersonBeerScore = BeerScore;
        return 4;
        mainCamera.SetActive(true);
        GUI.SetActive(true);
        DropperCamera.SetActive(false);
    }

    void SpawnBeer()
    {
        int selector = Random.Range(0, 4);
        print(selector);

        if (selector == 0)
        {
            GameObject myRoadInstance =
               Instantiate(Resources.Load("Book"),
               new Vector3(Random.Range(-100.0f, 100.0f), 240, 250),
               Quaternion.identity) as GameObject;
        }
        if (selector == 1)
        {
            GameObject myRoadInstance =
               Instantiate(Resources.Load("Beer"),
               new Vector3(Random.Range(-100.0f, 100.0f), 240, 250),
               Quaternion.identity) as GameObject;
        }
        if (selector == 2 || selector == 3)
        {
            GameObject myRoadInstance =
               Instantiate(Resources.Load("Bin"),
               new Vector3(Random.Range(-100.0f, 100.0f), 240, 250),
               Quaternion.identity) as GameObject;
        }
    }

    // Update is called once per frame
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
