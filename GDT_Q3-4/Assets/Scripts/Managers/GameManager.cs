using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    
    GameObject[] rooms;
    [SerializeField] private GameObject initialRoom;
    private GameObject activeRoom;
    
    protected override void Awake()
    {
        base.Awake();

        // Get all Room Objects in the Scene
        rooms = GameObject.FindGameObjectsWithTag("Room");
        Debug.Log("Total Rooms Detected: " + rooms.Length);

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

    public void ToggleRooms(GameObject targetRoom)
    {
        foreach (GameObject room in rooms)
        {   
            room.SetActive(false);
            if (room == targetRoom) room.SetActive(true);
            activeRoom = targetRoom;
        }
        
    }
}
