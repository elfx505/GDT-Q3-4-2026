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

        if (anchorA == null || anchorB == null)
        {
            Debug.LogWarning($"[Door] Missing anchor assignments on {gameObject.name}!");
            return;
        }

        if (doorUnlocked)
        {
            if (triggersGameState)
            {
                // 2. Tell the GameManager to update the state using the Inspector values
                GameManager.Instance.SetState(stateToChange, targetStateValue);
            }
            
            float distanceToA = Vector3.Distance(Camera.main.transform.position, anchorA.transform.position);
            float distanceToB = Vector3.Distance(Camera.main.transform.position, anchorB.transform.position);

            if (distanceToA < distanceToB)
            {
                GameManager.Instance.MoveToAnchor(anchorB);
            }
            else
            {
                GameManager.Instance.MoveToAnchor(anchorA);
            }
        }
        else
        {
            Debug.Log("The door is locked.");
        }
    }
    
    public void UnlockDoor()
    {
        doorUnlocked = true;
        Debug.Log("Door unlocked!");
    }
}