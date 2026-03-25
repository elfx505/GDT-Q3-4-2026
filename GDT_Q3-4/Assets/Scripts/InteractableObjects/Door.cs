using UnityEngine;

public class Door : InteractableObject
{

    [SerializeField] GameObject destinationRoom;
    [SerializeField] private bool doorUnlocked = true;
    protected override void PerformAction()
    {
        base.PerformAction();
        if (destinationRoom == null)
        {
            Debug.LogWarning("Destination Room not set!");
            return;
        }
        if (doorUnlocked)
        {
            GameManager.Instance.ToggleRooms(destinationRoom);
        }

    }

    public void UnlockDoor()
    {
        doorUnlocked = true;
        Debug.Log("Door unlocked!");
    }
}
