using System;
using UnityEditor.Animations;
using UnityEngine;

public class DeskDrawer : InteractableObject
{
    [SerializeField] private Animator animator;
    [SerializeField] private int id;

    protected override void PerformAction()
    {
        base.PerformAction();

        if (animator.GetInteger("Active Drawer") == id)
        {
            animator.SetInteger("Active Drawer", 0); // Close drawer if clicking on the same one
        } 
        else
        {
            animator.SetInteger("Active Drawer", id); // Open drawer
        }


    }

    
}
