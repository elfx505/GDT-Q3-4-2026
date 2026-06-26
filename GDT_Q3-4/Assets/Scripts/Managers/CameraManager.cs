using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : Singleton<CameraManager>
{
    [Header("References")]
    [Tooltip("The main camera that will be moving.")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("The black UI image used for the blink effect.")]
    [SerializeField] private Image blinkOverlay;

    [Header("Blink Settings")]
    [SerializeField] private float blinkDuration = 0.15f;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 0.2f;
    [SerializeField] private float minPitch = -60f; // Look up limit
    [SerializeField] private float maxPitch = 60f;  // Look down limit

    private float focusRotationSpeed = 10f;
    private bool isLookInitialized = false;

    private bool isTransitioning = false;
    private float currentPitch;
    private float currentYaw;
    private Transform focusTarget;


    private void Start()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnLookRotate += HandleCameraLook;
        }
        else
        {
            Debug.LogError("InputManager Instance is still null in Start!");
        }

        PauseMenuManager.onLookSensitivityChanged += SetLookSensitivity;

        // mainCamera.transform.rotation = Quaternion.Euler(9.5f, 70f, 0f);

    }

    private void Update()
    {
        if (focusTarget)
        {

            Vector3 direction = (focusTarget.position - mainCamera.transform.position).normalized;
            // context.MainCamera.transform.LookAt(focusTarget);
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation,
                targetRotation,
                focusRotationSpeed * Time.deltaTime
            );
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnLookRotate -= HandleCameraLook;
        }

        PauseMenuManager.onLookSensitivityChanged -= SetLookSensitivity;
    }

    public void SetCameraTarget(Transform target, float speed)
    {
        focusTarget = target;
        focusRotationSpeed = speed;
        if (target == null)
        {
            Vector3 euler = mainCamera.transform.eulerAngles;

            currentYaw = euler.y;

            currentPitch = euler.x;
            if (currentPitch > 180f) currentPitch -= 360f;
        }
    }

    private void HandleCameraLook(Vector2 delta)
    {
        if (isTransitioning) return;
        if (!isLookInitialized)
        {
            currentYaw = mainCamera.transform.eulerAngles.y;
            currentPitch = mainCamera.transform.eulerAngles.x;
            isLookInitialized = true;
        }
        currentYaw += delta.x * lookSensitivity;
        currentPitch -= delta.y * lookSensitivity;

        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        mainCamera.transform.eulerAngles = new Vector3(currentPitch, currentYaw, 0f);
    }

    public void MoveCameraToAnchor(Transform targetAnchor)
    {
        if (isTransitioning) return;

        StartCoroutine(BlinkAndMoveRoutine(targetAnchor));
    }

    private IEnumerator BlinkAndMoveRoutine(Transform targetAnchor)
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeBlink(0f, 1f));

        // ONLY change position. Let rotation (and our currentPitch/currentYaw) stay exactly as they are!
        mainCamera.transform.position = targetAnchor.position;

        yield return new WaitForSeconds(0.05f);

        yield return StartCoroutine(FadeBlink(1f, 0f));

        isTransitioning = false;
    }

    private IEnumerator FadeBlink(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color overlayColor = blinkOverlay.color;

        while (elapsedTime < blinkDuration)
        {
            elapsedTime += Time.deltaTime;
            overlayColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / blinkDuration);
            blinkOverlay.color = overlayColor;
            yield return null;
        }

        overlayColor.a = endAlpha;
        blinkOverlay.color = overlayColor;
    }

    public float GetLookSensitivity()
    {
        return lookSensitivity;
    }

    public void SetLookSensitivity(float newSensitivity)
    {
        lookSensitivity = newSensitivity;
    }

    public void SetRotation(Quaternion newRotation)
    {
        if (mainCamera != null)
        {
            mainCamera.transform.rotation = newRotation;
        }
    }

}