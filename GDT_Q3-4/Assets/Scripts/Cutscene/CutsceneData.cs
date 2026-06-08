using System;
using System.Collections.Generic;
using UnityEngine;

// A single synchronized scene event
[Serializable]
public struct CutsceneEvent
{
    [Header("Visuals & Timing")]
    public Sprite backgroundSprite;
    public float displayDuration; // How long the image shows BEFORE the text appears

    [Header("Dialogue (Optional)")]
    [TextArea(3, 5)]
    public string dialogueText; // If empty, the scene might skip the click step
}

// The overall list of events
[Serializable]
public class CutsceneData
{
    // The main list we use to structure the cutscene
    public List<CutsceneEvent> orderedEvents;
}