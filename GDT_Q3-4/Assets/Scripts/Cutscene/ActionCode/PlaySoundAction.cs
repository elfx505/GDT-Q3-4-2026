using System.Collections;
using UnityEngine;

public class PlaySoundAction : CutsceneAction
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float volume = 1;
    [SerializeField] private float pitch = 1;
    [SerializeField] private float startTime = 0;
    [SerializeField] private float endTime = -1;

    public override IEnumerator Play(CutsceneContext context)
    {
        AudioManager.Instance.PlaySFX(audioClip, volume, pitch, startTime, endTime);
        if (endTime > audioClip.length)
        {
            Debug.LogError("[PlaySoundSoundAction] start or end time not good");
            yield return null;
        }
        yield return new WaitForSeconds(endTime - startTime);
    }

}
