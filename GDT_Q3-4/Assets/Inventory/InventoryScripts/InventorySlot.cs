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

        InventoryManager.Instance.StartHolding(myItem);
    }
}