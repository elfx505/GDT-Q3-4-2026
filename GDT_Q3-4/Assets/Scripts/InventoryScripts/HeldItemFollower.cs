using UnityEngine;
using UnityEngine.UI;

public class HeldItemFollower : MonoBehaviour
{
    private Image myImage;
    private RectTransform myRectTransform;

    void Awake()
    {
        myImage = GetComponent<Image>();
        myRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (InventoryManager.Instance.heldItem == null)
        {
            myImage.enabled = false;
            return;
        }

        // Show the held item icon
        myImage.enabled = true;
        myImage.sprite = InventoryManager.Instance.heldItem.icon;

        // Simple and reliable for Screen Space - Overlay
        myRectTransform.position = Input.mousePosition;

        // Optional: small offset so the icon doesn't cover the cursor tip
        // myRectTransform.position = Input.mousePosition + new Vector3(25, -25, 0);
    }
}