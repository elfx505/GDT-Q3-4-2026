using System.Collections.Generic;
using UnityEngine;

public class DefectivePrinterDrawer : InteractableObject
{
    [SerializeField] private int drawerIndex;

    [Header("Inner Drawer Items")]
    // Drag the colliders of any items inside this drawer into this array in the Inspector
    private Collider[] innerItemColliders; 
    public bool isLevelComplete = false;

    private void Start()
    {
        innerItemColliders = GetOnlyChildColliders();
        
        SetInnerItemsInteractable(false);
    }

    public void SetInnerItemsInteractable(bool isInteractable)
    {
        foreach (Collider item in innerItemColliders)
        {
            if (item != null)
            {
                item.enabled = isInteractable;
            }
        }
    }

    protected override void PerformAction()
    {
        base.PerformAction();

        if (PrinterPuzzleManager.Instance == null) {
            Debug.LogWarning($"[DefectivePrinterDrawer] <{gameObject.name}> PrinterPuzzleManager Instance is null!");
            return;
        }

        if (isLevelComplete) return; // Block drawer if level is complete

        PrinterPuzzleManager.Instance.OpenDrawer(drawerIndex);
    }

    // Helper function since GetComponentsInChildren also take the parent's selected component 
    private Collider[] GetOnlyChildColliders()
    {
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        
        List<Collider> childCollidersOnly = new List<Collider>();

        foreach (Collider col in allColliders)
        {
            // If the collider's GameObject is NOT this GameObject, it's a child
            if (col.gameObject != this.gameObject)
            {
                childCollidersOnly.Add(col);
            }
        }

        return childCollidersOnly.ToArray();
    }
}
