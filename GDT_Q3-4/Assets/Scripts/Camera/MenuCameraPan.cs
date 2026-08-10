using UnityEngine;

public class MenuCameraPan : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSpeed = 2f;
    public Vector3 panDirection = Vector3.right; // Moves along the X axis

    void Update()
    {
        // Smoothly translates the camera in the chosen direction
        transform.Translate(panDirection * panSpeed * Time.deltaTime, Space.Self);
    }
}
