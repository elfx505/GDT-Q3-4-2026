using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Toolbars;
using UnityEngine;

public class Phone : InteractableObject
{
    [SerializeField] private string animationFingerName;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip fingerSnap;
    [SerializeField] private AudioClip phoneDial;
    [SerializeField] private AudioClip phoneDialCancel;
    [SerializeField] private SpriteRenderer hand;
    private bool canDial = false;
    private bool dialedOnce = false;

    protected override void Awake()
    {
        base.Awake();
        hand.enabled = false;
    }
    protected override void PerformAction()
    {
        base.PerformAction();


        if (!canDial)
        {
            StartCoroutine(BossInterrupt());
            return;
        }
        if (!dialedOnce)
        {
            StartCoroutine(SnapFinger());
        }
        else
        {
            ProperDial();
        }

    }

    public void UnlockDial()
    {
        GameManager.Instance.SetState(GameState.RatTrapPlaced, true);
        canDial = true;
    }

    public IEnumerator BossInterrupt()
    {
        AudioManager.Instance.PlaySFX(phoneDial, 1, 1, 0, 1);
        hand.enabled = true;
        animator.SetFloat("Speed", 1f);
        animator.Play(animationFingerName, 0, 0f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
        AudioManager.Instance.PlaySFX(phoneDialCancel);
        animator.SetFloat("Speed", -1f);
        animator.Play(animationFingerName, 0, 1f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0);
        hand.enabled = false;
        yield return null;

    }
    // TODO: use proper sound
    public IEnumerator SnapFinger()
    {
        AudioManager.Instance.PlaySFX(phoneDial, 1, 1, 0, 1);
        hand.enabled = true;
        animator.SetFloat("Speed", 1f);
        animator.Play(animationFingerName, 0, 0f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
        AudioManager.Instance.PlaySFX(fingerSnap);
        animator.SetFloat("Speed", -3f);
        animator.Play(animationFingerName, 0, 1f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0);
        hand.enabled = false;
        dialedOnce = true;
    }

    private void ProperDial()
    {
        GameManager.Instance.SetState(GameState.NumberDialed, true);
    }
}
