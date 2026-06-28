using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public string cutsceneID;

    public List<CutsceneAction> actions = new();
}