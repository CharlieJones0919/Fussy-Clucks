using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinanceController : MonoBehaviour
{
    public Dictionary<Button, bool> buttonRefs = new Dictionary<Button, bool>();
    private Button currentButton;
    public Text moneyTextbox;
    public int gold;

    public struct ItemForSale
    {
        public string itemName;
        public int itemPrice;
    };
    public ItemForSale[] itemList;

    private const int numOfItems = 5;
    private int counter;

    // Start is called before the first frame update
    private void Start()
    {
        itemList = new ItemForSale[numOfItems];
        counter = 0;
        itemList[counter].itemName = "Chicky Egg";
        itemList[counter].itemPrice = 5;

        counter++;
        itemList[counter].itemName = "Roosir Egg";
        itemList[counter].itemPrice = 15;

        counter++;
        itemList[counter].itemName = "Egg Nest";
        itemList[counter].itemPrice = 30;

        counter++;
        itemList[counter].itemName = "Hutch";
        itemList[counter].itemPrice = 100;

        counter++;
        itemList[counter].itemName = "Seed";
        itemList[counter].itemPrice = 10;
    }

    public int GetNumOfItems()
    {
        return numOfItems;
    }

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

    public bool CanAffordItem(string itemToCheck)
    {
        if (gold >= GetItemPrice(itemToCheck))
        {
            return true;
        }
        return false;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    public void SpendGold(int amount)
    {
        gold -= amount;
        UpdateUI();
    }

    public void UpdateUI()
    {
        moneyTextbox.text = (gold + "g");
        SetButtonAvailability();
    }

    public void SetButtonAvailability()
    {
        counter = 0;
        
        foreach (KeyValuePair<Button, bool> buttonRef in buttonRefs)
        {
            currentButton = buttonRef.Key;

            if ((itemList[counter].itemPrice > gold))
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

    public void DisableButton(string buttonName)
    {
        counter = 0;

        foreach (KeyValuePair<Button, bool> buttonRef in buttonRefs)
        {
            if ((itemList[counter].itemName == buttonName))
            {
                currentButton = buttonRef.Key;
                currentButton.interactable = false;
            }

            counter++;
        }
    }
}
