using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDialogueActionSO : CutsceneAction
{
    public List<ImageDialogueData> data = new();

    public override IEnumerator Play(CutsceneContext context)
    {

        if (data.Count <= 0)
        {
            yield break;
        }

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].image)
            {
                CutsceneUI.Instance.ShowImage(data[i].image);
                yield return new WaitForSeconds(data[i].displayDuration);

            }


            if (!string.IsNullOrWhiteSpace(data[i].dialogue))
            {
                GameTextController.Instance.HandleDialogue(data[i].dialogue);

                yield return new WaitUntil(() =>
                    !GameTextController.Instance.IsShowing);
            }
            CutsceneUI.Instance.HideAll();
        }
    }
}
[Serializable]
public class ImageDialogueData
{
    public Sprite image;
    public float displayDuration = 2f;
    [TextArea(3, 5)]
    public string dialogue;
}