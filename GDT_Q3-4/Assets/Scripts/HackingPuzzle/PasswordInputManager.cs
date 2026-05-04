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
    
}
