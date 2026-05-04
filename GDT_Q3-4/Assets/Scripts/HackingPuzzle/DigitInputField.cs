using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DigitInputField : MonoBehaviour
{   

    private int correctInput;

    public bool hasInput = false;
    public TextMeshProUGUI inputText;

    void Start()
    {

    }


    public void SetupInputField(int correctInput)
    {
        this.correctInput = correctInput;
    }

    public void RespondToInput(int givenInput)
    {
        inputText.text = givenInput.ToString();
        hasInput = true;
    }
}
