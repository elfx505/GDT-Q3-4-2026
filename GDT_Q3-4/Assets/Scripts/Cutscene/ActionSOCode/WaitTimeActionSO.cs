using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscenes/Actions/Wait For Seconds")]
public class WaitTimeActionSO : CutsceneActionSO
{
    [SerializeField] private float waitSeconds;

    public override IEnumerator Play(CutsceneContext context)
    {
        Debug.Log("Waiting for " + waitSeconds + " seconds");
        yield return new WaitForSeconds(waitSeconds);
    }
}
