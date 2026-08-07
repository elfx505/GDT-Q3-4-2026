using UnityEngine;

public class HoleInWall : InteractableObject
{   
    protected override void PerformAction()
    {
        base.PerformAction();


    }

    public void BreakWall()
    {
        gameObject.SetActive(false); // Disable the object to simulate wall break
    }
}
