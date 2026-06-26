using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Cutscenes/Actions/Image With Dialogue")]
public class ImageDialogueActionSO : CutsceneActionSO
{
    // public List<Sprite> image = new();

    // public List<float> displayDuration = new();

    // [TextArea]
    // public List<string> dialogue;

    public List<ImageDialogueData> data = new();

    public override IEnumerator Play(CutsceneContext context)
    {
        // if (image.Count <= 0 || displayDuration.Count <= 0 || dialogue.Count <= 0)
        // {
        //     yield break;
        // }
        if (data.Count <= 0)
        {
            yield break;
        }
        // for (int i = 0; i < image.Count; i++)
        // {

        //     CutsceneUI.Instance.ShowImage(image[i]);

        //     yield return new WaitForSeconds(displayDuration[i]);

        //     if (!string.IsNullOrWhiteSpace(dialogue[i]))
        //     {
        //         GameTextController.Instance.HandleDialogue(dialogue[i]);

        //         // bool clicked = false;

        //         // while (!clicked)
        //         // {
        //         //     if (Input.GetMouseButtonDown(0))
        //         //         clicked = true;

        //         //     yield return null;
        //         // }
        //         yield return new WaitUntil(() =>
        //             !GameManager.Instance.textOnScreen);
        //     }

        // }
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