using UnityEngine;

public class Water : InteractableObject
{
    protected override void PerformAction()
    {
        base.PerformAction();

        Debug.Log("Needs Sponge to clean up!");
    }

    public void CleanUpWater()
    {
        Debug.Log("Cleaned up water!");
        GameManager.Instance.SetState("completed_tutorial", true);
    }
}
