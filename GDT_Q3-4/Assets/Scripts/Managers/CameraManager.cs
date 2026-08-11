using System.Collections;
using UnityEditor.Compilation;
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
    [SerializeField] private bool startDark = false;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 0.2f;
    private float defaultSensitivity = 0.5f;
    [SerializeField] private float minPitch = -60f; // Look up limit
    [SerializeField] private float maxPitch = 60f;  // Look down limit

    private float focusRotationSpeed = 10f;
    private bool isLookInitialized = false;

    private bool isTransitioning = false;
    public bool isFaded = false;
    private float currentPitch;
    private float currentYaw;
    private Transform focusTarget;
    private bool blinkedOnce = false;

    protected override void Awake()
    {
        base.Awake();

        // Initialize sensitivity early so PauseMenuManager can safely read it later
        SetLookSensitivity(PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity));
    }

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

    public void RecalibrateCamera()
    {
        Vector3 euler = mainCamera.transform.eulerAngles;

        currentYaw = euler.y;

        currentPitch = euler.x;
        if (currentPitch > 180f) currentPitch -= 360f;
    }

    public void SetCameraTarget(Transform target, float speed)
    {
        focusTarget = target;
        focusRotationSpeed = speed;
        if (target == null)
        {
            RecalibrateCamera();
        }
    }

    private void HandleCameraLook(Vector2 delta)
    {
        if (isTransitioning) return;
        if (isFaded) return;
        if (!isLookInitialized)
        {
            RecalibrateCamera();
            isLookInitialized = true;
        }
        currentYaw += delta.x * lookSensitivity;
        currentPitch -= delta.y * lookSensitivity;
        float preClamp = currentPitch;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        mainCamera.transform.eulerAngles = new Vector3(currentPitch, currentYaw, 0f);
    }

    public void MoveCameraToAnchor(Transform targetAnchor, bool useAnchorRotation = false)
    {
        if (isTransitioning) return;
        StartCoroutine(BlinkAndMoveRoutine(targetAnchor, useAnchorRotation));
    }

    private IEnumerator BlinkAndMoveRoutine(Transform targetAnchor, bool useAnchorRotation)
    {
        isTransitioning = true;
        if (!startDark || blinkedOnce)
        {
            yield return StartCoroutine(FadeBlink(0f, 1f));
        }
        else
        {
            yield return StartCoroutine(FadeBlink(1f, 1f));
        }

        // ONLY change position. Let rotation (and our currentPitch/currentYaw) stay exactly as they are!
        mainCamera.transform.position = targetAnchor.position;
        // if(targetAnchor.)
        if (useAnchorRotation)
        {
            mainCamera.transform.rotation = targetAnchor.rotation;
            RecalibrateCamera();
        }

        yield return new WaitForSeconds(0.05f);
        if (!startDark || blinkedOnce)
        {
            Debug.Log("Fading");
            yield return StartCoroutine(FadeBlink(1f, 0f));
        }
        blinkedOnce = true;
        yield return new WaitForSeconds(0.5f);
        isTransitioning = false;
    }

    private IEnumerator FadeBlink(float startAlpha, float endAlpha, float duration = 0)
    {
        isFaded = true;
        float elapsedTime = 0f;
        Color overlayColor = blinkOverlay.color;
        float tempBlinkDuration = duration == 0 ? blinkDuration : duration;
        while (elapsedTime < tempBlinkDuration)
        {
            elapsedTime += Time.deltaTime;
            overlayColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / tempBlinkDuration);
            blinkOverlay.color = overlayColor;
            Debug.Log(blinkOverlay.color);
            yield return null;
        }

        overlayColor.a = endAlpha;
        blinkOverlay.color = overlayColor;
        isFaded = false;

    }

    public IEnumerator FadeIn(float duration)
    {
        yield return StartCoroutine(FadeBlink(0f, 1f, duration));
    }

    public IEnumerator FadeOut(float duration)
    {
        yield return StartCoroutine(FadeBlink(blinkOverlay.color.a, 0f, duration));
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