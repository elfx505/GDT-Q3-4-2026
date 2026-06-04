using UnityEngine;
using System.Collections.Generic;
using System;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public AudioTrack thisPuzzleTrack;

    public Slot[] slots;           // Must be ordered left → right in Inspector!
    public ConstraintUI[] constraints;
    public int[] correctOrder;
    public int[] sequence;
    string[] symbols = new string[] {"!", "@", "#", "$", "%", "^"};
    public Dictionary<string, int> symbolToIndex = new Dictionary<string, int>()
    {
        {"!", 0},
        {"@", 1},
        {"#", 2},
        {"$", 3},
        {"%", 4},
        {"^", 5}
    };
    private int currentStep = 0;
    public static bool sequenceMode = false;

    void Awake()
    {
        Instance = this;
        foreach (var i in sequence) 
        {
            if (i >= slots.Length)
            {
                Debug.Log("Problematic setup");
            }
        }
        GenerateConstraints();
        AddExtraConstraints();
        Shuffle<ConstraintUI>(constraints);
    }

    void Start()
    {
        AudioManager.Instance.PlayTrack(thisPuzzleTrack);
    }

    public void CheckWin()
    {
        // Update visual feedback on constraints
        foreach (var c in constraints)
            c.Evaluate();

        // Check if all slots are filled
        foreach (var slot in slots)
        {
            if (!slot.IsFilled())
                return;
        }

        // All filled → check constraints
        foreach (var c in constraints)
        {
            int leftIdx = GetSymbolIndex(c.leftSymbol);
            int rightIdx = GetSymbolIndex(c.rightSymbol);

            bool satisfied = false;

            if (c.operatorSymbol == "<")
                satisfied = leftIdx < rightIdx;
            else if (c.operatorSymbol == ">")
                satisfied = leftIdx > rightIdx;

            if (!satisfied)
                return;   // Fail early
        }

        Debug.Log("PUZZLE COMPLETE! 🎉");
        // TODO: Add win effects here (particles, sound, next level...)
        sequenceMode = true;
    }

    public int GetSymbolIndex(string symbol)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentButton != null && 
                slots[i].currentButton.symbol == symbol)
            {
                return i;
            }
        }
        return -1;
    }

    void GenerateConstraints()
    {
        for (int i = 0; i < correctOrder.Length - 1; i++)
        {
            string left = symbols[correctOrder[i]];
            string right = symbols[correctOrder[i + 1]];

            int r = UnityEngine.Random.Range(0, 2);
            if (r == 0) 
            {
                constraints[i].leftSymbol = left;
                constraints[i].rightSymbol = right;
                constraints[i].operatorSymbol = "<";
            } else 
            {
                constraints[i].leftSymbol = right;
                constraints[i].rightSymbol = left;
                constraints[i].operatorSymbol = ">";
            }
        }
    }

    void AddExtraConstraints()
    {
        int a = UnityEngine.Random.Range(0, correctOrder.Length);
        int b = UnityEngine.Random.Range(0, correctOrder.Length);

        if (a == b) return;

        int posA = System.Array.IndexOf(correctOrder, a);
        int posB = System.Array.IndexOf(correctOrder, b);

        ConstraintUI c = constraints[UnityEngine.Random.Range(slots.Length - 1, constraints.Length)];

        if (posA < posB)
        {
            c.leftSymbol = symbols[a];
            c.rightSymbol = symbols[b];
            c.operatorSymbol = "<";
        }
        else
        {
            c.leftSymbol = symbols[a];
            c.rightSymbol = symbols[b];
            c.operatorSymbol = ">";
        }
    }

    public static void Shuffle<T>(T[] array)
    {
        // Use System.Random, not UnityEngine.Random
        System.Random rng = new System.Random();
    
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
        
            // Swap
            T temp = array[k];
            array[k] = array[n];
            array[n] = temp;
        }
    }

    public void PressButton(int index)
    {
        if (!sequenceMode) return;

        if (index == sequence[currentStep])
        {
            currentStep++;

            if (currentStep >= sequence.Length)
            {
                Debug.Log("SEQUENCE COMPLETE 🎉");
                currentStep = 0;
            }
        }
        else
        {
            Debug.Log("WRONG SEQUENCE");
            Debug.Log(index + "pressed");

            slots[index].changeColor();
            currentStep = 0;
        }
    }
}