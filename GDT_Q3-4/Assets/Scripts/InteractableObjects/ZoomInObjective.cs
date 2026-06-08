using UnityEngine;

public class ZoomInObjective : InteractableObject
{   
    [SerializeField] private GameObject lockedCameraPerspective;   
    private Collider hitboxCollider;
    [SerializeField] private CameraAnchor previousAnchor; // Set In Inspector


    private void Start()
    {
        BackButton.onBackButton += GoToPreviousAnchor;
        
        if (previousAnchor == null)
        {
            Debug.LogWarning($"[ZoomInObjective] {gameObject.name}: previousAnchor not set!");
        }

        hitboxCollider = gameObject.GetComponent<Collider>();
    }

    private void OnDestroy()
    {
        BackButton.onBackButton -= GoToPreviousAnchor;
    }

    protected override void PerformAction()
    {
        base.PerformAction();

        CameraManager.Instance.MoveCameraToAnchor(lockedCameraPerspective.transform);

        CameraManager.Instance.SetRotation(lockedCameraPerspective.transform.rotation);

        GameManager.Instance.TogglePerspectiveLock();

        hitboxCollider.enabled = false;

    }

    private void GoToPreviousAnchor()
    {
        GameManager.Instance.MoveToAnchor(previousAnchor);
        GameManager.Instance.TogglePerspectiveLock();

        hitboxCollider.enabled = true;
    }


    
}
