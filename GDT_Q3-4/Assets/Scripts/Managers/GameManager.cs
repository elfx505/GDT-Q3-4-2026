using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    
    GameObject[] rooms;
    List<GameObject> activeRoomViews;
    [SerializeField] private GameObject initialRoom;
    private GameObject activeRoom;
    private int currentViewIndex = 0;

    public GameStateProfile startingProfile;

    private Dictionary<string, bool> gameStates = new Dictionary<string, bool>();

    public static event Action<string> onGameStateChange;
    
    protected override void Awake()
    {
        base.Awake();

        InitializeGameStatesFromProfile();
        
        GameManager.Instance.SetState("game_start", true);
        
        // Get all Room Objects in the Scene
        rooms = GameObject.FindGameObjectsWithTag("Room");
        Debug.Log("Total Rooms Detected: " + rooms.Length);
        activeRoomViews = new List<GameObject>();

        // Check if initialRoom is set
        if (initialRoom == null)
        {
            Debug.LogWarning("[GameManager] initialRoom not set!");

            // Running in the editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Running in a build
            Application.Quit();
        #endif
        }

        // Enable Active Room and disable all other rooms
        ToggleRooms(initialRoom);
        
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

    public void SetState(string key, bool value)
    {
        gameStates[key] = value;
        onGameStateChange?.Invoke(key);
    }

    public bool GetState(string key)
    {
        return gameStates.ContainsKey(key) && gameStates[key];
    }

    public void ToggleRooms(GameObject targetRoom)
    {
        foreach (GameObject room in rooms)
        {   
            room.SetActive(room == targetRoom);
        }
        
        activeRoom = targetRoom;
        currentViewIndex = 0;
        FetchViews(targetRoom);

        // Ensure only the first view is active immediately
        for (int i = 0; i < activeRoomViews.Count; i++)
        {
            activeRoomViews[i].SetActive(i == 0);
        }
        
        if (targetRoom.name == "Bathroom")
        {
            if(GetState("game_start"))
            {
                GameManager.Instance.SetState("water", true);
            }
        }
        if (targetRoom.name == "Office")
        {
            if(GetState("completed_tutorial"))
            {
                GameManager.Instance.SetState("see_boss", true);
            }
        }

    }

    private void FetchViews(GameObject room)
    {   
        // Clear previous views from list
        activeRoomViews.Clear();

        // A loop is used instead of GetComponentsInChildren, since GetComponentsInChildren is recursive and returns more than the first-depth children of the gameobject in the hierarchy
        foreach (Transform childTransform in room.transform) 
        {
            activeRoomViews.Add(childTransform.gameObject);
        }
        Debug.Log("View Count in " + activeRoom.gameObject.name + ": " + activeRoomViews.Count);
        
    }

    public void ChangeView()
    {
        if (activeRoomViews == null || activeRoomViews.Count == 0) return;

        // Disable the current view
        activeRoomViews[currentViewIndex].SetActive(false);
        // Increment index
        currentViewIndex = (currentViewIndex + 1) % activeRoomViews.Count;
        // Enable new view
        activeRoomViews[currentViewIndex].SetActive(true);
    }
}
