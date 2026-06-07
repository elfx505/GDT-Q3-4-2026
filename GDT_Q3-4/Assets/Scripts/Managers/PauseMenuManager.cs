using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    // Action Events
    public static event Action<float> onLookSensitivityChanged;
    public static event Action<float> onMasterVolumeChanged; // TO-DO, once we agree on what the main Audio Mixer object is

    void Awake()
    {
        
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
    
        } else
        {
            Debug.LogWarning("[PauseMenuManager] pauseMenuUI Gameobject not set in Inspector!");
        }

    }

    void Start()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.onEscape += TogglePause;
        } else
        {
            Debug.LogWarning("[PauseMenuManagerl] InputManager.Instance is null!");
        }

        if (CameraManager.Instance != null)
        {
            sensitivitySlider.value = CameraManager.Instance.GetLookSensitivity();
        } else
        {
            Debug.LogWarning("[PauseMenuManagerl] CameraManager.Instance is null!");
        }
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.onEscape -= TogglePause;
        } else
        {
            Debug.LogWarning("[PauseMenuManagerl] InputManager.Instance is null!");
        }
    }

    private void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resumes normal time
        GameManager.Instance.gameIsPaused = false;

    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freezes time in the game
        GameManager.Instance.gameIsPaused = true;

    }

    private void TogglePause()
    {
        Debug.Log("TogglePause was successfully called!");       

        if (GameManager.Instance.gameIsPaused) Resume(); else Pause();
    }

    public void SetSensitivity(float sensitivity) // Called by Slider Objects; Set up in Inspector
    {
        onLookSensitivityChanged?.Invoke(sensitivity);
    }

}
