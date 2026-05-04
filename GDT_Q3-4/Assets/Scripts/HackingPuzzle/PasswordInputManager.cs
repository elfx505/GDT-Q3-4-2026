using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PasswordInputManager : MonoBehaviour
{
    
    [Header("UI References")]
    [Tooltip("The parent object with the Layout Group")]
    public Transform contentPanel;
    public Transform numpadPanel;

    private Button[] numpadButtons;
    [SerializeField] private Button delButton;
    [SerializeField] private Button enterButton;
    private List<DigitInputField> digitInputFields = new List<DigitInputField>();
    public GameObject digitInputFieldPrefab;

    [SerializeField] private int[] correctCombination;
    
    void Start()
    {
        
        // Hardcoded for now
        correctCombination = new int[] {1, 4, 3, 2, 4, 0, 2, 1, 4, 7, 4, 3, 3, 1, 5};
        
        CreateInputField();

        // Populate numpadButtons list
        numpadButtons = numpadPanel.GetComponentsInChildren<Button>();

        // Set up event listeners
        foreach (Button button in numpadButtons)
        {   
            int inputValue = button.GetComponent<NumpadButton>().buttonValue; // Get the button value from the attached scripts

            button.onClick.RemoveAllListeners(); // Clear any pre-existing listeners
            button.onClick.AddListener(() => RegisterInput(inputValue));
        }

        // Assign Remaining Button listeners

        // Backspace Button
        delButton.onClick.RemoveAllListeners(); // Clear any pre-existing listeners
        delButton.onClick.AddListener(() => DeleteLastDigitInput()); 

        // Enter Button
        enterButton.onClick.RemoveAllListeners(); // Clear any pre-existing listeners
        enterButton.onClick.AddListener(() => CheckExistingInput());

    }

    private void RegisterInput(int input)
    {
        foreach (DigitInputField inputField in digitInputFields)
        {
            if (inputField.hasInput) continue;

            inputField.RespondToInput(input);

            break;
        }
    }

    private void CreateInputField()
    {
        foreach (int digit in correctCombination)
        {
            GameObject digitInputField = Instantiate(digitInputFieldPrefab, contentPanel);

            DigitInputField digitInputFieldScript = digitInputField.GetComponent<DigitInputField>();

            digitInputFieldScript.SetupInputField(digit);

            digitInputFields.Add(digitInputFieldScript);
        }
    }

    private void DeleteLastDigitInput()
    {   
        // Loop backwards starting from the end of the list
        for (int i = digitInputFields.Count - 1; i >= 0; i--)
        {
            if (digitInputFields[i].hasInput)
            {
                digitInputFields[i].ClearInput(); // Clear last filled input filled and stop looking
                break; 
            }
        }
    }

    private void CheckExistingInput()
    {   

        // Check if the last input field has an input, i.e. if the attempted password is fully entered
        if (!digitInputFields[digitInputFields.Count - 1].hasInput)
        {
            // Clear Current Input
            foreach (DigitInputField inputField in digitInputFields) inputField.ClearInput();
            return;
        }
        
        foreach (DigitInputField inputField in digitInputFields)
        {
            if (!inputField.CheckInputValidity())
            {
                // Clear Current Input
                foreach (DigitInputField field in digitInputFields) field.ClearInput();
                break;
            }

            // Reachable only if all Input is Valid
            Debug.Log("Correct Password Entered! Opening Shutters..."); // Temp
        }
    }
    
}
