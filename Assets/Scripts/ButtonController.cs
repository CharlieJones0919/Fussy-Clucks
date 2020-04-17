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

    private GameObject activeObject;
    private Vector3 activePosition;
    private Vector3 setSeedPosition;

    private Dictionary<Vector3, bool> nestSpawnPositions = new Dictionary<Vector3, bool>();
    private Dictionary<Vector3, bool> hutchSpawnPositions = new Dictionary<Vector3, bool>();

    public void Start()
    {
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        levelController.finances.moneyTextbox = GameObject.Find("MoneyOutput").GetComponent<Text>();

        thisButton = GetComponent<Button>();
        levelController.finances.buttonRefs.Add(thisButton, true);
   
        nestSpawnPositions.Add(new Vector3(-21.0f, 0.5f, -12.5f), true);
        nestSpawnPositions.Add(new Vector3(-21.0f, 0.5f, -8.5f), true);

        hutchSpawnPositions.Add(new Vector3(-18.5f, 3.5f, 10.0f), true);
        hutchSpawnPositions.Add(new Vector3(-11.0f, 3.5f, 10.0f), true);

        if (levelController.beginningOfGame)
        {
            if (typeName != "Egg Nest")
            {
                thisButton.interactable = false;
            }
        }
        else
        {
            levelController.finances.UpdateUI();
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
                activeObject.SetActive(true);

                levelController.finances.SpendGold(levelController.finances.GetItemPrice(typeName));
                break;
            }
        }
    }

    public void ActivateNest()
    {
        ActivateSingle(levelController.levelNests, nestSpawnPositions);

        if (levelController.beginningOfGame)
        {
            levelController.beginningOfGame = false;
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
                break;
            }
        }
    }
}
