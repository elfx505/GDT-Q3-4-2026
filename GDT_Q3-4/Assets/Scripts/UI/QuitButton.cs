using UnityEngine;

public class QuitButton : MonoBehaviour
{   
    [SerializeField] private GameObject quitConfirmationScreen;

    void Start()
    {   
        if (quitConfirmationScreen == null)
        {
            Debug.LogWarning($"[QuitButton] {quitConfirmationScreen.gameObject.name}: quitConfirmationScreen is not set!");
        } else
        {
            quitConfirmationScreen.SetActive(false);
        }
    }


    public void ToggleQuitConfirmationScreen() // sets the object to the opposite of itself enabling wise
    {
        quitConfirmationScreen.SetActive(!quitConfirmationScreen.activeSelf);
    }

    public void QuitGame() // Only called from button inside Quit Game Confirmation Screen on YES
    {
        Loader.Load(Loader.Scene.MainMenu);
    }

    public void CancelGameQuit() // Only called from button inside Quit Game Confirmation Screen on NO
    {
        ToggleQuitConfirmationScreen();
    }
}
