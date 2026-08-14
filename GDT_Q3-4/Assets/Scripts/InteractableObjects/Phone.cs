using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Toolbars;
using UnityEngine;

public class Phone : InteractableObject
{
    [SerializeField] private string animationFingerName;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip fingerSnap;
    [SerializeField] private SpriteRenderer hand;
    private bool canDial = false;

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

        StartCoroutine(ProperDial());

    }

    public void UnlockDial()
    {
        GameManager.Instance.SetState(GameState.RatTrapPlaced, true);
        canDial = true;
    }

    public IEnumerator BossInterrupt()
    {
        hand.enabled = true;
        animator.SetFloat("Speed", 1f);
        animator.Play(animationFingerName, 0, 0f);
        // yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
        // animator.SetFloat("Speed", -1f);
        // animator.Play(animationFingerName, 0, 1f);
        // yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0);
        // hand.enabled = false;
        yield return null;

    }
    // TODO: use proper sound
    public IEnumerator ProperDial()
    {
        hand.enabled = true;
        animator.SetFloat("Speed", 1f);
        animator.Play(animationFingerName, 0, 0f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
        AudioManager.Instance.PlaySFX(fingerSnap);
        animator.SetFloat("Speed", -3f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0);
        hand.enabled = false;
        GameManager.Instance.SetState(GameState.NumberDialed, true);
    }
}
