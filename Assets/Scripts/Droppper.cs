using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droppper : MonoBehaviour
{
    public float dropSpeed = 10;
    public float rotateSpeed = 10;
    public int BeerScore;
    public int BookScore;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject catcher = GameObject.Find("Catcher");

        MovementLR Store = catcher.GetComponent<MovementLR>();
        print(catcher.transform.position.x);
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        if (transform.position.y < 35 && transform.position.y > 25 && transform.position.x > catcher.transform.position.x - 25 && transform.position.x < catcher.transform.position.x + 25)
        {
            Store.BeerScore = Store.BeerScore + BeerScore;
            Store.BookScore = Store.BookScore + BookScore;

            Destroy(gameObject);
        }
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }

    }
}