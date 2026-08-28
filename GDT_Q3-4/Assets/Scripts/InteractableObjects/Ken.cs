using System;
using UnityEngine;

public class Ken : InteractableObject
{
    [SerializeField] private CameraAnchor introAnchor;
    [TextArea(3, 5)]
    [SerializeField] private String dialogueWhenFar;
    private bool gaveIntroduction = false;
    private int documentedReceived = 0;
    protected override void PerformAction()
    {
        base.PerformAction();
        if (!gaveIntroduction)
        {
            if (GameManager.Instance.CurrentAnchor == introAnchor)
            {
                GiveIntro();
                gaveIntroduction = true;
            }
            else
            {
                GameTextController.Instance.HandleDialogue(dialogueWhenFar);
            }
        }
        else
        {

            if (!GameManager.Instance.GetState(GameState.ComputerIntroDone))
            {
                GameTextController.Instance.HandleDialogue("Ken: Check out your computer and complete the onboarding!");
            }
            else
            {
                GameTextController.Instance.HandleDialogue("Ken: Hand me the document");
            }
        }
    }

    private void GiveIntro()
    {
        GameManager.Instance.SetState(GameState.KenGreeted, true);
    }

    public void ReceiveDocument()
    {
        documentedReceived++;
        switch (documentedReceived)
        {
            case 1:
                GameManager.Instance.SetState(GameState.DocGiven1, true);
                break;
            case 2:
                GameManager.Instance.SetState(GameState.DocGiven2, true);
                break;
            case 3:
                GameManager.Instance.SetState(GameState.DocGiven3, true);
                break;
            default:
                Debug.LogWarning("[KEN]: SOMEHOW RECEIVED WEIRD NUMBER OF DOCS");
                break;
        }
    }
}
