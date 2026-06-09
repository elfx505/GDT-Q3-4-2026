using UnityEngine;

public class Phone : InteractableObject
{   
    private bool canDial = false;
    protected override void PerformAction()
    {
        base.PerformAction();


       if (!canDial) return;

       GameManager.Instance.SetState(GameState.RatTrapPlaced, true); 
    }

    public void UnlockDial()
    {
        canDial = true;
    }
}
