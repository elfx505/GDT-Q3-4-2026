using System.Collections;
using UnityEngine;

public class PlaySoundAction : CutsceneAction
{
    [SerializeField] private AudioClip audioClip;

    public override IEnumerator Play(CutsceneContext context)
    {

        Debug.Log("Sound Playing Actions");
        Debug.Log("Sound time: " + audioClip.length);
        AudioManager.Instance.PlaySFX(audioClip);
        yield return new WaitForSeconds(audioClip.length);
    }

}
