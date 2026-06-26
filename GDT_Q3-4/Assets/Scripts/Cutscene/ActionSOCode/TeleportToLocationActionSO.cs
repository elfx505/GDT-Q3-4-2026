using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Cutscenes/Actions/Teleport To Location")]
public class TeleportToLocationActionSO : CutsceneActionSO
{
    [SerializeField] private GameObject targetPosition;
    [SerializeField] private GameObject objectToMove;
    public override IEnumerator Play(CutsceneContext context)
    {
        objectToMove.transform.position = targetPosition.transform.position;
        yield break;
    }
}
