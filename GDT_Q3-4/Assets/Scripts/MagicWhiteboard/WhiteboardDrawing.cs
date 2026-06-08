using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class WhiteboardDrawing : MonoBehaviour
{

    [Header("Cameras")]
    [Tooltip("The orthographic camera hidden under the map.")]
    public Camera hiddenCamera; 

    [Header("Brush Settings")]
    [Tooltip("The Prefab with the TrailRenderer attached.")]
    public GameObject brushPrefab; 
    [Tooltip("Minimum pixels the mouse must move before recording a new point for the AI.")]
    public float pointMinDistance = 5f; 

    [Header("AI Data")]
    public List<Vector2> strokeData = new List<Vector2>();
    private DollarRecognizer recognizer;
    private int strokeDataMinThreshold = 10;
    [SerializeField] private float minScoreThreshold = 2f;

    private GameObject currentBrush;
    private List<GameObject> oldStrokes = new List<GameObject>();
    private GestureDatabase activeDatabase = new GestureDatabase();

    [SerializeField] private String currentSymbolLabel = "Triangle";
    [SerializeField] private bool isDrawingOnThisBoard = false;
    private void Start()
    {   
        recognizer = new DollarRecognizer();

        LoadGesturesFromDisk();

        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnDrawStart += HandleDrawStart;
            InputManager.Instance.OnDrawHold += HandleDrawHold;
            InputManager.Instance.OnDrawEnd += HandleDrawEnd;

            InputManager.Instance.OnDrawClear += ClearWhiteboard; // Temp until final implementation with clear button
            InputManager.Instance.OnTrainAI += DebugSaveCurrentStroke; // Temp used for saving patterns to be copied
            InputManager.Instance.OnDeleteAIDatabase += DeleteAllSavedGestures;
        }
        else
        {
            Debug.LogError("WhiteboardDrawing: InputManager.Instance is still null in Start!");
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnDrawStart -= HandleDrawStart;
            InputManager.Instance.OnDrawHold -= HandleDrawHold;
            InputManager.Instance.OnDrawEnd -= HandleDrawEnd;

            InputManager.Instance.OnDrawClear -= ClearWhiteboard; // Temp until final implementation with clear button
            InputManager.Instance.OnTrainAI -= DebugSaveCurrentStroke; // Temp used for saving patterns to be copied
            InputManager.Instance.OnDeleteAIDatabase -= DeleteAllSavedGestures;
        }
    }

    // --- DRAWING LOGIC ---

    private void HandleDrawStart(Vector2 mousePosition)
    {
        // Check if the mouse is actually pointing at THIS specific whiteboard
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject != this.gameObject)
            {
                return;
            }
            
            isDrawingOnThisBoard = true;        }
        else
        {
            return;
        } 
        
        strokeData.Clear();
        RecordPoint(mousePosition);

        Vector3 startWorldPos = GetHiddenCameraWorldPosition(mousePosition);
        
        currentBrush = Instantiate(brushPrefab, startWorldPos, Quaternion.identity);
        currentBrush.layer = LayerMask.NameToLayer("BrushStrokes");
        oldStrokes.Add(currentBrush);
    }

    private void HandleDrawHold(Vector2 mousePosition)
    {
        if (!isDrawingOnThisBoard) return;
        
        if (currentBrush != null)
        {
            currentBrush.transform.position = GetHiddenCameraWorldPosition(mousePosition);

            if (Vector2.Distance(strokeData[strokeData.Count - 1], mousePosition) > pointMinDistance)
            {
                RecordPoint(mousePosition);
            }
        }
    }

    private void HandleDrawEnd()
    {
        
        if (!isDrawingOnThisBoard) return;

        isDrawingOnThisBoard = false; // Reset Flag

        currentBrush = null; 

        if (strokeData.Count > strokeDataMinThreshold) 
        {
            // The AI automatically finds the single greatest match out of all saved symbols!
            DollarRecognizer.Result result = recognizer.Recognize(strokeData);

            if (result.Match != null)
            {
                Debug.Log($"AI thinks you drew a: {result.Match.Name} (Confidence: {result.Score})");

                // Only trigger game logic if the drawing is decent (above threshold)
                if (result.Score > minScoreThreshold)
                {
                    // Use a switch statement to trigger different puzzle logic based on the symbol!
                    switch (result.Match.Name)
                    {
                        case "Triangle":
                            Debug.Log("SUCCESS: You unlocked the Triangle door!");
                            // GameManager.Instance.UnlockTriangleDoor();
                            break;
                        
                        case "Square":
                            Debug.Log("SUCCESS: You activated the Square elevator!");
                            // GameManager.Instance.ActivateSquareElevator();
                            break;
                        
                        case "Circle":
                            Debug.Log("SUCCESS: You cast the Circle spell!");
                            break;

                        default:
                            Debug.Log($"You drew a valid shape ({result.Match.Name}), but no game logic is hooked up for it yet.");
                            break;
                    }
                }
                else
                {
                    Debug.Log("The drawing was too messy! The AI wasn't confident enough.");
                }
            }
            else
            {
                Debug.Log("Recognizer library is empty. No match found.");
            }
        }
    }

    // --- UTILITIES ---

    private void RecordPoint(Vector2 screenPosition)
    {
        strokeData.Add(screenPosition);
    }

    private Vector3 GetHiddenCameraWorldPosition(Vector2 screenPosition)
    {
        // Shoot a ray from the Main Game Camera to the 3D Whiteboard
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        
        // Get the exact UV coordinate (0.0 to 1.0) of where the laser hit the texture
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2 uv = hit.textureCoord;

            // Translate that UV coordinate into the Hidden Camera's orthographic space
            float ortho = hiddenCamera.orthographicSize;
            float aspect = hiddenCamera.aspect; // Usually 1.0 for a square RenderTexture
            
            float localX = (uv.x - 0.5f) * (ortho * 2f * aspect);
            float localY = (uv.y - 0.5f) * (ortho * 2f);

            // Place the brush exactly in front of the hidden camera based on those offsets
            Vector3 finalPos = hiddenCamera.transform.position + 
                               (hiddenCamera.transform.right * localX) + 
                               (hiddenCamera.transform.up * localY) + 
                               (hiddenCamera.transform.forward * (hiddenCamera.nearClipPlane + 1f));

            return finalPos;
        }

        // Fallback if the mouse dragged off the edge of the board
        return hiddenCamera.transform.position; 
    }
    
    // TO-DO Implement Button to use this method when on the specific whiteboard view
    public void ClearWhiteboard()
    {
        foreach (GameObject stroke in oldStrokes)
        {
            Destroy(stroke);
        }
        
        strokeData.Clear();
    }

    // --- SAVE & LOAD SYSTEM ---

    public void DebugSaveCurrentStroke()
    {
        if (strokeData.Count > strokeDataMinThreshold)
        {
            List<Vector2> copiedPoints = new List<Vector2>(strokeData);
            
            // Train the recognizer's brain
            recognizer.SavePattern(currentSymbolLabel, copiedPoints);
            
            // Add it to our master list so it doesn't get lost
            activeDatabase.allGestures.Add(new SavedGesture(currentSymbolLabel, copiedPoints));
            
            Debug.Log($"Successfully taught the AI what a '{currentSymbolLabel}' looks like!");
            SaveGesturesToDisk();
        }
    }


    private string GetGesturesFilePath()
    {
        // Application.streamingAssetsPath automatically points to the correct 
        // StreamingAssets folder in BOTH the Unity Editor and the Built Game.
        return Path.Combine(Application.streamingAssetsPath, "AI_Gestures.json");
    }

    public void SaveGesturesToDisk()
    {
        string json = JsonUtility.ToJson(activeDatabase, true);
        
        // Ensure the StreamingAssets directory exists before saving (mostly for Editor safety)
        string directory = Path.GetDirectoryName(GetGesturesFilePath());
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(GetGesturesFilePath(), json);
        Debug.Log($"Saved gestures to: {GetGesturesFilePath()}");
    }

    public void LoadGesturesFromDisk()
    {
        string filePath = GetGesturesFilePath();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            activeDatabase = JsonUtility.FromJson<GestureDatabase>(json);

            if (activeDatabase != null && activeDatabase.allGestures != null)
            {
                foreach (SavedGesture gesture in activeDatabase.allGestures)
                {
                    recognizer.SavePattern(gesture.gestureName, gesture.points);
                }
                Debug.Log($"Loaded {activeDatabase.allGestures.Count} gestures from StreamingAssets!");
            }
        }
        else
        {
            Debug.LogWarning($"No gesture file found at {filePath}. AI is starting empty.");
        }
    }

    public void DeleteAllSavedGestures()
    {
        string filePath = GetGesturesFilePath();

        // Delete the physical file from the hard drive
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Deleted AI_Gestures.json from the hard drive.");
        }
        else
        {
            Debug.Log("No save file found to delete.");
        }

        // Wipe our master list in memory
        activeDatabase = new GestureDatabase();

        // Wipe the AI's brain by completely replacing it with a fresh, empty one
        recognizer = new DollarRecognizer();

        Debug.Log("AI memory has been completely wiped!");
    }
}