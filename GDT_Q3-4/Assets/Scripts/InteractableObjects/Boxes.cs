using UnityEngine;

public class Boxes : InteractableObject
{   

    [SerializeField] private Animator animator;
    protected override void PerformAction()
    {
        base.PerformAction();

        if (animator == null)
        {
            Debug.LogWarning("[Boxes] Animator is missing!");
            return;
        }


        animator.SetBool("hasClickedOnBoxes", true);
    }
}
