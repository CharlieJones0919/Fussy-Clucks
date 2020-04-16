using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public Dictionary<GameObject, bool> levelChickens = new Dictionary<GameObject, bool>();
    public GameObject chickyPrefab;
    private Vector3 chickySpawnPos = new Vector3(0.0f, -5.0f, 0.0f);
    private int cloneNumIndex = -1;

    private string sceneName;
    private int poolSize;

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        sceneName = sceneName.Substring(6, 1);
        poolSize = System.Convert.ToInt32(sceneName) * 5;

        LoadChickyPool();
    }

    private void LoadChickyPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            cloneNumIndex++;

            GameObject chickyClone = (GameObject)Instantiate(chickyPrefab, chickySpawnPos, Quaternion.identity);
            chickyClone.SetActive(false);
            levelChickens.Add(chickyClone, true);
            chickyClone.GetComponent<Chuck>().chickyProps.cloneNum = cloneNumIndex;
        }

        cloneNumIndex = -1;
    }

    public void DeactivateChicky(GameObject deadChicky)
    {
        deadChicky.transform.position = chickySpawnPos;
        deadChicky.SetActive(false);
        levelChickens[deadChicky] = true;
    }

    public Dictionary<GameObject, bool> GetChuckPool()
    {
        return levelChickens;
    }
}
