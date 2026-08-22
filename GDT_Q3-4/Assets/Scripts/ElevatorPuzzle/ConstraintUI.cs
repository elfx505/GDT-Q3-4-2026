using UnityEngine;
using TMPro;

public class ConstraintUI : MonoBehaviour
{
    public PuzzleColor leftColor;
    public PuzzleColor rightColor;
    public string operatorSymbol; // ">" or "<"

    [SerializeField] private SpriteRenderer leftDotSprite;
    [SerializeField] private TextMeshPro operatorText;
    [SerializeField] private SpriteRenderer rightDotSprite;

    void Start()
    {   
        // Indices for Children are strict
        // Grabbing SpriteRenderers for the dots and TextMeshPro for the operator
        leftDotSprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        operatorText = gameObject.transform.GetChild(1).GetComponent<TextMeshPro>();
        rightDotSprite = gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>();
        
        // We can still call it here just as a failsafe
        UpdateText();
    }

    private void OnEnable() => PuzzleManager.OnSequenceModeChanged += HandleSequenceModeSwitch;
    private void OnDisable() => PuzzleManager.OnSequenceModeChanged -= HandleSequenceModeSwitch;

    private Color GetUnityColor(PuzzleColor color)
    {
        switch (color)
        {
            case PuzzleColor.Cyan: return Color.cyan;
            case PuzzleColor.Yellow: return Color.yellow;
            case PuzzleColor.Red: return Color.red;
            case PuzzleColor.Green: return Color.green;
            case PuzzleColor.Blue: return Color.blue;
            case PuzzleColor.Magenta: return Color.magenta;
            default: return Color.white;
        }
    }

    public void UpdateText()
    {
        if (operatorText != null) operatorText.text = operatorSymbol;

        // Apply colors to the SpriteRenderers instead of TextMeshPro properties
        if (leftDotSprite != null) leftDotSprite.color = GetUnityColor(leftColor);
        if (rightDotSprite != null) rightDotSprite.color = GetUnityColor(rightColor);
        if (operatorText != null) operatorText.color = Color.white; 
    }

    public void Evaluate()
    {
        int leftIndex = PuzzleManager.Instance.GetColorIndex(leftColor);
        int rightIndex = PuzzleManager.Instance.GetColorIndex(rightColor);

        if (leftIndex == -1 || rightIndex == -1)
        {
            if (operatorText != null) operatorText.color = Color.gray;
            return;
        }

        bool correct = false;
        if (operatorSymbol == ">") correct = leftIndex > rightIndex;
        else if (operatorSymbol == "<") correct = leftIndex < rightIndex;

        if (operatorText != null) operatorText.color = correct ? Color.green : Color.red;
    }

    private void HandleSequenceModeSwitch(bool isSequenceMode)
    {
        if (!isSequenceMode) return;
        gameObject.SetActive(false);
    }
}