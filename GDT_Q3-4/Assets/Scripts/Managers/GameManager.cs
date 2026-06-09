using System;
using System.Collections.Generic;
using UnityEngine;

// For Debugging (View Game State Data in Inspector)
[Serializable]
public struct GameStateData
{
    public GameState state;
    public bool isActive;
}


public class GameManager : Singleton<GameManager>
{
    [Header("Camera System")]
    [SerializeField] private CameraAnchor startingAnchor;
    
    private CameraAnchor currentAnchor;
    public CameraAnchor CurrentAnchor => currentAnchor; 

    [Header("Game State")]
    public GameStateProfile startingProfile;

    [Header("Debug")]
    [SerializeField] private List<GameStateData> debugStatesList = new List<GameStateData>();
    private Dictionary<GameState, bool> gameStates = new Dictionary<GameState, bool>();
    public static event Action<GameState> onGameStateChange;
    public bool canDraw;
    public bool gameIsPaused;
    public bool perspectiveIsLocked = false;
    public bool textOnScreen = false;
    public GameObject backButton; // Set In Inspector

    protected override void Awake()
    {
        base.Awake();
        InitializeGameStatesFromProfile();
        SetState(GameState.IntroSequenceDone, true);
    }

    private void Start()
    {
        if (startingAnchor != null)
        {
            MoveToAnchor(startingAnchor);
        }
        else
        {
            Debug.LogWarning("[GameManager] Starting Anchor not set!");
        }
        
        if (backButton != null)
        {
            backButton.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[GameManager] Back Button not set!");
        }
    }

    public void MoveToAnchor(CameraAnchor targetAnchor)
    {
        if (targetAnchor == null) return;

        if (currentAnchor != null)
        {
            currentAnchor.ToggleActiveState(true);
        }

        currentAnchor = targetAnchor;
        currentAnchor.ToggleActiveState(false);
        
        // Tell the Camera Transition script to do the blink & move (position only)
        CameraManager.Instance.MoveCameraToAnchor(targetAnchor.transform);
    }

    private void InitializeGameStatesFromProfile()
    {
        if (startingProfile == null) return;

        foreach (var item in startingProfile.states)
        {
            gameStates[item.key] = item.value;
        }

        Debug.Log("Loaded Game State Data into gameState Dict!");
    }

    public void SetState(GameState key, bool value)
    {
        gameStates[key] = value;
        UpdateDebugList(); // Update the list whenever the state changes
        onGameStateChange?.Invoke(key);
    }

    public bool GetState(GameState key)
    {
        return gameStates.ContainsKey(key) && gameStates[key];
    }

    public void TogglePerspectiveLock()
    {
        perspectiveIsLocked = !perspectiveIsLocked;
        backButton.SetActive(perspectiveIsLocked);
    }

    private void UpdateDebugList()
    {
        debugStatesList.Clear();
        foreach (var kvp in gameStates)
        {
            debugStatesList.Add(new GameStateData { state = kvp.Key, isActive = kvp.Value });
        }
    }


}