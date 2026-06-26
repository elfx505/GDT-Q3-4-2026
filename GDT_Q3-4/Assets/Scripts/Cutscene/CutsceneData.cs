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


public class CutsceneContext
{
    public Camera MainCamera;
    public List<Transform> focusTargets;
    private int currentFocusTarget = 0;

    public CutsceneContext(Camera camera, List<Transform> targets)
    {
        MainCamera = camera;
        focusTargets = targets;
    }

    public Transform GetCurrentFocusTarget()
    {
        if (currentFocusTarget < focusTargets.Count)
        {
            Transform result = focusTargets[currentFocusTarget];
            currentFocusTarget++;
            return result;
        }
        Debug.LogError("Missing focus target");
        return null;
    }
}