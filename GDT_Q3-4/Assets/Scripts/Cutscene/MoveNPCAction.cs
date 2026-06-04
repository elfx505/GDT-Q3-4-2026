using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Cutscene/Actions/Move NPC")]
public class MoveNPCAction : CutsceneAction
{
    public NPCController npc;
    public Transform target;
    public float speed = 2f;

    public override IEnumerator Play()
    {
        while (Vector3.Distance(npc.transform.position, target.position) > 0.1f)
        {
            npc.transform.position = Vector3.MoveTowards(
                npc.transform.position,
                target.position,
                speed * Time.deltaTime
            );

            yield return null;
        }
    }
}