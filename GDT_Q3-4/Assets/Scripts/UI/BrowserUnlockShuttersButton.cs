using UnityEngine;
using UnityEngine.UI; 

public class BrowserUnlockShuttersButton : MonoBehaviour
{
   
    [SerializeField] private Button button;
    [SerializeField] private Animator shuttersAnimator; 
    private bool hasOpenedShutters = false;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float startTime;
    [SerializeField] private float endTime;

    void Start()
    {
        // Attach the listener to the button
        button.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {   

        if (hasOpenedShutters) return;

        hasOpenedShutters = true;

        Debug.Log("Open Shutters Button pressed!");

        // Play Shutters Opening Animation
        shuttersAnimator.SetBool("isOpen", true);

        // Update GameStates
        GameManager.Instance.SetState(GameState.ShuttersOpened, true);

        if (audioClip != null)
        {
            AudioManager.Instance.PlaySFX(audioClip, volume: volume, startTime: startTime, endTime: endTime);
        }

        
    }

    void OnDestroy()
    {
        // Clears all code-based listeners from this button
        button.onClick.RemoveAllListeners(); 
    }
}
