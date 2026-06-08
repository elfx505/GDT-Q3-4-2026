using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumpadLock : MonoBehaviour
{


    private int[] correctCombination;
    private List<int> currentCombination;
    [SerializeField] private int maxCombinationSize = 4;
    [SerializeField] TextMeshPro text;

    
    void Awake()
    {
        
        correctCombination = new int[] {6, 0, 3, 2};

        currentCombination = new List<int>();

        if (text == null) Debug.LogWarning("[NumpadLock] Text object not assigned!");
    }

    public void RegisterInput(int value)
    {
        if (currentCombination.Count >= 4) return;

        currentCombination.Add(value);
        UpdateText();
    }

    public void ClearInput()
    {
        if (currentCombination.Count <= 0) return;

        currentCombination.Clear();
        UpdateText();

    }

    public void CheckExistingInput()
    {
        if (currentCombination.SequenceEqual(correctCombination))
        {
            Debug.Log("Correct Combination");
        } else
        {
            ClearInput();
        }
    }

    private void UpdateText()
    {
        text.text = string.Join("", currentCombination);
    }
    
}