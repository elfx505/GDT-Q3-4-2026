using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Ensure Game Scene is Present in Build Settings
    public string mainGameSceneName = "GameScene"; 
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;

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

    public void OnPlayButton()
    {
        SceneManager.LoadScene(mainGameSceneName);
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
}
