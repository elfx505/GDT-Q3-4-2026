using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameTextController : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI textUI;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowing = false;

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
        if (isShowing && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ShowNextMessage();
        }
    }

    private void HandleGameStateChange(string key)
    {
        switch (key)
        {
            case "game_start":
                StartSequence(new List<string>
                {
                    "\n Click to continue...",
                    "You: \n Another day in the cursed company.",
                    "You: \n I need to take a bothroom break."
                });
                break;

            case "water":
                StartSequence(new List<string>
                {
                    "You: \n What the ...",
                    "You: \n I need something that can absorb water. ",
                    "\n (Press E to open inventory)"
                });
                break;

            case "see_boss":
                StartSequence(new List<string>
                {
                    "Boss: \n Well, well, well!",
                    "Boss: \n I\'ve been waiting for you. Please, take your seat. "
                });
                break;
        }
    }

    public void StartSequence(List<string> messages)
    {
        messageQueue.Clear();

        foreach (string msg in messages)
        {
            messageQueue.Enqueue(msg);
        }

        panel.SetActive(true);
        isShowing = true;

        ShowNextMessage();
    }

    private void ShowNextMessage()
    {
        if (messageQueue.Count == 0)
        {
            EndSequence();
            return;
        }

        textUI.text = messageQueue.Dequeue();
    }

    private void EndSequence()
    {
        panel.SetActive(false);
        isShowing = false;
    }
}