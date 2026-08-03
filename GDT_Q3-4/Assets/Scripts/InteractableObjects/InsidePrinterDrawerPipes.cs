using UnityEngine;

public class InsidePrinterDrawerPipes : ZoomInObjective
{   

    [SerializeField] private int drawerIndex; // Must be specified in Inspector

    protected override void PerformAction()
    {
        base.PerformAction();

        PipeGridManager.Instance.LoadLevel(drawerIndex);

    }
}
