using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SavedGesture
{
    public string gestureName;
    public List<Vector2> points;

    public SavedGesture(string name, List<Vector2> pts)
    {
        gestureName = name;
        points = pts;
    }
}

[System.Serializable]
public class GestureDatabase
{
    public List<SavedGesture> allGestures = new List<SavedGesture>();
}