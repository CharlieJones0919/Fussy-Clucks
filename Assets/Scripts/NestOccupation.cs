using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestOccupation : MonoBehaviour
{
    public GameObject chickyInNest;
    public bool nestOccupied;
    public GameObject outerNestWalls;

    // Start is called before the first frame update
    private void OnEnable()
    {
        nestOccupied = false;
        outerNestWalls.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Chuck")
        {
            if (!nestOccupied)
            {
                chickyInNest = collision.gameObject;
                nestOccupied = true;
                outerNestWalls.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Chuck")
        {
            if (collision.gameObject == chickyInNest)
            {
                nestOccupied = false;
                outerNestWalls.SetActive(false);
            }
        }
    }
}
