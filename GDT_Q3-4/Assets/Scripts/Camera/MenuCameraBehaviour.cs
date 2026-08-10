using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MenuCameraBehaviour : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 5f; // Speed in degrees per second
    [SerializeField] private Vector3 spinAxis = Vector3.up; // Spins around the Y axis (left to right)

    [Header("Anchor Settings")]
    [SerializeField] private List<Transform> anchors;
    private int currentAnchorIndex = 0;
    private float accumulatedRotation = 0f;

    [Header("Blink Transition Settings")]
    [Tooltip("The black UI image used for the blink effect.")]
    [SerializeField] private Image blinkOverlay;
    [SerializeField] private float blinkDuration = 0.25f;

    private bool isTransitioning = false;

    void Start()
    {
        if (anchors.Count == 0)
        {
            Debug.LogWarning("[MenuCameraBehaviour] anchors Transform List is empty!");
            return;
        }

        // Initialize the camera to the very first anchor upon starting
        transform.position = anchors[0].position;
        transform.rotation = anchors[0].rotation;

        // Ensure the screen starts fully transparent
        if (blinkOverlay != null)
        {
            Color c = blinkOverlay.color;
            c.a = 0f;
            blinkOverlay.color = c;
        }
    }
    void Update()
    {
        // Prevent rotation logic from running if the anchors list is empty
        if (anchors.Count == 0) return;

        // Calculate how much we are rotating this frame
        float rotationStep = spinSpeed * Time.deltaTime;
        
        // Smoothly rotates the camera around the chosen axis
        // We use Space.World to ensure the camera doesn't tilt off-axis if it's angled downward
        transform.Rotate(spinAxis, rotationStep, Space.World);

        // Keep track of how far we've rotated (use Abs to account for negative spin speeds)
        accumulatedRotation += Mathf.Abs(rotationStep);

        // Once we hit or exceed 360 degrees, trigger the transition
        if (accumulatedRotation >= 360f)
        {
            // Reset the accumulator for the next loop
            accumulatedRotation = 0f;
            StartCoroutine(SwitchAnchor());
        }
    }

    private IEnumerator SwitchAnchor()
    {
        isTransitioning = true;

        // 1. Fade to Black (Alpha 0 to 1)
        yield return StartCoroutine(FadeBlink(0f, 1f));

        // 2. Increment index and wrap back to 0 if we hit the end of the list
        currentAnchorIndex = (currentAnchorIndex + 1) % anchors.Count;
        Transform targetAnchor = anchors[currentAnchorIndex];

        // 3. Snap camera position and rotation to the new anchor
        transform.position = targetAnchor.position;
        transform.rotation = targetAnchor.rotation;

        // Brief buffer time to ensure everything renders properly before revealing
        yield return new WaitForSeconds(0.05f);

        // 4. Fade back to Transparent (Alpha 1 to 0)
        yield return StartCoroutine(FadeBlink(1f, 0f));

        isTransitioning = false;
    }

    private IEnumerator FadeBlink(float startAlpha, float endAlpha)
    {
        if (blinkOverlay == null)
        {
            Debug.LogWarning("[MenuCameraBehaviour] Blink Overlay Image is not assigned!");
            yield break;
        }

        float elapsedTime = 0f;
        Color overlayColor = blinkOverlay.color;

        while (elapsedTime < blinkDuration)
        {
            elapsedTime += Time.deltaTime;
            overlayColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / blinkDuration);
            blinkOverlay.color = overlayColor;
            yield return null;
        }

        // Ensure the final alpha is exactly the target amount at the end
        overlayColor.a = endAlpha;
        blinkOverlay.color = overlayColor;
    }
}
