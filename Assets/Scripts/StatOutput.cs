using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatOutput : MonoBehaviour
{
    private LevelController levelController;
    public GameObject whichChicky;
    public GameObject lastChicky;
    private int simplifiedValue;

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

    private void Start()
    {
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        levelController.uiOutput = this;
        whichChicky = null;
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
}
