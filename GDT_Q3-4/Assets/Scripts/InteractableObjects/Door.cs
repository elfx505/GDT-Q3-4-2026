using UnityEngine;

public class Door : InteractableObject
{
    
    [SerializeField] GameObject destinationRoom;
    protected override void PerformAction()
    {
        base.PerformAction();
        if (destinationRoom == null)
        {
            Debug.LogWarning("Destination Room not set!");
            return;
        }   

        GameManager.Instance.ToggleRooms(destinationRoom);

    }
}
