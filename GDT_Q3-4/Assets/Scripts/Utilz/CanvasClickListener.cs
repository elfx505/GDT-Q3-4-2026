using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.Events;       


public class CanvasClickListener : MonoBehaviour, IPointerClickHandler
{
    [Header("Custom Click Events")]
    // Assign functions in the Inspector
    public UnityEvent onLeftClick;
    public UnityEvent onRightClick;

    // Required by the IPointerClickHandler interface
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check which mouse button was pressed
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onLeftClick?.Invoke(); // Trigger any functions assigned in the Inspector
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRightClick?.Invoke();
        }
        
    }
}
