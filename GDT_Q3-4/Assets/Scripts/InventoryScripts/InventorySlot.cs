using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;

    private ItemSO myItem;

    public void SetItem(ItemSO item)
    {
        myItem = item;
        iconImage.sprite = item?.icon;
        iconImage.enabled = item != null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (myItem == null) return;

        if (myItem.isViewable)
        {
            Debug.Log("Opening viewer for: " + myItem.itemName);
            ItemViewer.Instance.ShowItem(myItem);     // Open viewer
        }
        else
        {
            Debug.Log("Starting to hold: " + myItem.itemName);
            InventoryManager.Instance.StartHolding(myItem);   // Normal hold behavior
        }
    }
}