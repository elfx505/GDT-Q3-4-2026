using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DigitInputField : MonoBehaviour
{   

    public int correctInput;
    public int currentInput = -1; // Value that is impossible to be present in the correct combination

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
        currentInput = givenInput;
        inputText.text = givenInput.ToString();
        hasInput = true;
    }

    public void ClearInput()
    {
        hasInput = false;
        inputText.text = "_";
        currentInput = -1; // Set current input to the original value
    }

    public bool CheckInputValidity()
    {
        return currentInput == correctInput;
    }
}
