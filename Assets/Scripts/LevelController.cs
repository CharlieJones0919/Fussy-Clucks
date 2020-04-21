using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

//Controls general game values, instantiates all the objects in the scene (except for the LevelController object the FinanceController, ChickyProperties and this script is attached to), into object pools, or as individual prefab clone instances.
public class LevelController : MonoBehaviour
{
    public SceneMaintainance sceneNumScriptRef; //Reference to the script that maintains the level number.
    public ChickyPropertiesController chickyPropController;   //Reference to the properties controller for the Chuck scripts to reference.
    public int sceneNum;    //A duplicate of the scene number from the SceneNumMaintainance script for this instance of the level to access as its more difficult to access a static variable from other classes.

    public int numOfTips;           //How many tip messages there are to iterate through.
    public string[] tutorialSteps;  //A string list of all the tip messages.
    public int tutorialCounter;     //Which number tip message should be displayed in the tip text box from the tutorialSteps list of string messages.

    public FinanceController finances;  //A reference to the FinanceController script, so other scripts which include a reference to LevelController can also access the financial values/function without needing to retrieve the component again for their own reference. Also used in this script.
    public UIController uiOutput;       //A reference to the UIController script for the same reason a reference to the FinancialController is retrieved/stored.
    public float goalGold;              //How value of gold the player must acquire to trigger the Winning panel from which they can proceed to the next level.

    private Vector3 inactivePosition = new Vector3(0.0f, -5.0f, 0.0f);  //Where objects in the object pool should be placed in worldspace when they're not active on the map.     
    public Dictionary<Vector3, bool> seedPositions = new Dictionary<Vector3, bool>(); //The positions seeds have been activated to. Used by the Chuck script so chickens can find activated seed.
    public Dictionary<GameObject, int> coinValue = new Dictionary<GameObject, int>(); //How much each coin pile object in the list is worth depending on the chicken's stats at the time of dropping it.

    //Lists to store/initialise the game objects in for later activation into the game. The boolean is used to check if the object is currently active or not.
    public Dictionary<GameObject, bool> levelChickens = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelSeed = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelCoins = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelNests = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> levelHutches = new Dictionary<GameObject, bool>();

    //The sizes of each object pool. (How many prefab clones should be instantiated and added to their list).
    public int chickyPoolSize;
    public int seedPoolSize;
    public int coinPoolSize;
    public int nestPoolSize;
    public int hutchPoolSize;

    //These store references to the prefabs which the game objects should be clones of when instantiated.
    //Single Instance Environmental Prefabs:
    public GameObject cameraSetupPrefab;
    public GameObject lightingSetupPrefab;
    public GameObject UIPrefab;
    public GameObject fencePrefab;
    public GameObject groundPrefab;
    //Multiple Instance Gameplay Prefabs:
    public GameObject chuckPrefab;      //The script, collider, rigidbody and FSM part of the chickens.
    public GameObject chickyEggPrefab;
    public GameObject chickyPrefab;
    public GameObject roosirEggPrefab;
    public GameObject roosirPrefab;
    public GameObject seedPrefab;
    public GameObject coinPrefab;
    public GameObject nestPrefab;
    public GameObject hutchPrefab;

    //Called at the start of each level load.
    private void Start()
    {
        //Retrieving required components and values.
        sceneNumScriptRef = GameObject.Find("SceneMaintainanceObject").GetComponent<SceneMaintainance>();
        sceneNum = sceneNumScriptRef.GetSceneNum();
        chickyPropController = sceneNumScriptRef.gameObject.GetComponent<ChickyPropertiesController>();

        finances = transform.gameObject.GetComponent<FinanceController>();  //Retrieving the finance script component from the game object this one is also attached to.
        finances.gold = sceneNum * 40;      //Sets the value of gold the player starts with to the level number times 40.
        goalGold = sceneNum * 150;          //How much gold the player must acquire to progress onto the next level based on the level number times 150.

        //Sets the number of hutches and nests which should be allowed/instantiated in the level depending on what level the player is on. (This is capped as only so many will fit on the map).
        if (sceneNum < 5)
        {
            nestPoolSize = 3;
            hutchPoolSize = 2;
        }
        else if (sceneNum < 10)
        {
            nestPoolSize = 6;
            hutchPoolSize = 3;
        }
        else
        {
            nestPoolSize = 9;
            hutchPoolSize = 5;
        }

        numOfTips = 7;                              //How many tips are in the tutorialSteps list.
        tutorialSteps = new string[numOfTips];      //Sets the size of the list to correspond to the size specified in numOfTips.
        tutorialCounter = 0;                        //Set the counter back to 0 to iterate through the tutorialSteps list so strings can be assigned to it. (Unnecessary but good for readability).
        tutorialSteps[tutorialCounter] = "TIP: Drag your EGGs or chickens into a NEST to keep them warm.";
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
        tutorialCounter++;
        tutorialSteps[tutorialCounter] = "TIP: Don't put EGGs in your HUTCH. You can't get them out.";

        //Specifiying the size of the object pools. (How many clones of the appriopriate prefab should be instantiated and added to its object pool list). Dependant on the level/scene number.
        chickyPoolSize = sceneNum * 8;
        seedPoolSize = chickyPoolSize * 3;
        coinPoolSize = chickyPoolSize * 10;

        //Instantiating each of the single instance prefab clones. Parameter specified which prefab they should be clones of.
        LoadPrefabPool(UIPrefab);
        LoadPrefabPool(cameraSetupPrefab);
        LoadPrefabPool(lightingSetupPrefab);
        LoadPrefabPool(fencePrefab);
        LoadPrefabPool(groundPrefab);
        //Instantiating each of the multiple instance prefab clones. Parameters specify which prefab they should be clones of, how many clones should be instantiated, and which object pool list the clone object should be added to.
        //Make the pool half roosirs, and half chickys.
        LoadPrefabPool(chuckPrefab, (chickyPoolSize/2), "Roosir Egg", roosirEggPrefab, roosirPrefab);  
        LoadPrefabPool(chuckPrefab, (chickyPoolSize/2), "Chicky Egg", chickyEggPrefab, chickyPrefab);
        LoadPrefabPool(seedPrefab, seedPoolSize, levelSeed);
        LoadPrefabPool(coinPrefab, coinPoolSize, levelCoins);
        LoadPrefabPool(nestPrefab, nestPoolSize, levelNests);
        LoadPrefabPool(hutchPrefab, hutchPoolSize, levelHutches);
    }

    //Used to instantiate and add to the object pool list all of the chicken clones, and sets which type they are while assigning their model's base prefabs.
    private void LoadPrefabPool(GameObject targetPrefab, int poolSize, string eggType, GameObject typeEggModel, GameObject typeChuckModel)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefabClone = (GameObject)Instantiate(targetPrefab, inactivePosition, targetPrefab.transform.rotation); //Their position is initially set to the "inactivePosition" under the map, but maintain their prefab's specified rotation.

            GameObject newEggModel = Instantiate(typeEggModel, targetPrefab.transform.position, Quaternion.identity);          //Instantiates a clone of the egg model specified in the parameter.
            newEggModel.transform.parent = prefabClone.transform;                                                              //Sets the new model object as a child to the chicken object.
            GameObject newChuckModel = Instantiate(typeChuckModel, targetPrefab.transform.position, Quaternion.identity);      //Instantiates a clone of the adult model specified in the parameter.
            newChuckModel.transform.parent = prefabClone.transform;                          //Sets the new model object as a child to the chicken object.
            prefabClone.GetComponent<Chuck>().SetType(eggType, newEggModel, newChuckModel);  //Get its script component to set its type (Roosir or Chicky) and thusly which egg model it should instantiate as its model.

            levelChickens.Add(prefabClone, true);  //Add the newly instantiated object to its parameter specified list. Its boolean is set to true by default meaning this is an object which isn't yet active and can thusly be activated.
            prefabClone.SetActive(false);          //Initially set the object to inactive as these are objects that must be "purchased" (activated by their item button), before becoming active.
        }
    }

    //Used to instantiate and add to the object pool list all of the multiple instance prefabs.
    private void LoadPrefabPool(GameObject targetPrefab, int poolSize, Dictionary<GameObject, bool> targetList)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefabClone = (GameObject)Instantiate(targetPrefab, inactivePosition, targetPrefab.transform.rotation);   //Their position is initially set to the "inactivePosition" under the map, but maintain their prefab's specified rotation.
            prefabClone.SetActive(false);       //Initially set the object to inactive as these are objects that must be "purchased" (activated by their item button), before becoming active.
            targetList.Add(prefabClone, true);  //Add the newly instantiated object to its parameter specified list. Its boolean is set to true by default meaning this is an object which isn't yet active and can thusly be activated.
        }
    }

    //Used to instantiate all of the single instance prefabs.
    private void LoadPrefabPool(GameObject targetPrefab)
    {
            GameObject prefabClone = (GameObject)Instantiate(targetPrefab, targetPrefab.transform.position, targetPrefab.transform.rotation);   //The object's positions and rotations are set to their prefabs' default vectors.
            prefabClone.SetActive(true);    //These are active upon instantiation as they're just environmental/system parts of the game like the ground/camera/UI which are required for play.
    }

    //Used to deactivate all of the objects from play except the chickens. (Doesn't destroy them; just deactivates them). The parameters specify which object should be deactivated, and what list the object is from.
    public void DeactivateObject(GameObject whichObject, Dictionary<GameObject, bool> whichList)
    {
        whichObject.transform.position = inactivePosition;  //Sets its position back to the inactivePosition.
        whichObject.SetActive(false);                       //Deativates the object.
        whichList[whichObject] = true;                      //Sets its boolean in the list back to true to mean it's free for activation.
    }

    //Used to deactive the chickens. The same as the prior function, except it also displays a string to the tip textbox which is passed in as a parameter which explains why the chicken "died." 
    //^ It also checks if the level has failed, as if a chicken is deactivated, the player may be out of chickens and not have the gold to buy another, meaning they can't make more money to win the level.
    public void DeactivateObject(GameObject whichObject, Dictionary<GameObject, bool> whichList, string deathMessage)
    {
        whichObject.transform.position = inactivePosition;
        whichObject.SetActive(false);
        whichList[whichObject] = true;

        uiOutput.DisplayTip(deathMessage);
        uiOutput.tipTimer = 6.0f;
        CheckLevelFailed();
    }

    //Checks if no chickens are active, and if the player doen't have enough gold to buy another. If this is the case, the player won't have any chickens to activate/create more gold and so has no means of acquring the goal amount of gold to win.
    public bool CheckLevelFailed()
    {
        if (finances.gold < finances.GetItemPrice("Chicky Egg"))    //If the player has less gold than is required to buy another egg, there is a possibility they could have failed the level.
        {
            foreach (KeyValuePair<GameObject, bool> chicken in levelChickens)
            {
                if (chicken.Value == false)     //If any of the chickens from the pool list are active, the player still has a chance of winning so end the function check and return false to mean the player hasn't lost.
                {
                    return false;
                }
            }

            uiOutput.DisplayFailScreen();       //If none of the chickens are active in addition to the gold being to low, the player has failed so display the failure screen and end the function as returning true.
            return true;
        }

        return false;                           //If the player has enough money to buy another chicken, they can still win, so return false.
    }

    //If the player has the goal amount of gold or more, they've won the level , so display the success screen and end the function. Otherwise, return false and end the function.
    public bool CheckLevelSuccess()
    {
        if (finances.gold >= goalGold)
        {
            uiOutput.DisplaySuccessScreen();
            return true;
        }
        return false;
    }

    //Load the level again. If the boolean parameter is entered as true when called, increase the level number before loading the scene again to change all the variables dependant on the interger.
    public void LoadLevel(bool increaseLevel)
    {
        if (increaseLevel)
        {
            sceneNumScriptRef.IncreaseSceneNum();
        }
        SceneManager.LoadScene("Game");
    }
}
