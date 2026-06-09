using UnityEngine;

public class Phone : InteractableObject
{   
    private bool canDial = false;
    protected override void PerformAction()
    {
        base.PerformAction();


       if (!canDial) return;

       GameManager.Instance.SetState(GameState.NumberDialed, true); 
    }

    public void UnlockDial()
    {   
        GameManager.Instance.SetState(GameState.RatTrapPlaced, true);
        canDial = true;
    }
}
