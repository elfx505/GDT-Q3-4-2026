using UnityEngine;

public class Boxes : InteractableObject
{   

    [SerializeField] private Animator animator;
    [SerializeField] private Collider col;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("[Boxes] Animator component is missing!");
        }

        col = gameObject.GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("[Boxes] Collider component is missing!");
        }
    }
    protected override void PerformAction()
    {
        base.PerformAction();

        if (animator == null) return;

        animator.SetBool("hasClickedOnBoxes", true);

        if (col == null) return;

        col.enabled = false;

    }
}
