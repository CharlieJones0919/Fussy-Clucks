using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ButtonController : MonoBehaviour
{
    private LevelLoader levelLoader;

    public string typeName;
    public GameObject typeModel;

    private GameObject activeChicky;
    private Vector3 activeChickyPos;

    public void Start()
    {
        levelLoader = GameObject.Find("LevelController").GetComponent<LevelLoader>();
    }

    public void ActivateChicky()
    {
        foreach (KeyValuePair<GameObject, bool> chicken in levelLoader.levelChickens)
        {
            if (chicken.Value == true)
            {
                activeChicky = chicken.Key;

                levelLoader.levelChickens[activeChicky] = false;

                activeChickyPos = new Vector3(Random.insideUnitCircle.x, 4.0f + Random.insideUnitCircle.y, Random.insideUnitCircle.y);
                activeChicky.transform.position = activeChickyPos;
                activeChicky.GetComponent<Chuck>().SetType(typeName, typeModel);
                activeChicky.SetActive(true);
                break;
            }
        }
    }
}