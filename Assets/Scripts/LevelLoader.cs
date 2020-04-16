using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private string sceneNum;
    public FinanceController finances;

    public Dictionary<GameObject, bool> levelChickens = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelSeed = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelNests = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelHutches = new Dictionary<GameObject, bool>();

    public int chickyPoolSize;
    public int seedPoolSize;
    public int nestPoolSize;
    public int hutchPoolSize;

    public GameObject chickyPrefab;
    public GameObject seedPrefab;
    public GameObject nestPrefab;
    public GameObject hutchPrefab;

    //Single Instance Environmental Assets
    public GameObject cameraSetupPrefab;
    public GameObject lightingSetupPrefab;
    public GameObject UIPrefab;
    public GameObject fencePrefab;
    public GameObject groundPrefab;

    private void Start()
    {
        sceneNum = SceneManager.GetActiveScene().name;
        sceneNum = sceneNum.Substring(6, 1);
        finances = this.gameObject.GetComponent<FinanceController>();

        chickyPoolSize = System.Convert.ToInt32(sceneNum) * 5;
        seedPoolSize = System.Convert.ToInt32(sceneNum) * 5;
        nestPoolSize = System.Convert.ToInt32(sceneNum) * 3;
        hutchPoolSize = System.Convert.ToInt32(sceneNum) * 2;

        LoadPrefabPool(chickyPrefab, chickyPoolSize, levelChickens);
        LoadPrefabPool(seedPrefab, seedPoolSize, levelSeed);
        LoadPrefabPool(nestPrefab, nestPoolSize, levelNests);
        LoadPrefabPool(hutchPrefab, hutchPoolSize, levelHutches);

        LoadPrefabPool(cameraSetupPrefab);
        LoadPrefabPool(lightingSetupPrefab);
        LoadPrefabPool(UIPrefab);

        LoadPrefabPool(fencePrefab);
        LoadPrefabPool(groundPrefab);
    }

    private void LoadPrefabPool(GameObject targetPrefab, int poolSize, Dictionary<GameObject, bool> targetList)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefabClone = (GameObject)Instantiate(targetPrefab, targetPrefab.transform.position, targetPrefab.transform.rotation);
            prefabClone.SetActive(false);
            targetList.Add(prefabClone, true);
        }
    }

    private void LoadPrefabPool(GameObject targetPrefab)
    {
            GameObject prefabClone = (GameObject)Instantiate(targetPrefab, targetPrefab.transform.position, targetPrefab.transform.rotation);
            prefabClone.SetActive(true);
    }

    public void DeactivateChicky(GameObject deadChicky)
    {
        deadChicky.transform.position = chickyPrefab.transform.position;
        deadChicky.SetActive(false);
        levelChickens[deadChicky] = true;
    }

    public Dictionary<GameObject, bool> GetChuckPool()
    {
        return levelChickens;
    }
}
