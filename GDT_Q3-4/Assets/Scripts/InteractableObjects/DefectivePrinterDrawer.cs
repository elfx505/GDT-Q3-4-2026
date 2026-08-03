using UnityEngine;

public class DefectivePrinterDrawer : InteractableObject
{
    [SerializeField] private int drawerIndex;

    private bool isOpen = false;

    protected override void PerformAction()
    {
        base.PerformAction();

        if (PrinterPuzzleManager.Instance == null) {
            Debug.LogWarning($"[DefectivePrinterDrawer] <{gameObject.name}> PrinterPuzzleManager Instance is null!");
            return;
        }

        PrinterPuzzleManager.Instance.OpenDrawer(drawerIndex);
    }
}
