using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Cutscene")]
public class Cutscene : ScriptableObject
{
    public string cutsceneID;
    public List<CutsceneAction> actions;
}