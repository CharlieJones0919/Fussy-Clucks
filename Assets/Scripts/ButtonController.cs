using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonController : MonoBehaviour
{
    private LevelLoader levelLoader;

    private Button thisButton;
    public string typeName;
    public GameObject typeModel;

    private GameObject activeObject;
    private Vector3 activePosition;

    private Vector3 activeChickyPos;
    private Vector3 activeSeedPos;

    private Dictionary<Vector3, bool> nestSpawnPositions = new Dictionary<Vector3, bool>();
    private Dictionary<Vector3, bool> hutchSpawnPositions = new Dictionary<Vector3, bool>();

    public void Start()
    {
        levelLoader = GameObject.Find("LevelController").GetComponent<LevelLoader>();
        thisButton = GetComponent<Button>();

        nestSpawnPositions.Add(new Vector3(-21.0f, 0.5f, -12.5f), true);
        nestSpawnPositions.Add(new Vector3(-21.0f, 0.5f, -8.5f), true);

        hutchSpawnPositions.Add(new Vector3(-18.5f, 3.5f, 10.0f), true);
        hutchSpawnPositions.Add(new Vector3(-11.0f, 3.5f, 10.0f), true);

        if (!levelLoader.finances.CanAffordItem(typeName))
        {
            thisButton.interactable = false;
        }
        levelLoader.finances.moneyTextbox = GameObject.Find("MoneyOutput").GetComponent<Text>();
        levelLoader.finances.buttonRefs.Add(thisButton, true);
        levelLoader.finances.UpdateUI();
    }

    public void ActivateChicky()
    {
        if (levelLoader.finances.CanAffordItem(typeName))
        {
            foreach (KeyValuePair<GameObject, bool> chicken in levelLoader.levelChickens)
            {
                if (chicken.Value == true)
                {
                    activeObject = chicken.Key;
                    levelLoader.levelChickens[activeObject] = false;

                    activeChickyPos = new Vector3(Random.insideUnitCircle.x, 4.0f + Random.insideUnitCircle.y, Random.insideUnitCircle.y);
                    activeObject.transform.position = activeChickyPos;
                    activeObject.GetComponent<Chuck>().SetType(typeName, typeModel);
                    activeObject.SetActive(true);
                    break;
                }
            }

            levelLoader.finances.SpendGold(levelLoader.finances.GetItemPrice(typeName));
        }
    }

    public void ActivateNest()
    {
        if (levelLoader.finances.CanAffordItem(typeName))
        {
            ActivateNestOrHutch(levelLoader.levelNests, nestSpawnPositions);
            levelLoader.finances.SpendGold(levelLoader.finances.GetItemPrice(typeName));
        }
    }

    public void ActivateHutch()
    {
        if (levelLoader.finances.CanAffordItem(typeName))
        {
            ActivateNestOrHutch(levelLoader.levelHutches, hutchSpawnPositions);
            levelLoader.finances.SpendGold(levelLoader.finances.GetItemPrice(typeName));
        }
    }

    private void ActivateNestOrHutch(Dictionary<GameObject, bool> objectList, Dictionary<Vector3, bool> positionList)
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

                        foreach (KeyValuePair<GameObject, bool> chicken in levelLoader.levelChickens)
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
                        break;
                    }
                }
                break;
            }
        }
    }
}