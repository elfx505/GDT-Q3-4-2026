using UnityEngine;

/// Represents a single location the player can stand in.
public class CameraAnchor : InteractableObject
{
    protected override void PerformAction()
    {
        base.PerformAction();
        
        // Move to this anchor. We don't pass a "lookAwayFrom" variable, 
        // so it will just use however you rotated this anchor object in the editor.
        GameManager.Instance.MoveToAnchor(this);
    }
}
