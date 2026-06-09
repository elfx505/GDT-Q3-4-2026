using UnityEngine;

public class ResignationPaper : CollectibleItem
{
    protected override void PerformAction()
    {
        base.PerformAction();

        GameManager.Instance.SetState(GameState.ResignationPapersPrinted, true);
    }
}
