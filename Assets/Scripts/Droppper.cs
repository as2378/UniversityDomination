using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ADDITION: 12/02/18
 * Droppper is a class attached to the falling objects in the dropper game.
 * It causes the objects to rotate and checks to see if they have been caught or have fallen passed the catcher.
 * When this occurs, the item will be destroyed.
 */
public class Droppper : MonoBehaviour
{
    public float dropSpeed = 10;
    public float rotateSpeed = 10;
    public int BeerScore;
    public int BookScore;

    /**
     * Update():
     * Called once per frame and is used to control the rotation of the item.
     * It also checks to see if the item has been caught by the catcher or if it has been missed.
     * ADDITION: 12/02/18
     */
    void Update()
    {
        GameObject catcher = GameObject.Find("Catcher");

        MovementLR Store = catcher.GetComponent<MovementLR>();

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