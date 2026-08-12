using UnityEngine;

public class NumpadLockEnterButton : InteractableObject
{
    [SerializeField] private NumpadLock numpadLock;

    protected override void Awake()
    {
        base.Awake();
        
        numpadLock = GetComponentInParent<NumpadLock>();
    }

    protected override void PerformAction()
    {
        base.PerformAction();

        numpadLock.CheckExistingInput();
    }
}
