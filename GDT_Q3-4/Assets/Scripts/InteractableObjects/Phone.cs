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
    private bool isAnimating = false;

    protected override void Awake()
    {
        base.Awake();
        hand.enabled = false;
    }
    protected override void PerformAction()
    {
        base.PerformAction();


        if (!canDial && !isAnimating)
        {
            StartCoroutine(BossInterrupt());
            return;
        }
        if (!dialedOnce && !isAnimating)
        {
            StartCoroutine(SnapFinger());
        }
        else if (dialedOnce && !isAnimating)
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
        // Lock interactions
        isAnimating = true; 

        AudioManager.Instance.PlaySFX(phoneDial, 1, 1, 0, 1);
        hand.enabled = true;
        animator.SetBool("isInterrupting", true);

        // Wait for the Animator to enter the FingerApproach state
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FingerApproach"));

        // Get the exact length of the approach clip and wait for those seconds
        float approachLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(approachLength);

        // The Animator will auto-transition to FingerDisengage. Wait for it to register.
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FingerDisengage"));

        AudioManager.Instance.PlaySFX(phoneDialCancel);

        // Get the exact length of the reverse clip and wait for those seconds
        float disengageLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(disengageLength);

        // Tell the Animator to return to DummyState
        animator.SetBool("isInterrupting", false);

        // Wait to confirm we are back in the idle state
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("DummyState"));

        // Cleanup and unlock interactions
        hand.enabled = false;
        isAnimating = false; 
    }

    // TODO: use proper sound
    public IEnumerator SnapFinger()
    {   
        isAnimating = true;

        AudioManager.Instance.PlaySFX(phoneDial, 1, 1, 0, 1);
        hand.enabled = true;
        
        animator.SetBool("isInterrupting", true);

        // Wait for the Animator to enter the FingerApproach state
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FingerApproach"));

        // Set isSnapping to true so the Animator knows to take the FingerSnap path
        animator.SetBool("isSnapping", true);
        
        float approachLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(approachLength);

        // Wait for it to register the new state
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FingerSnap"));

        AudioManager.Instance.PlaySFX(fingerSnap);

        // Divide the length by -3 because your state speed is set to -3!
        float snapLength = animator.GetCurrentAnimatorStateInfo(0).length / 3f;
        yield return new WaitForSeconds(snapLength);

        // You MUST reset both booleans, otherwise DummyState will instantly re-trigger FingerApproach
        animator.SetBool("isSnapping", false);
        animator.SetBool("isInterrupting", false);

        // Wait to confirm we are back in the idle state
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("DummyState"));

        // Cleanup and unlock interactions
        hand.enabled = false;
        isAnimating = false; 
        dialedOnce = true;
    }

    private void ProperDial()
    {
        GameManager.Instance.SetState(GameState.NumberDialed, true);
    }
}
