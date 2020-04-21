using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to an object that isn't deleted between scene loads to maintain what level the game is on as a static integer.
public class SceneMaintainance : MonoBehaviour
{
    private static int sceneNum = 1; //Stores which level the player is on starting at 1. Maintained between loads so only 1 scene needs to exist within the build to save space.

    //Before the game starts, don't delete the first level instance of this object, but destroy any that are created as a result of the object being in the initial setup of the scene.
    private void Awake()
    {
        if (sceneNum == 1)
        {
            DontDestroyOnLoad(this);
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }

    //Used to retrieve a copy of the sceneNum int as a non-static int in LevelLoader.
    public int GetSceneNum()
    {
        return sceneNum;
    }

    //Used by LevelLoader to increment the level if the player acquires the goalGold amount.
    public void IncreaseSceneNum()
    {
        sceneNum++;
    }
}
