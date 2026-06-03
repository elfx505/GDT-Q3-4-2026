using UnityEngine;

/// Represents a single location the player can stand in.
public class CameraAnchor : InteractableObject
{   
    
    [SerializeField] private Collider anchorCollider;
    
    private void Awake()
    {
        
        anchorCollider = GetComponent<Collider>();
        
        if (anchorCollider == null)
        {
            Debug.LogWarning($"[CameraAnchor] {gameObject.name} is missing a Collider!");
        }
    }

    public void ToggleActiveState(bool isActive)
    {
        if (anchorCollider != null)
        {
            anchorCollider.enabled = isActive;
        }
    }


    protected override void PerformAction()
    {
        base.PerformAction();
        
        GameManager.Instance.MoveToAnchor(this);
    }
}
