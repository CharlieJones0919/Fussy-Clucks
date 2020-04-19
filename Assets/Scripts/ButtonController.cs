using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;  //Specifiying that the Random call used should be the one from the UnityEngine library. Required to avoid abiguity. 

public class ButtonController : MonoBehaviour
{
    private LevelController levelController;    //Reference to the LevelController script to get universal game values and functions.

    private Button thisButton;      //The button that operations should be applied to.
    public string typeName;         //Which item the button should activate on the map. (e.g A "Chicky Egg" or a "Egg Nest").
    public GameObject typeModel;    //Used for the egg buttons; contains the egg model mesh and material to be instantiated/cloned for the new egg.

    private Vector3 startingNestPos = new Vector3(-18.5f, 0.5f, -10.5f);    //Position for the first activated nest to be transformed to so the other positions can be extrapolated from it in a for loop.
    private Vector3 startingHutchPos = new Vector3(-18.5f, 3.5f, 10.0f);    //Same as above but for the hutches.

    private GameObject activeObject;    //Which of the objects in the relevant object pool to be transformed into its active position and activated.
    private Vector3 activePosition;     //The position the activated object should be transformed to.

    private Dictionary<Vector3, bool> nestSpawnPositions = new Dictionary<Vector3, bool>();     //A list of positions for the nests to be placed on, and a boolean for if they've been used yet.
    private Dictionary<Vector3, bool> hutchSpawnPositions = new Dictionary<Vector3, bool>();    //Same as above but for the hutch's positions.

    /////-----/////-----/////-----/////
    ///When the script is activated.///
    /////-----/////-----/////-----/////
    public void OnEnable()
    {
        //Get the relevant components on start.
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();   
        thisButton = GetComponent<Button>();
        levelController.finances.buttonRefs.Add(thisButton, true);  //Add the button this copy of the ButtonController script is attached to into the FinanceController's button references list for them to be made interactive dependant on the value of the "gold" float.

        //Allocates all the positions the nests/hutches can be activated onto, depending on the number possible to activate (pool size).
        for (int i = 0; i < levelController.nestPoolSize; i++)    
        {
            nestSpawnPositions.Add(new Vector3(startingNestPos.x + (i * 5.0f), startingNestPos.y, startingNestPos.z), true);
        }
        for (int i = 0; i < levelController.hutchPoolSize; i++)
        {
            hutchSpawnPositions.Add(new Vector3(startingHutchPos.x + (i * 9.25f), startingHutchPos.y, startingHutchPos.z), true);
        }

        //On start, deactivate all of the buttons except the "Egg Nest", as any eggs bought would die if a nest isn't bought first.
        if (typeName != "Egg Nest")
        {
            thisButton.interactable = false;
        }
    }

    //////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----//////
    ///Called when the "Chicky Egg" or "Roosir Egg" buttons are pressed to activate one of the Chicky prefabs in the object pool for such.///
    //////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----//////
    public void ActivateChicky()
    {
        //For every object in the chicken's object pool list, if any are "true" (haven't been activated yet), activate it.
        foreach (KeyValuePair<GameObject, bool> chicken in levelController.levelChickens)
        {
            if (chicken.Value == true)
            {
                activeObject = chicken.Key; //Sets the activeObject object to be a reference to the unactivated chicky prefab clone so it can be modified. 
                levelController.levelChickens[activeObject] = false;    //Set the object to false to indicate the object is now activated from the pool and in play.

                activeObject.transform.position = new Vector3(Random.insideUnitCircle.x, 4.0f + Random.insideUnitCircle.y, Random.insideUnitCircle.y);  //Move its position to a random point in the centre of the map.
                activeObject.GetComponent<Chuck>().SetType(typeName, typeModel);    //Get its script component to set its type (Roosir or Chicky) and thusly which egg model it should instantiate as its model.
                activeObject.SetActive(true);   //Activate the game object.

                levelController.uiOutput.whichChicky = activeObject;    //Sets the UI's object reference to this newly activated chicken so it'll output its property values.
                levelController.finances.SpendGold(levelController.finances.GetItemPrice(typeName));    //Get the price of the item and subtract it from the gold value.
                break;  //Once a non-active chicken has been activated, break the loop through the pool list doesn't need to be continued.
            }
        }

        //If the tip message about needing to buy an egg to go in the nest hasn't been displayed yet, display it then set it to null to dictate that it has now. Then set the boolean "loopTips" as true so the tips will just start looping on a timer.
        if (levelController.tutorialSteps[2] != null)
        {
            levelController.uiOutput.DisplayTip(levelController.tutorialSteps[levelController.tutorialCounter]);
            levelController.tutorialSteps[levelController.tutorialCounter] = null;
            levelController.uiOutput.loopTips = true;
        }
    }

    //////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////
    ///Called when the "Egg Nest" button is pressed to activate one of the Nest prefabs in the object pool for such.///
    //////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////
    public void ActivateNest()
    {
        ActivateSingle(levelController.levelNests, nestSpawnPositions);

        //If the tip message about needing to buy a nest hasn't been displayed yet, display it then set it to null to dictate that it has now.
        if (levelController.tutorialSteps[1] != null)
        {
            levelController.uiOutput.DisplayTip(levelController.tutorialSteps[levelController.tutorialCounter]);
            levelController.tutorialSteps[levelController.tutorialCounter] = null;
            levelController.tutorialCounter++;
        }
    }

    ////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----////
    ///Called when the "Hutch" button is pressed to activate one of the Hutch prefabs in the object pool for such.///
    ////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----////
    public void ActivateHutch()
    {
        ActivateSingle(levelController.levelHutches, hutchSpawnPositions);
    }

    /////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////
    //////Function used by the nest and hutch buttons to activate an object from their respective pools if any are left un-activated.//////
    /////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////
    private void ActivateSingle(Dictionary<GameObject, bool> objectList, Dictionary<Vector3, bool> positionList)
    {
        //The same as the ActivateChicky() loop; for finding an unactivated object in the respective list.
        foreach (KeyValuePair<GameObject, bool> obj in objectList)
        {
            if (obj.Value == true)
            {
                activeObject = obj.Key;
                objectList[activeObject] = false;

                //Find a position in the list of postions that isn't used and set it to "false" to define it as used. Then transform and activate the object from the prior loop to this position.
                foreach (KeyValuePair<Vector3, bool> position in positionList)
                {
                    if (position.Value == true)
                    {
                        activePosition = position.Key;
                        positionList[activePosition] = false;
                        activeObject.transform.position = activePosition;
                        activeObject.SetActive(true);

                        //If any of the active chickens were in the range of the newly activated object, move them back to the centre of the map.
                        foreach (KeyValuePair<GameObject, bool> chicken in levelController.levelChickens)
                        {
                            if (chicken.Value == false)
                            {
                                GameObject currentChicky = chicken.Key;
                                if ((currentChicky.transform.position.x < (activePosition.x + 5.0f)) && (currentChicky.transform.position.x > (activePosition.x - 5.0f)) &&
                                    (currentChicky.transform.position.z < (activePosition.z + 5.0f)) && (currentChicky.transform.position.z > (activePosition.z - 5.0f)))
                                {
                                    currentChicky.transform.position = new Vector3(Random.insideUnitCircle.x, 4.0f + Random.insideUnitCircle.y, Random.insideUnitCircle.y);
                                }
                            }
                        }

                        levelController.finances.SpendGold(levelController.finances.GetItemPrice(typeName));    //Get the price of the item and subtract it from the gold value.
                        levelController.CheckLevelFailed();     //After the purchase, check if the level has failed as the player may now not have a chicken or enough money to buy another one, meaning they can't complete the level as they can't make anymore money.
                        break;      //Once a non-active object has been activated, break the loop through the pool list doesn't need to be continued.
                    }
                }
                break; 
            }
        }
    }

    ///-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----///
    ///Called when the "Seed" button is pressed to activate one of the Seed prefabs in the object pool for such.///
    ///-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----/////-----///
    public void ActivateSeed()
    {
        //Same as the prior 2 activation loops except the position is randomised on the X and Z axis.
        foreach (KeyValuePair<GameObject, bool> seed in levelController.levelSeed)
        {
            if (seed.Value == true)
            {
                activeObject = seed.Key;
                levelController.levelSeed[activeObject] = false;

                activeObject.transform.position = new Vector3(Random.insideUnitCircle.x * Random.Range(0.0f, 10.0f), 0.5f, Random.insideUnitCircle.y * Random.Range(0.0f, 10.0f));
                activeObject.SetActive(true);

                //Sets the chicken's boolean for seedAvailable as true allong with the seed's position so the chickens can go to it.
                foreach (KeyValuePair<GameObject, bool> chicky in levelController.levelChickens)
                {
                    if (chicky.Value == false)
                    {
                        GameObject currentChicky = chicky.Key;
                        currentChicky.GetComponent<Chuck>().SetSeedAsAvailable(activeObject.transform.position);
                    }
                }

                levelController.finances.SpendGold(levelController.finances.GetItemPrice(typeName));
                levelController.CheckLevelFailed();
                break;
            }
        }
    }
}