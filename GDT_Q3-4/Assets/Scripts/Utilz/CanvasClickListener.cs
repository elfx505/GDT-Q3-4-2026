using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.Events;       


public class CanvasClickListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Custom Click Events")]
    public UnityEvent onLeftClick;
    public UnityEvent onRightClick;

    public UnityEvent onLeftDown;
    public UnityEvent onLeftUp;
    public UnityEvent onRightUp;
    
    public UnityEvent onPointerExit;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onLeftClick?.Invoke(); 
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRightClick?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            onLeftDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            onLeftUp?.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            onRightUp?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Triggers regardless of which mouse button was being held
        onPointerExit?.Invoke(); 
    }
}
