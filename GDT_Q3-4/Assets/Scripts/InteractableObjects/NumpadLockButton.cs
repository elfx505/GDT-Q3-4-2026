using UnityEngine;

public class NumpadLockButton : InteractableObject
{   
    [SerializeField] private NumpadLock numpadLock;
    private int buttonValue;

    void Awake()
    {
        if (int.TryParse(gameObject.name, out int result))
        {
            buttonValue = result;

        } else
        {
            Debug.LogWarning($"Failed to Convert Button Name to Button Value for {gameObject.name}!");
        }

        numpadLock = GetComponentInParent<NumpadLock>();
    }


    protected override void PerformAction()
    {
        base.PerformAction();

        numpadLock.RegisterInput(buttonValue);

    }
}
