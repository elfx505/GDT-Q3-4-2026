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
            InventoryManager.Instance.AddItem(startingNote);
        }
    }

    public void AddItem(ItemSO item)
    {
        if (items.Contains(item))
        {
            Debug.Log("Dduplicates " + item.itemName);
            return;
        }
        items.Add(item);
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.Refresh();
        if (InventoryUI.Instance != null)
        {
            Debug.Log("Added " + item.itemName);
            InventoryUI.Instance.Refresh();
        }
    }

    public void StartHolding(ItemSO item)
    {
        heldItem = item;
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.panelToToggle.SetActive(false); // close inv
    }

    public void StopHolding()
    {
        heldItem = null;
    }

    void Update()
    {
        if (heldItem != null && Input.GetMouseButtonDown(0)) // left click
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Used " + heldItem.itemName + " at " + pos);
            // StopHolding();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StopHolding();
        }
    }
}