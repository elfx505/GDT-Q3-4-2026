using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Use LateUpdate instead of Update to ensure the camera has completely 
    // finished moving for the frame before the sprite rotates to match it.
    void LateUpdate()
    {
        if (mainCamera == null) return;

        transform.forward = mainCamera.transform.forward;

    }
}