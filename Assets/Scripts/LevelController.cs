using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public bool beginningOfGame;
    public bool seedAvailable;

    private int sceneNum;
    public FinanceController finances;
    public StatOutput uiOutput;
    private Vector3 inactivePosition = new Vector3(0.0f, -5.0f, 0.0f);

    public Dictionary<GameObject, bool> levelChickens = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelSeed = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelCoins = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelNests = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelHutches = new Dictionary<GameObject, bool>();

    public int chickyPoolSize;
    public int seedPoolSize;
    public int coinPoolSize;
    public int nestPoolSize;
    public int hutchPoolSize;

    public GameObject chickyPrefab;
    public GameObject seedPrefab;
    public GameObject coinPrefab;
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
        finances = this.gameObject.GetComponent<FinanceController>();
        sceneNum = System.Convert.ToInt32(SceneManager.GetActiveScene().name.Substring(6, 1));

        finances.gold = 35;
        seedAvailable = false;

        if (sceneNum == 1)
        {
            beginningOfGame = true;
        }
        else
        {
            beginningOfGame = false;
        }

        chickyPoolSize = sceneNum * 5;
        seedPoolSize = chickyPoolSize * 2;
        coinPoolSize = chickyPoolSize * 5;
        nestPoolSize = sceneNum * 3;
        hutchPoolSize = sceneNum * 2;

        LoadPrefabPool(chickyPrefab, chickyPoolSize, levelChickens);
        LoadPrefabPool(seedPrefab, seedPoolSize, levelSeed);
        LoadPrefabPool(coinPrefab, coinPoolSize, levelCoins);
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

    public void DeactivateObject(GameObject whichObject, Dictionary<GameObject, bool> whichList)
    {
        whichObject.transform.position = inactivePosition;

        if (whichList == levelChickens)
        {
            whichObject.GetComponent<Chuck>().Initialize();
        }

        whichObject.SetActive(false);
        whichList[whichObject] = true;
    }

    public Dictionary<GameObject, bool> GetChuckPool()
    {
        return levelChickens;
    }
}
