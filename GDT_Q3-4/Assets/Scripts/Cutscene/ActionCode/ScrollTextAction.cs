using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScrollTextAction : CutsceneAction
{
    [SerializeField] private TextMeshProUGUI textToScroll;
    [SerializeField] private float scrollToPositions;
    [SerializeField] private float speed;
    public override IEnumerator Play(CutsceneContext context)
    {
        float currentPosition = textToScroll.rectTransform.anchoredPosition.y;
        float moveDirection = (scrollToPositions - currentPosition) > 0 ? 1f : -1f;

        while (Math.Abs(currentPosition - scrollToPositions) > 5f)
        {
            Debug.Log(textToScroll.rectTransform.anchoredPosition.y);
            currentPosition += speed * moveDirection * Time.deltaTime;

            Vector2 position = textToScroll.rectTransform.anchoredPosition;
            position.y = currentPosition;
            textToScroll.rectTransform.anchoredPosition = position;
            yield return null;
        }
    }
}
