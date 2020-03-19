using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CreateChicky : MonoBehaviour
{
    private GameObject chickyControllerObject;
    public string breedName;
    public GameObject breedModel;

    public Dictionary<GameObject, bool> levelChickens = new Dictionary<GameObject, bool>();
    public GameObject chickyPrefab;

    private Vector3 inactivePos;
    private Vector3 spawnPos;
    private GameObject activeChicky;

    private string sceneName;
    private int poolSize;

    private void Start()
    {
        chickyControllerObject = GameObject.Find("ChickyVariablesController");

        inactivePos = new Vector3(0,-5,0);

        sceneName = SceneManager.GetActiveScene().name;
        sceneName = sceneName.Substring(6 , 1);
        poolSize = System.Convert.ToInt32(sceneName) * 5;

        LoadChickyPool();
    }

    private void LoadChickyPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = (GameObject)Instantiate(chickyPrefab, inactivePos, Quaternion.identity);
            obj.SetActive(false);
            levelChickens.Add(obj, true);
        }
    }

    public void ActivateChicky()
    {
        foreach (KeyValuePair<GameObject, bool> obj in levelChickens)
        {
            if (obj.Value == true)
            {
                activeChicky = obj.Key;

                levelChickens[activeChicky] = false;

                spawnPos = new Vector3(0.0f, Random.Range(2.0f, 6.0f), 0.0f);
                activeChicky.transform.position = spawnPos;
                activeChicky.GetComponent<Chuck>().HasJustBeenCreated();
                activeChicky.GetComponent<Chuck>().SetBreed(breedName, breedModel);
                activeChicky.SetActive(true);
                break;
            }
        }
    }

    public void DeactivateChicky(GameObject deadChicky)
    {
        deadChicky.transform.position = inactivePos;
        deadChicky.SetActive(false);
        levelChickens[deadChicky] = true;
    }
}