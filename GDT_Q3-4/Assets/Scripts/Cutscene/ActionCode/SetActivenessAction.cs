using System.Collections;
using UnityEngine;
using UnityEngine.WSA;

public class SetActivenessAction : CutsceneAction
{
    [SerializeField] private bool toActivate = true;
    [SerializeField] private GameObject targetObject;
    public override IEnumerator Play(CutsceneContext context)
    {
        if (toActivate)
        {
            targetObject.SetActive(true);
        }
        else
        {
            targetObject.SetActive(false);

        }
        yield return null;
    }
}
