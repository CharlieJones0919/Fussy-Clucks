using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public int sceneNum;
    public int numOfLevels;

  //  public bool beginningOfGame;
    public int numOfTips;
    public int tutorialCounter;
    public string[] tutorialSteps;

    public FinanceController finances;
    public UIController uiOutput;
    public float goalGold;

    private Vector3 inactivePosition = new Vector3(0.0f, -5.0f, 0.0f);
    public bool seedAvailable;

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
        finances = transform.gameObject.GetComponent<FinanceController>();
        sceneNum = System.Convert.ToInt32(SceneManager.GetActiveScene().name.Substring(6, 1));
        seedAvailable = false;
        finances.gold = sceneNum * 40;
        numOfLevels = 3;

        switch (sceneNum)
        {
            case 1:
                goalGold = 250;
                hutchPoolSize = 2;
                break;
            case 2:
                goalGold = 500;
                hutchPoolSize = 3;
                break;
            case 3:
                goalGold = 1000;
                hutchPoolSize = 5;
                break;
        }

        tutorialCounter = 0;
        numOfTips = 8;
        tutorialSteps = new string[numOfTips];
        tutorialSteps[tutorialCounter] = "TIP: Buy a NEST to keep your eggs and chickens warm. (Don't let them get too hot).";
        tutorialCounter++;
        tutorialSteps[tutorialCounter] = "TIP: Now buy an EGG to go in your NEST.";
        tutorialCounter++;
        tutorialSteps[tutorialCounter] = "TIP: NESTS heat up your EGGS, CHICKYS and ROOSIRS.";
        tutorialCounter++;
        tutorialSteps[tutorialCounter] = "TIP: You can buy SEED to feed your chickens.";
        tutorialCounter++;
        tutorialSteps[tutorialCounter] = "TIP: Pickup your chickens to raise their affection level.";
        tutorialCounter++;
        tutorialSteps[tutorialCounter] = "TIP: Chickens that are RESTED, FED, WARM and LOVED produce more coins.";
        tutorialCounter++;
        tutorialSteps[tutorialCounter] = "TIP: Buy a HUTCH for your chickens to sleep in.";
        tutorialCounter++;
        tutorialSteps[tutorialCounter] = "TIP: ROOSIRS produce more coins.";
        tutorialCounter = 0;

        chickyPoolSize = sceneNum * 6;
        seedPoolSize = chickyPoolSize * 3;
        coinPoolSize = chickyPoolSize * 10;
        nestPoolSize = sceneNum * 3;

        LoadPrefabPool(UIPrefab);
        LoadPrefabPool(cameraSetupPrefab);
        LoadPrefabPool(lightingSetupPrefab);

        LoadPrefabPool(chickyPrefab, chickyPoolSize, levelChickens);
        LoadPrefabPool(seedPrefab, seedPoolSize, levelSeed);
        LoadPrefabPool(coinPrefab, coinPoolSize, levelCoins);
        LoadPrefabPool(nestPrefab, nestPoolSize, levelNests);
        LoadPrefabPool(hutchPrefab, hutchPoolSize, levelHutches);

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
        whichObject.SetActive(false);
        whichList[whichObject] = true;
    }

    public void DeactivateObject(GameObject whichObject, Dictionary<GameObject, bool> whichList, string deathMessage)
    {
        whichObject.transform.position = inactivePosition;
        whichObject.SetActive(false);
        whichList[whichObject] = true;

        uiOutput.DisplayTip(deathMessage);
        CheckLevelFailed();
    }

    public Dictionary<GameObject, bool> GetChuckPool()
    {
        return levelChickens;
    }

    public bool CheckLevelFailed()
    {
        if (finances.gold < finances.GetItemPrice("Chicky Egg"))
        {
            foreach (KeyValuePair<GameObject, bool> chicken in levelChickens)
            {
                if (chicken.Value == false)
                {
                    return false;
                }
            }
            uiOutput.DisplayFailScreen();
            return true;
        }

        return false;
    }

    public bool CheckLevelSuccess()
    {
        if (finances.gold >= goalGold)
        {
            uiOutput.DisplaySuccessScreen();
            return true;
        }
        return false;
    }

    public void DestroyLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
