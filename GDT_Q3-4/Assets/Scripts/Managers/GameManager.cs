using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Camera System")]
    [SerializeField] private CameraAnchor startingAnchor;
    
    private CameraAnchor currentAnchor;
    public CameraAnchor CurrentAnchor => currentAnchor; 

    [Header("Game State")]
    public GameStateProfile startingProfile;
    private Dictionary<GameState, bool> gameStates = new Dictionary<GameState, bool>();
    public static event Action<GameState> onGameStateChange;
    public bool canDraw;

    protected override void Awake()
    {
        base.Awake();
        InitializeGameStatesFromProfile();
        SetState(GameState.GameStart, true);
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
    }

    public void MoveToAnchor(CameraAnchor targetAnchor)
    {
        if (targetAnchor == null) return;

        currentAnchor = targetAnchor;
        
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
        onGameStateChange?.Invoke(key);
    }

    public bool GetState(GameState key)
    {
        return gameStates.ContainsKey(key) && gameStates[key];
    }
}