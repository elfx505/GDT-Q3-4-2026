using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Cutscene/Actions/Dialogue")]
public class DialogueAction : CutsceneAction
{
    [TextArea]
    public string dialogue;

    public override IEnumerator Play()
    {
        DialogueUI.Instance.Show(dialogue);

        yield return new WaitUntil(() => DialogueUI.Instance.IsComplete);
    }
}