using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Responsible for updating the value of the gold, the item's prices, and item buttons' interactivity depending of if the player has enough gold to buy the item. 
public class FinanceController : MonoBehaviour
{
    public Dictionary<Button, bool> buttonRefs = new Dictionary<Button, bool>();    //A list of all the item buttons in the UI, so they can be made interactable depending on their price and how much gold is in the gold int.
    private Button currentButton;   //Which button from the list operations should be applied to.
    public Text moneyText;          //The text that the gold amount is outputted into in the UI.
    public int gold;                //The player's gold. (Chickens coins which when picked up increase it, and it's decreased when an item is bought).

    //Structure for the items sold so their name can be correlated to a price.
    public struct ItemForSale       
    {
        public string itemName;
        public int itemPrice;
    };
    public ItemForSale[] itemList;  //List of all the items sold.

    public const int numOfItems = 5;   //Current number of items sold.
    private int counter;               //Used to iterate through the item list without having to maintain what number in the list the item is.

    //Called on start when the script is first activated.
    private void OnEnable()
    {
        itemList = new ItemForSale[numOfItems];     //Sets the list size to the current number of items.
        counter = 0;                                //Sets the counter to 0. (Unnecessary but good for readability).

        //The following add the item names to the list and set them to a price. After the first, the counter is incremented to move into the next storage allocation in the list.
        itemList[counter].itemName = "Chicky Egg";  
        itemList[counter].itemPrice = 5;

        counter++;
        itemList[counter].itemName = "Roosir Egg";
        itemList[counter].itemPrice = 20;

        counter++;
        itemList[counter].itemName = "Egg Nest";
        itemList[counter].itemPrice = 30;

        counter++;
        itemList[counter].itemName = "Hutch";
        itemList[counter].itemPrice = 60;

        counter++;
        itemList[counter].itemName = "Seed";
        itemList[counter].itemPrice = 12;
    }

    //Takes the parameter of an item name as a string, and then iterates through the list to find an item with the same name, to return that item's price..
    public int GetItemPrice(string item)
    {
        for(int i = 0; i < numOfItems; i++)
        {
            if (itemList[i].itemName == item)
            {
                return itemList[i].itemPrice;
            }
        }

        return 0;
    }

    //Called whenever the player picks up one of the chicken's coin drops. Add the value of the coin drop as entered via the parameter to the gold amount and updates the moneyText value to reflect the gold's new value;
    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    //Called when the player presses one of the buttons to buy an item if one is available. Subtracts the item's price from gold and updates the moneyText UI to reflect the new value.
    public void SpendGold(int amount)
    {
        gold -= amount;
        UpdateUI();
    }

    //Update's the moneyText UI component to output the current value in the gold int, and sets all the item's button's interactivity based on the new gold value.
    public void UpdateUI()
    {
        moneyText.text = (gold + "G");
        SetButtonAvailability();
    }

    //For every item button, if the corresponding item's price is more than the value in the gold int, set the button as non-interactable so it the player can't buy/press it. Otherwise, make the button interactable.
    public void SetButtonAvailability()
    {
        counter = 0;
        
        foreach (KeyValuePair<Button, bool> buttonRef in buttonRefs)
        {
            currentButton = buttonRef.Key;

            if (itemList[counter].itemPrice > gold)
            {
                currentButton.interactable = false;
            }
            else
            {
                currentButton.interactable = true;
            }

            counter++;
        }
    }
}
