using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLR : MonoBehaviour
{
    public float movementSpeed = 10;
    public float BookScore;
    public float BeerScore;
    // Use this for initialization
    void Start()
    {
        float spawn = Random.Range(0.3f, 1.75f);
        InvokeRepeating("SpawnBeer", 2.0f, spawn);

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
}
