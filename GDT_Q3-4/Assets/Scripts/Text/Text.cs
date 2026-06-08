using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameTextController  : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI textUI;
    public List<string> stateText;
    public List<GameState> keys;
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

    private List<string> sections(string s) {
        string[] delimiters = {"[S]"};
        List<string> dialogueList = s
                    .Split(delimiters, StringSplitOptions.None)
                    .ToList();
        return dialogueList;
    }


    private void HandleGameStateChange(GameState key)
    {
        for (int i = 0; i < keys.Count; i ++)
        {
            if (key == keys[i])
            {
                StartSequence(sections(stateText[i]));
                break;
            }
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