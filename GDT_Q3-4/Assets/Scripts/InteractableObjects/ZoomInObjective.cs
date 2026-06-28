using UnityEngine;
using System.Collections.Generic;

public class ZoomInObjective : InteractableObject
{
    [SerializeField] private GameObject lockedCameraPerspective;
    private Collider hitboxCollider;
    [SerializeField] private CameraAnchor previousAnchor; // Set In Inspector
    [SerializeField] private List<Collider> optionalColliders = new List<Collider>(); // In Case other colliders are in the way


    private void Start()
    {

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

        // SUBSCRIBE ONLY WHEN ZOOMING IN
        BackButton.onBackButton += GoToPreviousAnchor;

        CameraManager.Instance.MoveCameraToAnchor(lockedCameraPerspective.transform);

        CameraManager.Instance.SetRotation(lockedCameraPerspective.transform.rotation);

        GameManager.Instance.TogglePerspectiveLock();

        hitboxCollider.enabled = false;

        foreach (Collider collider in optionalColliders)
        {
            if (collider == null) continue;

            collider.enabled = false;
        }

    }

    private void GoToPreviousAnchor()
    {
        // UNSUBSCRIBE IMMEDIATELY WHEN BACKING OUT
        BackButton.onBackButton -= GoToPreviousAnchor;

        GameManager.Instance.MoveToAnchor(previousAnchor);
        GameManager.Instance.TogglePerspectiveLock();
        CameraManager.Instance.RecalibrateCamera();

        hitboxCollider.enabled = true;

        foreach (Collider collider in optionalColliders)
        {
            if (collider == null) continue;

            collider.enabled = true;
        }

    }



}
