using UnityEngine;
using UnityEngine.UI;

public class HeldItemFollower : MonoBehaviour
{
    Image myImage;
    RectTransform myRectTransform;
    Canvas parentCanvas;

    void Awake()
    {
        myImage = GetComponent<Image>();
        myRectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (InventoryManager.Instance.heldItem == null)
        {
            myImage.enabled = false;
            return;
        }

        myImage.enabled = true;
        myImage.sprite = InventoryManager.Instance.heldItem.icon;

        // Convert screen mouse position to local canvas coordinates
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(), // The canvas's RectTransform
            Input.mousePosition,                         // Screen point
            parentCanvas.worldCamera,                    // Use null for Overlay canvas
            out localPoint
        );

        // Now set the anchoredPosition of this image
        myRectTransform.anchoredPosition = localPoint;
    }
}