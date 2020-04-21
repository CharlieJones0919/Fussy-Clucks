using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Allocated to each button's text object, to set its text value to the name of the button object, and it's displayed text to the current price of the item in the FinanceController script.
public class GetButtonText : MonoBehaviour
{
    private FinanceController financeController;    //Used to store a reference to the FinanceController script in the LevelController object.
    private string buttonName;  //Name of the parenting button game object.
    private Text buttonText;    //Text component of the object.

    //Called on start when the script is first activated along with the button.
    void OnEnable()
    {
        financeController = GameObject.Find("LevelController").GetComponent<FinanceController>();   //Finding the LevelController object and its FinanceController script to get the item's price.
        buttonText = GetComponent<Text>(); //Retrieving the button's text component.

        buttonName = transform.parent.name.Replace(" Button", "");  //Gets the name of the parent button object but removes the "Button" part of the object name to just get the item name.
        buttonText.text = buttonName + "               " + "[" + financeController.GetItemPrice(buttonName) + "G]"; //Set's the button's text to the item name in the button object's name, adds a space, the price of that item from the FinanceController script, and a "G" for presentation.
    }
}
