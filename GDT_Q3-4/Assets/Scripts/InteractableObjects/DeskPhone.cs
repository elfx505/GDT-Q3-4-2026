using UnityEngine;

public class DeskPhone : ZoomInObjective
{
    [SerializeField] private Animator animator;


    private bool isOpen = false;
    protected override void PerformAction()
    {
        base.PerformAction();
        if (!isOpen)
        {
            isOpen = true;
            animator.SetBool("hasOpenedPhone", isOpen);
        }

    }


}
