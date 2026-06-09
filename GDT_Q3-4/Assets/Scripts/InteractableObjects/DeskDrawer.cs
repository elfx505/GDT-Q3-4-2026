using System;
using UnityEngine;

public class DeskDrawer : InteractableObject
{
    [SerializeField] private String animationOpenName;
    [SerializeField] private String animationCloseName;
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
        else
        {
            animator.Play(animationCloseName, 0, 0f);
            isOpen = false;

        }

    }
}
