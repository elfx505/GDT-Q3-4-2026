using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscenes/Cutscene")]
public class Cutscene : ScriptableObject
{
    public string cutsceneID;

    public List<CutsceneActionSO> actions = new();
}