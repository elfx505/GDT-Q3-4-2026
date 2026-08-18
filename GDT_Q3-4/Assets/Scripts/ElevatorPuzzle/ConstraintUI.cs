using UnityEngine;
using TMPro;

public class ConstraintUI : MonoBehaviour
{
    public string leftSymbol;
    public string rightSymbol;
    public string operatorSymbol; // ">" or "<"

    private TextMeshPro text;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        Color color = text.color;
        color = Color.gray;
        color.a = text.color.a;
        text.color = color;
        UpdateText();
    }

    private void OnEnable()
    {
        PuzzleManager.OnSequenceModeChanged += HandleSequenceModeSwitch;
    }

    private void OnDisable()
    {
        PuzzleManager.OnSequenceModeChanged -= HandleSequenceModeSwitch;
    }

    public void UpdateText()
    {
        text.text = leftSymbol + " " + operatorSymbol + " " + rightSymbol;
    }

    public void Evaluate()
    {
        int leftIndex = PuzzleManager.Instance.GetSymbolIndex(leftSymbol);
        int rightIndex = PuzzleManager.Instance.GetSymbolIndex(rightSymbol);

        // If not placed yet
        if (leftIndex == -1 || rightIndex == -1)
        {
            Color color = text.color;
            color = Color.gray;
            color.a = text.color.a;
            text.color = color;
            return;
        }

        bool correct = false;

        if (operatorSymbol == ">")
            correct = leftIndex > rightIndex;
        else if (operatorSymbol == "<")
            correct = leftIndex < rightIndex;

        text.color = correct ? Color.green : Color.red;
    }

    private void HandleSequenceModeSwitch(bool isSequenceMode)
    {
        if (!isSequenceMode) return;

        gameObject.SetActive(false);

    }


}