using UnityEngine;

public class GasolineHole : InteractableObject
{
    protected override void PerformAction()
    {
        base.PerformAction();

    }

    public void PrintPaper()
    {   
        if (GameManager.Instance.GetState(GameState.AllPipePuzzlesCompleted))
        {
            GameManager.Instance.SetState(GameState.PrinterFueled, true);
        }

        // Else Play Default Dialogue for the previously completed GameState
    }
}
