using System.Collections;
using UnityEngine;

public abstract class CutsceneActionSO : ScriptableObject
{
    public bool runInParallel;

    public abstract IEnumerator Play(CutsceneContext context);
}