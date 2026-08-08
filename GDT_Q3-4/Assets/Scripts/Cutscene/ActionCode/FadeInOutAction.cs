using System.Collections;
using UnityEngine;

public class FadeInOutAction : CutsceneAction
{
    [SerializeField] private float timeBetweenFade;
    [SerializeField] private bool useTime = true;
    [SerializeField] private bool fadeOutAction = false;
    public override IEnumerator Play(CutsceneContext context)
    {
        Debug.Log("Fade Action");
        if (fadeOutAction)
        {
            yield return StartCoroutine(CameraManager.Instance.FadeOut());
            yield break;
        }

        yield return StartCoroutine(CameraManager.Instance.FadeIn());
        if (useTime)
        {
            yield return new WaitForSeconds(timeBetweenFade);
            yield return StartCoroutine(CameraManager.Instance.FadeOut());
        }

    }
}

