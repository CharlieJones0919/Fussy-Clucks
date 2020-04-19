using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonController : MonoBehaviour
{
    private LevelController levelController;

    private Button thisButton;
    public string typeName;
    public GameObject typeModel;

    private Vector3 startingNestPos = new Vector3(-18.5f, 0.5f, -10.5f);
    private Vector3 startingHutchPos = new Vector3(-18.5f, 3.5f, 10.0f);

    private GameObject activeObject;
    private Vector3 activePosition;
    private Vector3 setSeedPosition;

    private Dictionary<Vector3, bool> nestSpawnPositions = new Dictionary<Vector3, bool>();
    private Dictionary<Vector3, bool> hutchSpawnPositions = new Dictionary<Vector3, bool>();

    public void OnEnable()
    {
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        thisButton = GetComponent<Button>();
        levelController.finances.buttonRefs.Add(thisButton, true);

        for (int i = 0; i < levelController.nestPoolSize; i++)
        {
            nestSpawnPositions.Add(new Vector3(startingNestPos.x + (i * 5.0f), startingNestPos.y, startingNestPos.z), true);
        }
        for (int i = 0; i < levelController.hutchPoolSize; i++)
        {
            hutchSpawnPositions.Add(new Vector3(startingHutchPos.x + (i * 9.25f), startingHutchPos.y, startingHutchPos.z), true);
        }

        if (typeName != "Egg Nest")
        {
            thisButton.interactable = false;
        }
    }

    public void ActivateChicky()
    {
        foreach (KeyValuePair<GameObject, bool> chicken in levelController.levelChickens)
        {
            if (chicken.Value == true)
            {
                activeObject = chicken.Key;
                levelController.levelChickens[activeObject] = false;

                activeObject.transform.position = new Vector3(Random.insideUnitCircle.x, 4.0f + Random.insideUnitCircle.y, Random.insideUnitCircle.y);
                activeObject.GetComponent<Chuck>().SetType(typeName, typeModel);
                activeObject.GetComponent<Chuck>().Initialize();
                activeObject.SetActive(true);

                levelController.uiOutput.whichChicky = activeObject;
                levelController.finances.SpendGold(levelController.finances.GetItemPrice(typeName));
                break;
            }
        }

        if (levelController.tutorialSteps[2] != null)
        {
            levelController.uiOutput.DisplayTip(levelController.tutorialSteps[levelController.tutorialCounter]);
            levelController.tutorialSteps[levelController.tutorialCounter] = null;
            levelController.uiOutput.loopTips = true;
        }

        levelController.CheckLevelFailed();
    }

    public void ActivateNest()
    {
        ActivateSingle(levelController.levelNests, nestSpawnPositions);

        if (levelController.tutorialSteps[1] != null)
        {
            levelController.uiOutput.DisplayTip(levelController.tutorialSteps[levelController.tutorialCounter]);
            levelController.tutorialSteps[levelController.tutorialCounter] = null;
            levelController.tutorialCounter++;
        }
    }

    public void ActivateHutch()
    {
        ActivateSingle(levelController.levelHutches, hutchSpawnPositions);
    }

    private void ActivateSingle(Dictionary<GameObject, bool> objectList, Dictionary<Vector3, bool> positionList)
    {
        foreach (KeyValuePair<GameObject, bool> obj in objectList)
        {
            if (obj.Value == true)
            {
                activeObject = obj.Key;
                objectList[activeObject] = false;

                foreach (KeyValuePair<Vector3, bool> position in positionList)
                {
                    if (position.Value == true)
                    {
                        activePosition = position.Key;
                        positionList[activePosition] = false;
                        activeObject.transform.position = activePosition;
                        activeObject.SetActive(true);

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

                        foreach (KeyValuePair<GameObject, bool> seed in levelController.levelSeed)
                        {
                            if (seed.Value == false)
                            {
                                GameObject currentSeed = seed.Key;
                                if ((currentSeed.transform.position.x < (activePosition.x + 5.0f)) && (currentSeed.transform.position.x > (activePosition.x - 5.0f)) &&
                                    (currentSeed.transform.position.z < (activePosition.z + 5.0f)) && (currentSeed.transform.position.z > (activePosition.z - 5.0f)))
                                {
                                    currentSeed.transform.position = new Vector3(Random.insideUnitCircle.x * 10.0f, 0.5f, Random.insideUnitCircle.y * 10.0f);
                                }
                            }
                        }

                        levelController.finances.SpendGold(levelController.finances.GetItemPrice(typeName));
                        levelController.CheckLevelFailed();
                        break;
                    }
                }
                break;
            }
        }
    }

    public void ActivateSeed()
    {
        foreach (KeyValuePair<GameObject, bool> seed in levelController.levelSeed)
        {
            if (seed.Value == true)
            {
                activeObject = seed.Key;
                levelController.levelSeed[activeObject] = false;

                activeObject.transform.position = new Vector3(Random.insideUnitCircle.x * Random.Range(0.0f, 10.0f), 0.5f, Random.insideUnitCircle.y * Random.Range(0.0f, 10.0f));
                activeObject.SetActive(true);

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
