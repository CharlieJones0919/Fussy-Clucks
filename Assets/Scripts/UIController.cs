using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is responsible for setting the text in the tip textbox, and outputting the property values of the last created or picked up chicken.
public class UIController : MonoBehaviour
{
    private LevelController levelController;    //Reference to the LevelController script to get universal game values and functions.
    public GameObject whichChicky;              //The chicken the UI should currently be outputting the property values of. This is set in the chicken's Chuck script when it's picked up, and in the ButtonController script when a chicken is activated in the scene.
    private int simplifiedValue;                //This is the what the UI will set the chicken's property text values to instead of their real float values. 

    private GameObject activeCoin;              //The current coin from the list of coins in the coin object pool being operated on.
    private int coinDropValue;                  //How much gold the activeCoin was assigned when dropped based on the chicken's specs.

    //References to the winning/losing screen game objects, so they can be set to active as appropriate. 
    public GameObject failLevelPanel;           
    public GameObject successLevelPanel;

    //UI components used to output the current string tip messages and the level's goal. 
    public Text goalText;
    public Text tipText;

    public bool loopTips;           //Boolean to dictate if the tips should start looping. (Will be set to true after a "Egg Nest" and "Chicky Egg" have been bought).
    public float timeToStayOnTip;   //How long each tip should remain in the textbox when the script is looping through the list of string tip messages.
    public float tipTimer;          //How long the current tip has been displayed for. (So it can swap to the next one in the list when this value gets to be higher than or equal to the timeToStayOnTip float).

    //The UI elements used to output data amount the current chicken.
    public InputField nameInputBox; //Input textbox used to give the chicken a name.
    public Text tempText;       //Outputs the chicken's temperature value.
    public Text attentionText;  //Outputs the chicken's attention value.
    public Text hungryText;     //Outputs the chicken's hunger value.
    public Text sleepText;      //Outputs the chicken's rest value.

    //The image UI components used to show the chicken's propeties in a visual sprite form.
    public Image tempIcon;      
    public Image attentionIcon;
    public Image hungryIcon;
    public Image sleepIcon;

    //The sprite images set into the UI image components to show if the property values are too high or low visually
    public Sprite attention;
    public Sprite attentionLow;
    public Sprite temp;
    public Sprite tempCold;
    public Sprite tempHot;
    public Sprite hunger;
    public Sprite hungerLow;
    public Sprite sleep;
    public Sprite sleepLow;

    //When the UI is enabled as active this function is executed.
    private void OnEnable()
    {
        //Retrieving required component references.
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        levelController.finances.moneyText = GameObject.Find("MoneyOutput").GetComponent<Text>();

        levelController.finances.moneyText.text = levelController.finances.gold.ToString() + "G";               //Setting the moneyText textbox text to the initial value of gold as set in LevelController.
        goalText.text = "LEVEL " + levelController.sceneNum + " GOAL: Make " + levelController.goalGold + "G."; //Sets the goalText value to the current goalGold amount.

        levelController.uiOutput = this;    //Sets the uiOutput script reference in LevelController script, to this script, to ensure that the object is only assigned after the UI prefab has been instantiated.
        whichChicky = null;                 //Initialise whichChicky to null when the script is created.

        //Deactivate the winning/failing panels on start.
        failLevelPanel.SetActive(false);
        successLevelPanel.SetActive(false);

        //Initialise the tip message variables.
        loopTips = false;
        tipTimer = 0.0f;
        timeToStayOnTip = 10.0f;

        //Display the first tip message as the player requires at least 1 nest to win each level.
        DisplayTip("TIP: Buy a NEST to keep your EGG and CHICKYS warm. (Don't let them get too hot)."); 
    }

    //Sets the name textbox to the current chicken's name as passed in as a parameter by the chicken's Chuck script.
    public void SetNameOutput(string name)
    {
        nameInputBox.text = name;
    }

    //Sets the text component parameter to the float property parameter. Called by the current chicken's Chuck script to set the UI output to the chicken's property values.
    public void SetTextOutput(Text whichText, float propertyValue)
    {
        if (propertyValue >= 1.0f)     //If the input value is more than or equal to 1.0f, set the textbox to the float as an integer.
        {
            simplifiedValue = (int)propertyValue;
        }  
        else if ((propertyValue < 1.0f) && (propertyValue > 0.0f))      //If the input value is less than 1.0f but more than 0.0f, keep it as 1. (Round up).
        {
            simplifiedValue = 1;
        }

        if (whichText == tempText)  //If the UI output field being updated is the temperature textbox, end the text with °C, and otherwise end it with % to show what the metric is.
        {
            whichText.text = simplifiedValue + "°C";
        }
        else
        {
            whichText.text = simplifiedValue + "%";
        }
    }

    //Called by the active chicken in its property check functions, to set the relevant Image component to the appropriate sprite. Parameters set which image should be changed and which sprite it should be set to. 
    public void SetPropetyIcon(Image whichIcon, Sprite whichSprite)
    {
        whichIcon.sprite = whichSprite;
    }

    //Called when text is input into the name textbox, and sets the input text as being the active chicken's name if there is an allocated active chicken set to whichChicky.
    public void SetChickyName()
    {
        if (whichChicky != null)
        {
            whichChicky.GetComponent<Chuck>().chickyProps.name = nameInputBox.text;
        }
    }

    //On each update, run the TipsLoop() function to show the appropriate tip message, and check if the player has picked up any of the dropped coins.
    private void Update()
    {
        TipsLoop();
        CheckCoinPickUp();
    }

    //Displays the parameter string to the tipText textbox.
    public void DisplayTip(string message)
    {
        tipText.text = message;
    }

    //If the timer float is less than the time each message should be set as the text in the tip textbox, increase the timer. Once the timer gets to this value, set the timer back to 0 and set the textbox to the new message in the list.
    private void TipsLoop()
    {
        if (tipTimer < timeToStayOnTip)
        {
            tipTimer += Time.deltaTime;
        }
        else
        {
            tipTimer = 0;

            if (loopTips)
            {
                GetNextTip();
            }
        }
    }

    //Sets the tips textbox's value to the next string message in the tips list.
    private void GetNextTip()
    {
        if (levelController.tutorialCounter < (levelController.numOfTips - 1))    //If the counter hasn't gotten to the end of the list, increase the counter variable to get the next string.
        {
            levelController.tutorialCounter++;  
        }
        else    //If at the end of the list, set the counter back to 0.
        {
            levelController.tutorialCounter = 0;
        }

        tipText.text = levelController.tutorialSteps[levelController.tutorialCounter];  //Sets the text to this string message.
    }

    //Checks if the player has tapped any of the coins active on the map, and if they have, add the pile's gold worth to finance's gold value.
    public void CheckCoinPickUp()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //A vector of the player's touching position from the screen position into the world position.
            touchPosition.y = 0.0f;     //The touch Y-axis is set to 0 (the Y position of the coins), so it's only checking on the position level of the coins.

            foreach (KeyValuePair<GameObject, bool> coin in levelController.levelCoins)
            {
                if (coin.Value == false)
                {
                    activeCoin = coin.Key;  //Sets the active coin to this coin in the list.

                    if (((touchPosition.x > (activeCoin.transform.position.x - 2.0f)) && (touchPosition.x < (activeCoin.transform.position.x + 2.0f)))      //Checks that the tap position is within the range of the coin drop on the X and Y axis.
                         && ((touchPosition.z > (activeCoin.transform.position.z - 2.0f)) && (touchPosition.z < (activeCoin.transform.position.z + 2.0f))))
                    {
                        foreach (KeyValuePair<GameObject, int> coinValue in levelController.coinValue)
                        {
                            if (coinValue.Key == coin.Key)      //Finds the same game object in the coinValue list and sets coinValue to its assigned integer value.
                            {
                                coinDropValue = coinValue.Value;
                                levelController.coinValue.Remove(coinValue.Key);    //Remove this coin value from the list.
                                break;
                            }
                        }

                        levelController.DeactivateObject(activeCoin, levelController.levelCoins);    //Deactivate the coin from the active scene.
                        levelController.finances.AddGold(coinDropValue);                             //Add the gold value of the coin pile to the player's gold value.
                        levelController.CheckLevelSuccess();                                         //Check if the gold value is at the goal gold value yet.
                        break;
                    }
                }
            }
        }
    }

    //Just activates the fail screen panel.
    public void DisplayFailScreen()
    {
        failLevelPanel.SetActive(true);
    }

    //Just activates the success screen panel.
    public void DisplaySuccessScreen()
    {
        successLevelPanel.SetActive(true);
    }

    //Reloads the scene as it was.
    public void ReloadLevel()
    {
        levelController.LoadLevel(false);
    }

    //Increaments the level number then reloads the scene to get the conditions of the next level iteration.
    public void LoadNextLevel()
    {
        levelController.LoadLevel(true);
    }
}
