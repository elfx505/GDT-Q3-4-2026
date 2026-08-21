using System.Collections;
using UnityEngine;

public class FadeInOutAction : CutsceneAction
{
    [SerializeField] private float timeBetweenFade;
    [SerializeField] private bool fadeOutAction = false;
    [SerializeField] private bool useTime = true;
    [SerializeField] private float fadeInDuration = 0.15f;
    [SerializeField] private float fadeOutDuration = 0.15f;
    public override IEnumerator Play(CutsceneContext context)
    {
        Debug.Log("Fade Action");
        if (fadeOutAction)
        {
            yield return StartCoroutine(CameraManager.Instance.FadeOut(fadeOutDuration));
            yield break;
        }

        yield return StartCoroutine(CameraManager.Instance.FadeIn(fadeInDuration));
        if (useTime && !fadeOutAction)
        {
            yield return new WaitForSeconds(timeBetweenFade);
            yield return StartCoroutine(CameraManager.Instance.FadeOut(fadeOutDuration));
        }

    }
}

