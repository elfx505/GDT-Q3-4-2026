using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // Ensure Game Scene is Present in Build Settings
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;

    [Header("Settings UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private float defaultVolume = .5f;
    [SerializeField] private float defaultSensitivity = .5f;

    private void Awake()
    {
        if (mainMenu == null)
        {
            Debug.LogWarning("[MainMenuManager] Main Menu GameObject is missing!");
        }
        
        if (settingsMenu == null)
        {
            Debug.LogWarning("[MainMenuManager] Settings Menu GameObject is missing!");
        }

        ToggleMenus(true);
    }

    private void Start()
    {
        // Load the saved values into the Main Menu sliders on boot
        // Using SetValueWithoutNotify prevents accidental saving on startup
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVolume", defaultVolume));
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity));
        }
    }

    public void OnPlayButton()
    {
        Loader.Load(Loader.Scene.Test);
    }

    public void OnSettingsButton()
    {
        ToggleMenus(false);
    }

    public void OnSettingsMenuBackButton()
    {
        ToggleMenus(true);
    }

    public void OnQuitButton()
    {
        Debug.Log("Quit Game!"); 
        Application.Quit();
    }

    private void ToggleMenus(bool mainMenuOn)
    {
        mainMenu.SetActive(mainMenuOn);
        settingsMenu.SetActive(!mainMenuOn);
    }

    public void SetSensitivity(float sensitivity) // Called by Slider Objects; Set up in Inspector
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        PlayerPrefs.Save();
    }


    public void SetVolume(float volume) // Called by Slider Objects; Set up in Inspector
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
}
