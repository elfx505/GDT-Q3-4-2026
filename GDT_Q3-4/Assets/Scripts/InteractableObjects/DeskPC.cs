using Unity.VisualScripting;
using UnityEngine;

public class DeskPC : InteractableObject
{
    private bool introDone = false;
    protected override void PerformAction()
    {
        base.PerformAction();
        if (GameManager.Instance.GetState(GameState.KenGreeted) && !introDone)
        {
            SetState();
            introDone = true;
        }

    }

    private void SetState()
    {

        GameManager.Instance.SetState(GameState.ComputerIntroDone, true);
    }
}
