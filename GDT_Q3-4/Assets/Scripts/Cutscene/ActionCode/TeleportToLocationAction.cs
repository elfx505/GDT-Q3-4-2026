using System.Collections;
using UnityEngine;


public class TeleportToLocationActionSO : CutsceneAction
{
    [SerializeField] private GameObject targetPosition;
    [SerializeField] private GameObject objectToMove;
    public override IEnumerator Play(CutsceneContext context)
    {
        Debug.Log("TELEPORTING OBJECT");

        objectToMove.transform.position = targetPosition.transform.position;
        yield break;
    }
}
