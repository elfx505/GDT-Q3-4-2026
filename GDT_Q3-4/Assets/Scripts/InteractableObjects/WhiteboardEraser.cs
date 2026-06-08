using UnityEngine;

public class WhiteboardEraser : InteractableObject
{
    [SerializeField] private WhiteboardDrawing whiteboard;

    void Start()
    {
        if (whiteboard == null)
        {
            Debug.LogWarning($"[WhiteboardEraser] {gameObject.name}: Missing Whiteboard Assignment!");
        }
    }

    protected override void PerformAction()
    {
        base.PerformAction();

        whiteboard.ClearWhiteboard();
    }
}
