using UnityEngine;
using System.Collections.Generic;
using System;

public class PuzzleManager : Singleton<PuzzleManager>
{

    public AudioTrack thisPuzzleTrack;

    public Slot[] slots;           // Must be ordered left → right in Inspector!
    public ConstraintUI[] constraints;
    [SerializeField] private int[] correctOrder = new int[6];
    [SerializeField] private int[] sequence = new int[4];
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

    public static event Action<bool> OnSequenceModeChanged;

    private static bool _sequenceMode;

    // Use a property so that changing the mode automatically fires the event
    public static bool sequenceMode
    {
        get { return _sequenceMode; }
        set
        {
            if (_sequenceMode != value)
            {
                _sequenceMode = value;
                // Fire the event, passing the new state
                OnSequenceModeChanged?.Invoke(_sequenceMode); 
            }
        }
    }

    public static event Action<bool> OnSequenceCompleteChanged;

    private static bool _sequenceComplete;

    public static bool sequenceComplete
    {
        get { return _sequenceComplete; }
        set
        {
            if (_sequenceComplete != value)
            {
                _sequenceComplete = value;
                OnSequenceCompleteChanged?.Invoke(_sequenceComplete); 
            }
        }
    }

    [SerializeField] private CameraAnchor receptionFloorElevatorAnchor;

    void Start()
    {   
        
        sequenceComplete = false;

        // Ensure correctOrder has values (0 to 5) and shuffle them
        // If all elements are 0 (unassigned in Inspector), auto-fill 0 to correctOrder.Length - 1
        bool isAllZeroes = true;
        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (correctOrder[i] != 0) { isAllZeroes = false; break; }
        }

        if (isAllZeroes)
        {
            for (int i = 0; i < correctOrder.Length; i++)
            {
                correctOrder[i] = i;
            }
        }
        
        Shuffle(correctOrder);

        // Generate a sequence of 4 UNIQUE slot indices without duplicates
        if (sequence.Length > slots.Length)
        {
            Debug.LogWarning("Sequence length cannot be greater than the number of slots! Adjusting sequence size.");
            System.Array.Resize(ref sequence, slots.Length);
        }

        // Populate a pool of all available slot indices (0, 1, 2... slots.Length - 1)
        int[] availableIndices = new int[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            availableIndices[i] = i;
        }

        // Shuffle the available indices and take the first 4 for the sequence
        Shuffle(availableIndices);
        for (int i = 0; i < sequence.Length; i++)
        {
            sequence[i] = availableIndices[i];
        }

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

        foreach (var c in constraints)
        {
            c.UpdateText();
        }

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
        GameManager.Instance.SetState(GameState.ElevatorPuzzleDone, true);
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
            
            slots[index].changeColor(true);
            currentStep++;

            if (currentStep >= sequence.Length)
            {
                Debug.Log("SEQUENCE COMPLETE 🎉");
                currentStep = 0;

                sequenceComplete = true; // Ensure lights stop blinking after the sequence has been completed.

                 // Trigger Player Movement to Reception floor
                GameManager.Instance.MoveToAnchor(receptionFloorElevatorAnchor);               
                // Update GameState after reaching the next floor to trigger dialogue only then
                GameManager.Instance.SetState(GameState.ElevatorButtonSequencePressed, true);



            }
        }
        else
        {
            Debug.Log("WRONG SEQUENCE");
            Debug.Log(index + "pressed");

            slots[index].changeColor(false);
            currentStep = 0;
        }
    }
}