using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestOccupation : MonoBehaviour
{
    private GameObject chickyInNest;
    public bool nestOccupied;

    // Start is called before the first frame update
    void Start()
    {
        nestOccupied = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Chuck")
        {
            if (!nestOccupied)
            {
                chickyInNest = collision.gameObject;
                nestOccupied = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Chuck")
        {
            if (collision.gameObject == chickyInNest)
            {
                nestOccupied = false;
            }
        }
    }
}
