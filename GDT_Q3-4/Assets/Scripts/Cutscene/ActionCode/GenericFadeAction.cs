using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenericFadeAction : CutsceneAction
{
    [SerializeField] private bool fadeSolid;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float fadeTime;

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

    public override IEnumerator Play(CutsceneContext context)
    {
        List<ComponentFader> faders = new List<ComponentFader>();

        // 1. Gather all SpriteRenderers and TMP_Text components on this object AND its children
        foreach (var sr in targetObject.GetComponentsInChildren<SpriteRenderer>()) 
            faders.Add(new SpriteFader(sr));
            
        foreach (var txt in targetObject.GetComponentsInChildren<TMP_Text>()) 
            faders.Add(new TMPFader(txt));

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