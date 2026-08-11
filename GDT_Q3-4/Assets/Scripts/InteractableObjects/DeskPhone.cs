using UnityEngine;

public class DeskPhone : ZoomInObjective
{
    [SerializeField] private string animationOpenName;
    [SerializeField] private Animator animator;


    private bool isOpen = false;
    protected override void PerformAction()
    {
        base.PerformAction();
        if (!isOpen)
        {
            animator.Play(animationOpenName, 0, 0f);
            isOpen = true;
        }


    }
}
