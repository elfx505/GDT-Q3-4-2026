using UnityEngine;

public class CollectibleItem : InteractableObject
{
    public ItemSO itemData;
    protected override void PerformAction()
    {
        base.PerformAction();
        
        if (itemData == null)
        {   
            Debug.LogWarning($"[CollectibleItem] {gameObject.name} Item Scriptable Object not set!");
            return;
        }
        InventoryManager.Instance.AddItem(itemData);
        gameObject.SetActive(false);
        Debug.Log("Picked up: " + itemData.itemName);
    }
}