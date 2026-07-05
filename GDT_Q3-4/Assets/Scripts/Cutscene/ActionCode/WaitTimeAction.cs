using System.Collections;
using UnityEngine;


public class WaitTimeAction : CutsceneAction
{
    [SerializeField] private float waitSeconds;

    public override IEnumerator Play(CutsceneContext context)
    {
        Debug.Log("Waiting for " + waitSeconds + " seconds");
        yield return new WaitForSeconds(waitSeconds);
    }
}
