using System;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BathroomStall : InteractableObject
{
    [SerializeField] private ItemSO hintItem;
    [TextArea(3, 5)]
    [SerializeField] private string earlyInteractionText;
    protected override void PerformAction()
    {
        base.PerformAction();

        if (GameManager.Instance.GetState(GameState.ResignationPapersPrinted))
        {
            SetState();
            GivePlayerHint();
        }
        else
        {
            GameTextController.Instance.HandleDialogue(earlyInteractionText);
        }
    }

    private void SetState()
    {
        GameManager.Instance.SetState(GameState.JanitorSpokenTo, true);
    }

    private void GivePlayerHint()
    {
        if (hintItem != null) InventoryManager.Instance.AddItem(hintItem);
    }
}
