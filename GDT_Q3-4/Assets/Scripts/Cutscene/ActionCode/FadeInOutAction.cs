using System.Collections;
using UnityEngine;

public class FadeInOutAction : CutsceneAction
{
    [SerializeField] private float timeBetweenFade;
    public override IEnumerator Play(CutsceneContext context)
    {
        Debug.Log("Fade Action");
        yield return StartCoroutine(CameraManager.Instance.FadeIn());
        yield return new WaitForSeconds(timeBetweenFade);
        yield return StartCoroutine(CameraManager.Instance.FadeOut());

    }
}

