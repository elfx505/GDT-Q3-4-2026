using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GenericFadeAction : CutsceneAction
{
    [SerializeField] private bool fadeSolid;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float fadeTime;
    private List<ComponentFader> faders = new List<ComponentFader>();

    // A lightweight helper class to handle different component types seamlessly
    private abstract class ComponentFader
    {
        public float startAlpha;
        public abstract void SetAlpha(float alpha);
    }

    private class SpriteFader : ComponentFader
    {
        private SpriteRenderer sr;
        public SpriteFader(SpriteRenderer sr) { this.sr = sr; startAlpha = sr.color.a; }
        public override void SetAlpha(float alpha) { Color c = sr.color; c.a = alpha; sr.color = c; }
    }

    private class TMPFader : ComponentFader
    {
        private TMP_Text txt;
        public TMPFader(TMP_Text txt) { this.txt = txt; startAlpha = txt.color.a; }
        public override void SetAlpha(float alpha) { Color c = txt.color; c.a = alpha; txt.color = c; }
    }

    private class ImageFader : ComponentFader
    {
        private Image targetImage;
        public ImageFader(Image image)
        {
            this.targetImage = image;
            startAlpha = image.color.a;
        }

        public override void SetAlpha(float alpha)
        {
            Color c = targetImage.color;
            c.a = alpha;
            targetImage.color = c;
        }
    }


    public override IEnumerator Play(CutsceneContext context)
    {
        // 1. Gather components and current alphas at the exact moment the fade begins.
        // Clearing ensures we don't duplicate entries if this action is run multiple times.
        faders.Clear();

        // Passing 'true' guarantees it finds components even on disabled child GameObjects
        foreach (var sr in targetObject.GetComponentsInChildren<SpriteRenderer>(true))
            faders.Add(new SpriteFader(sr));

        foreach (var txt in targetObject.GetComponentsInChildren<TMP_Text>(true))
            faders.Add(new TMPFader(txt));

        foreach (var image in targetObject.GetComponentsInChildren<Image>(true))
            faders.Add(new ImageFader(image));

        float elapsedTime = 0f;
        float targetAlpha = fadeSolid ? 1f : 0f;

        // 2. Fade them all over time
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;

            // Mathf.Clamp01 ensures our interpolation fraction never exceeds 100%
            float t = Mathf.Clamp01(elapsedTime / fadeTime);

            foreach (var fader in faders)
            {
                float currentAlpha = Mathf.Lerp(fader.startAlpha, targetAlpha, t);
                fader.SetAlpha(currentAlpha);
            }
            yield return null;
        }

        // 3. Guarantee the final alpha state is exact when the loop ends
        foreach (var fader in faders)
        {
            fader.SetAlpha(targetAlpha);
        }
    }
}