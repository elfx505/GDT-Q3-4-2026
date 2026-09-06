using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public string cutsceneID;

    public List<CutsceneAction> actions = new();
    public bool hasBackgroundMusic;
    public float musicFadeDuration = 1f;
}