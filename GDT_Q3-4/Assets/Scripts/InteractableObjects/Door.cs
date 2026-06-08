using UnityEngine;

public class Door : InteractableObject
{
    [Header("Location A (e.g., Office)")]
    [SerializeField] private CameraAnchor anchorA;

    [Header("Location B (e.g., Hallway)")]
    [SerializeField] private CameraAnchor anchorB;

    [Header("Door Status")]
    [SerializeField] private bool doorUnlocked = true;

    protected override void PerformAction()
    {
        base.PerformAction();

        if (anchorA == null || anchorB == null)
        {
            Debug.LogWarning($"[Door] Missing anchor assignments on {gameObject.name}!");
            return;
        }

        if (!doorUnlocked)
        {
            Debug.Log("The door is locked.");
            return;
        }

        // Determine which anchor is closer to the camera
        float distanceToA = Vector3.Distance(Camera.main.transform.position, anchorA.transform.position);
        float distanceToB = Vector3.Distance(Camera.main.transform.position, anchorB.transform.position);

        // Set the target to the furthest one (the one we want to move to)
        CameraAnchor targetAnchor = (distanceToA < distanceToB) ? anchorB : anchorA;

        // Execute the move
        GameManager.Instance.MoveToAnchor(targetAnchor);

        // Check the if it is a draw anchor
        GameManager.Instance.canDraw = targetAnchor.isDrawAnchor;
    }
    
    public void UnlockDoor()
    {
        doorUnlocked = true;
        Debug.Log("Door unlocked!");
    }
}