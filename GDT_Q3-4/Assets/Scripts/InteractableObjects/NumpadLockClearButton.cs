using UnityEngine;

public class NumpadLockClearButton : InteractableObject
{   
    [SerializeField] private NumpadLock numpadLock;

    private void Awake()
    {
        numpadLock = GetComponentInParent<NumpadLock>();
    }

    protected override void PerformAction()
    {
        base.PerformAction();

        numpadLock.ClearInput();
    }
}
