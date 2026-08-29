using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NumpadLock : MonoBehaviour
{


    [SerializeField] private int[] correctCombination = new int[4];
    private List<int> currentCombination;
    [SerializeField] private int maxCombinationSize = 4;
    [SerializeField] TextMeshPro text;
    [SerializeField] TextMeshPro bossMonitorText;

    
    void Awake()
    {
        for (int i = 0; i < correctCombination.Length; i++)
        {
            correctCombination[i] = UnityEngine.Random.Range(0, 10);
        }
        
        currentCombination = new List<int>();

        if (text == null) Debug.LogWarning("[NumpadLock] Text object not assigned!");
    }

    void Start()
    {
        UpdateMonitorTextWithCorrectCombination(correctCombination);
    }

    private void UpdateMonitorTextWithCorrectCombination(int[] combination)
    {
        string newText = "";

        foreach (int i in combination)
        {
            newText += i.ToString();
        }

        Debug.Log(newText);

        bossMonitorText.text = newText;
    }


    public void RegisterInput(int value)
    {
        if (currentCombination.Count >= maxCombinationSize) return;

        currentCombination.Add(value);
        UpdateText();
    }

    public void ClearInput()
    {
        if (currentCombination.Count <= 0) return;

        currentCombination.Clear();
        UpdateText();

    }

    public bool CheckExistingInput()
    {
        if (currentCombination.SequenceEqual(correctCombination))
        {
            Debug.Log("Correct Combination");

            GameManager.Instance.SetState(GameState.JanitorDoorUnlocked, true);
            return true;
        } else
        {
            ClearInput();
            return false;
        }
    }

    private void UpdateText()
    {
        text.text = string.Join("", currentCombination);
    }
    
}