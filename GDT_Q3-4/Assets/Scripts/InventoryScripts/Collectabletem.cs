using UnityEngine;

public class CollectibleItem : InteractableObject
{
    public ItemSO itemData;
    protected override void PerformAction()
    {
        base.PerformAction();
        
        if (itemData == null) return;
        InventoryManager.Instance.AddItem(itemData);
        gameObject.SetActive(false);
        Debug.Log("Picked up: " + itemData.itemName);
    }
}