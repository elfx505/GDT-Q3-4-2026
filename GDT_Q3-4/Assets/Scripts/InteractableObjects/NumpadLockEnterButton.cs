using UnityEngine;

public class NumpadLockEnterButton : InteractableObject
{
    [SerializeField] private NumpadLock numpadLock;
    [Header("Error and Success SFX")]
    [SerializeField] private AudioClip successSFX;
    [SerializeField] private AudioClip errorSFX;


    protected override void Awake()
    {
        base.Awake();
        
        numpadLock = GetComponentInParent<NumpadLock>();
    }

    protected override void PerformAction()
    {
        base.PerformAction();

        bool result = numpadLock.CheckExistingInput();

        if (successSFX == null || errorSFX == null) return;

        AudioClip sfx = result ? successSFX : errorSFX;

        float startTime = result ? 1f : 0.5f; // success : error

        float endTime = result ? 2.2f : 1.5f;

        AudioManager.Instance.PlaySFX(sfx, startTime: startTime, endTime: endTime);
    }
}
