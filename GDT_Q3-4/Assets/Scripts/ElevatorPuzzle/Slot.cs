using UnityEngine;

public class Slot : MonoBehaviour
{
    public DraggableButton currentButton;

    public bool IsFilled() => currentButton != null;
}