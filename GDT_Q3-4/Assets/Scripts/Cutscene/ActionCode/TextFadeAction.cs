using System.Collections;
using TMPro;
using UnityEngine;

public class TextFadeAction : CutsceneAction
{
    [SerializeField] private bool fadeSolid;
    [SerializeField] private TextMeshPro textToFade;
    [SerializeField] private float fadeTime;
    private float startAlpha;
    public override IEnumerator Play(CutsceneContext context)
    {
        float elapsedTime = 0f;
        Color textColor = textToFade.color;
        startAlpha = textColor.a;
        if (fadeSolid)
        {
            while (elapsedTime < fadeTime && textToFade.color.a <= 1f)
            {
                elapsedTime += Time.deltaTime;
                textColor.a = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeTime);
                textToFade.color = textColor;
                yield return null;
            }
            textColor.a = 1f;
            textToFade.color = textColor;
        }
        else
        {
            while (elapsedTime < fadeTime && textToFade.color.a > 0f)
            {
                elapsedTime += Time.deltaTime;
                textColor.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeTime);
                textToFade.color = textColor;
                Debug.Log(textToFade.color);

                yield return null;
            }
            textColor.a = 0f;
            textToFade.color = textColor;
        }
    }
}
