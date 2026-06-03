using UnityEngine;

/// Represents a single location the player can stand in.
public class CameraAnchor : InteractableObject
{   
    
    [SerializeField] private Collider anchorCollider;
    public CameraAnchor infiniteStairwellCameraAnchor;
    
    private void Awake()
    {
        
        anchorCollider = GetComponent<Collider>();
        
        if (anchorCollider == null)
        {
            Debug.LogWarning($"[CameraAnchor] {gameObject.name} is missing a Collider!");
        }

        if (gameObject.CompareTag("LoopAnchor") && infiniteStairwellCameraAnchor == null)
        {
            Debug.LogWarning($"[CameraAnchor] Infinite Stairwell Destination Anchor is not assigned!");
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
        
        if (gameObject.CompareTag("LoopAnchor"))
        {   
            GameManager.Instance.MoveToAnchor(infiniteStairwellCameraAnchor);
        }
        else
        {
            GameManager.Instance.MoveToAnchor(this);
        }

        if (gameObject.CompareTag("DrawAnchor"))
        {
            GameManager.Instance.canDraw = true;
        }
        else
        {
            GameManager.Instance.canDraw = false;
        }
    }
}
