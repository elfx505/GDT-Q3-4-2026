using UnityEngine;
using System.Collections;

public abstract class CutsceneAction : ScriptableObject
{
    public bool runInParallel = false;

    public abstract IEnumerator Play();
}