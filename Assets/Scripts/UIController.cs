using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private LevelController levelController;
    public GameObject whichChicky;
    private int simplifiedValue;

    public GameObject failLevelPanel;
    public GameObject successLevelPanel;
    public GameObject successLevelEndPanel;

    public Text goalText;
    public Text tipText;
    public bool loopTips;
    private float timeToStayOnTip;
    private float tipTimer;

    public InputField nameInputBox;
    public Text tempText;
    public Text attentionText;
    public Text hungryText;
    public Text sleepText;

    public Image tempIcon;
    public Image attentionIcon;
    public Image hungryIcon;
    public Image sleepIcon;

    public Sprite attention;
    public Sprite attentionLow;
    public Sprite temp;
    public Sprite tempCold;
    public Sprite tempHot;
    public Sprite hunger;
    public Sprite hungerLow;
    public Sprite sleep;
    public Sprite sleepLow;

    private void OnEnable()
    {
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        levelController.finances.moneyTextbox = GameObject.Find("MoneyOutput").GetComponent<Text>();
        levelController.finances.moneyTextbox.text = levelController.finances.gold.ToString() + "G";

        levelController.uiOutput = this;
        whichChicky = null;
        failLevelPanel.SetActive(false);
        successLevelPanel.SetActive(false);
        successLevelEndPanel.SetActive(false);

        DisplayTip(levelController.tutorialSteps[levelController.tutorialCounter]);
        tipTimer = 0.0f;
        timeToStayOnTip = 20.0f;

        if (levelController.tutorialSteps[0] != null)
        {
            loopTips = false;
            levelController.tutorialSteps[levelController.tutorialCounter] = null;
            levelController.tutorialCounter++;
        }

        goalText.text = "GOAL: Make " + levelController.goalGold + "G";
    }

    public void SetNameOutput(string name)
    {
        nameInputBox.text = name;
    }

    public void SetTextOutput(Text whichText, float propertyValue)
    {
        if (propertyValue >= 1)
        {
            simplifiedValue = (int)propertyValue;
        }
        else if ((propertyValue < 1.0f) && (propertyValue > 0.0f))
        {
            simplifiedValue = 1;
        }

        if (whichText == tempText)
        {
            whichText.text = simplifiedValue + "°C";
        }
        else
        {
            whichText.text = simplifiedValue + "%";
        }

        if (loopTips)
        {
            TipsLoop();
        }
    }

    public void SetPropetyIcon(Image whichIcon, Sprite whichSprite)
    {
        whichIcon.sprite = whichSprite;
    }

    public void SetChickyName()
    {
        if (whichChicky != null)
        {
            whichChicky.GetComponent<Chuck>().chickyProps.name = nameInputBox.text;
        }
    }

    public void DisplayTip(string message)
    {
        tipText.text = message;
    }

    private void TipsLoop()
    {
        if (tipTimer < timeToStayOnTip)
        {
            tipTimer += Time.deltaTime;
        }
        else
        {
            tipTimer = 0;
            GetNextTip();
        }
    }

    private void GetNextTip()
    {
        levelController.tutorialCounter++;

        if (levelController.tutorialCounter < levelController.numOfTips)
        {
            if (levelController.tutorialSteps[levelController.tutorialCounter] != null)
            {
                tipText.text = levelController.tutorialSteps[levelController.tutorialCounter];
            }
            else
            {
                levelController.tutorialCounter++;
                GetNextTip();
            }
        }
        else
        {
            levelController.tutorialCounter = 0;
        }
    }

    public void DisplayFailScreen()
    {
        failLevelPanel.SetActive(true);
    }

    public void DisplaySuccessScreen()
    {
        if (levelController.sceneNum != levelController.numOfLevels)
        {
            successLevelPanel.SetActive(true);
        }
        else
        {
            successLevelEndPanel.SetActive(true);
        }
    }

    public void ReloadLevel()
    {
        levelController.DestroyLevel("Level " + levelController.sceneNum);
    }

    public void LoadNextLevel()
    {
        levelController.DestroyLevel("Level " + (levelController.sceneNum + 1));
    }
}
