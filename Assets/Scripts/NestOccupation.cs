using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to each nest object for to ensure only one chicken can be placed in it at once.
public class NestOccupation : MonoBehaviour
{
    public GameObject outerNestWalls;   //A reference to the nest's outer collider object.

    //Called on start when the script is first activated along with the nest.
    private void OnEnable()
    {
        outerNestWalls.SetActive(false);    //By default on start the outer nest collider object isn't active so chickens can be placed into it.
    }

    //When an object enters the nest's trigger collider (the "InsideNest" collider without physics), and it's a chicken, the outer collider walls are set to active so other chickens can't be put into it while it's occupied.
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Chuck")
        {
            outerNestWalls.SetActive(true);
        }
    }

    //When the chicken exits the "InsideNest" trigger collider (as it would be when the chicken is picked up as the pickup height would remove it from the collider), then deactivate the outerwall colliders so the chicken can be removed.
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Chuck")
        {
            outerNestWalls.SetActive(false);
        }
    }
}
