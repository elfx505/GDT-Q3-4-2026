using UnityEngine;

public class ViewerObjective : InteractableObject
{
    [SerializeField] private ItemSO viewObject;
    protected override void PerformAction()
    {
        base.PerformAction();
        ItemViewer.Instance.ShowItem(viewObject);
    }
}
