using System.Collections;
using UnityEngine;

public abstract class CutsceneAction : MonoBehaviour
{
    public bool runInParallel;

    public abstract IEnumerator Play(CutsceneContext context);
}