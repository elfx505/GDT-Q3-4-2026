using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("Starting Items")]
    public ItemSO startingNote;
    public List<ItemSO> items = new List<ItemSO>();
    public ItemSO heldItem = null;

    void Start()
    {
        if (startingNote != null)
        {
            AddItem(startingNote); 
        }
    }

    public void AddItem(ItemSO item)
    {
        if (items.Contains(item))
        {
            Debug.Log("Duplicate: " + item.itemName);
            return;
        }
        items.Add(item);
        
        if (InventoryUI.Instance != null)
        {
            Debug.Log("Added " + item.itemName);
            InventoryUI.Instance.Refresh();
        }
    }

    public void StartHolding(ItemSO item)
    {
        heldItem = item;
        
    }

    public void StopHolding()
    {
        heldItem = null;
    }
}