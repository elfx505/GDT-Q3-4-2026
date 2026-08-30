using System.Collections;
using UnityEngine;

public class FadeInScreenAction : CutsceneAction
{
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private float waitDelay;

    public override IEnumerator Play(CutsceneContext context)
    {
        Debug.Log("Fade In Screen Action");

        yield return new WaitForSecondsRealtime(waitDelay);

        yield return StartCoroutine(CameraManager.Instance.FadeOut(fadeOutDuration));

    }
}

