using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public Slot[] slots;           // Must be ordered left → right in Inspector!
    public ConstraintUI[] constraints;

    void Awake()
    {
        Instance = this;
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
}