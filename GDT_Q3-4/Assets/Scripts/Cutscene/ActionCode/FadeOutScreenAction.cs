using System.Collections;
using UnityEngine;

public class FadeOutScreenAction : CutsceneAction
{
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float waitDelay;

    public override IEnumerator Play(CutsceneContext context)
    {
        Debug.Log("Fade Out Screen Action");
        yield return StartCoroutine(CameraManager.Instance.FadeIn(fadeInDuration));

        yield return new WaitForSecondsRealtime(waitDelay);
    }
}
