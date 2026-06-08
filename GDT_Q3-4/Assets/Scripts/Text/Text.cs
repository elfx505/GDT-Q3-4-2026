using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameTextController : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject panel;
    public TextMeshProUGUI textUI;

    [Header("State Dialogues Mapping")]
    [TextArea(3, 5)] public List<string> stateText;
    public List<GameState> keys;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowing = false;


    // ADD THIS METHOD:
    private void Awake()
    {
        // Force the panel to hide itself when the game first launches
        if (panel != null)
        {
            panel.SetActive(false);
        }
        isShowing = false;
    }

    private void OnEnable()
    {
        GameManager.onGameStateChange += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.onGameStateChange -= HandleGameStateChange;
    }

    private void Update()
    {
        // Only progress text if the panel is currently showing
        if (isShowing && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ShowNextMessage();
        }
    }

    private List<string> sections(string s) 
    {
        if (string.IsNullOrEmpty(s)) return new List<string>();

        string[] delimiters = {"[S]"};
        
        // FIX: Added 'RemoveEmptyEntries' to automatically delete accidental 
        // blank spaces, double [S][S] tags, or trailing delimiters.
        List<string> dialogueList = s
            .Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim()) // Cleans up accidental leading/trailing spaces or \n
            .Where(line => !string.IsNullOrEmpty(line)) // Double check filtering
            .ToList();

        return dialogueList;
    }

    private void HandleGameStateChange(GameState key)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (key == keys[i])
            {
                // Ensure we don't grab an out-of-bounds element if lists don't match size
                if (i < stateText.Count)
                {
                    StartSequence(sections(stateText[i]));
                }
                break;
            }
        }
    }

    public void StartSequence(List<string> messages)
    {
        // Safety check: if no valid messages were extracted, don't even open the panel
        if (messages == null || messages.Count == 0) return;

        messageQueue.Clear();

        foreach (string msg in messages)
        {
            messageQueue.Enqueue(msg);
        }

        panel.SetActive(true);
        isShowing = true;

        // Display the very first message immediately
        ShowNextMessage();
    }

    private void ShowNextMessage()
    {
        // If the queue is empty, clicking now means the user is ready to close the panel
        if (messageQueue.Count == 0)
        {
            EndSequence();
            return;
        }

        // Pull the next line out of the queue and display it
        textUI.text = messageQueue.Dequeue();
    }

    private void EndSequence()
    {
        panel.SetActive(false);
        isShowing = false;
        textUI.text = ""; // Clear the UI string to reset cleanly
    }
}