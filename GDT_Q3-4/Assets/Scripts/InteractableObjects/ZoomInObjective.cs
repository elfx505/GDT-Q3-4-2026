using UnityEngine;

public class ZoomInObjective : InteractableObject
{   
    [SerializeField] private Transform lockedCameraPerspectiveTransform;   


    protected override void PerformAction()
    {
        base.PerformAction();

        CameraManager.Instance.MoveCameraToAnchor(lockedCameraPerspectiveTransform);

        GameManager.Instance.perspectiveIsLocked = true;

    }


    
}
