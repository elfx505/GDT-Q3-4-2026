using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemSO itemData;
    void OnMouseDown()
    {
        if (itemData == null) return;
        InventoryManager.Instance.AddItem(itemData);
        gameObject.SetActive(false);
        Debug.Log("Picked up: " + itemData.itemName);
    }
}