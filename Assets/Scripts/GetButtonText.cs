using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetButtonText : MonoBehaviour
{
    private LevelController levelController;

    private string buttonName;
    private Text buttonText;

    // Start is called before the first frame update
    void Start()
    {
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        buttonText = GetComponent<Text>();

        buttonName = transform.parent.name.Replace(" Button", "");
        buttonText.text = buttonName + "               " + "[" + levelController.finances.GetItemPrice(buttonName) + "g]";
    }
}
