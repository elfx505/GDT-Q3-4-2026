using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [SerializeField] Transform slotParent;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] public GameObject panelToToggle;

    private List<GameObject> createdSlots = new List<GameObject>();

    // hidden at first
    void Awake()
    {
        panelToToggle.SetActive(false);
    }


    void Start()
    {
        if (panelToToggle != null)
        {
            panelToToggle.SetActive(false);
        }
        else
        {
            Debug.LogWarning("panelToToggle is not assigned in Inspector!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (InventoryManager.Instance.heldItem == null)
            {
                if (panelToToggle != null)
                {
                    bool shouldShow = !panelToToggle.activeSelf;
                    panelToToggle.SetActive(shouldShow);
                    Debug.Log("Inventory toggled to: " + shouldShow);

                    if (shouldShow)
                        Refresh();
                }
                else
                {
                    Debug.LogError("panelToToggle is null - assign it in Inspector!");
                }
            }
        }
    }

    public void Refresh()
    {
        Debug.Log($"Refresh called. Items count: {InventoryManager.Instance.items.Count}");

        // Clear old
        foreach (var slot in createdSlots) Destroy(slot);
        createdSlots.Clear();

        foreach (var item in InventoryManager.Instance.items)
        {
            Debug.Log($"Instantiating slot for: {item.itemName}");

            GameObject slotGO = Instantiate(slotPrefab, slotParent);

            // After Instantiate(slotPrefab, slotParent)
            slotGO.name = "Slot_For_" + item.itemName;  // Easier to spot in Hierarchy
            RectTransform rt = slotGO.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;  // Reset position
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            Debug.Log($"Created slot at local pos: {rt.localPosition}, scale: {rt.localScale}");

            slotGO.SetActive(true);                     // Ensure active
            slotGO.transform.localScale = Vector3.one;  // Reset scale (sometimes instantiate messes it)

            createdSlots.Add(slotGO);

            // Assign icon
            Image iconImg = slotGO.GetComponentInChildren<Image>(true);  // true = include inactive
            if (iconImg != null)
            {
                if (item.icon != null)
                {
                    iconImg.sprite = item.icon;
                    iconImg.enabled = true;
                    iconImg.color = Color.white;  // Ensure full opacity
                    Debug.Log($"Icon set: {item.icon.name}");
                }
                else
                {
                    Debug.LogWarning($"Item {item.itemName} has no icon sprite assigned!");
                }
            }
            else
            {
                Debug.LogWarning("No Image component found in children of slot prefab!");
            }

            Button btn = slotGO.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                ItemSO captured = item;
                btn.onClick.AddListener(() => PickUpFromInventory(captured));
            }
        }

        // Force layout rebuild - do it on the slotParent's RectTransform
        if (slotParent != null)
        {
            Canvas.ForceUpdateCanvases();  // Helps in some cases
            LayoutRebuilder.ForceRebuildLayoutImmediate(slotParent.GetComponent<RectTransform>());
            Debug.Log("Forced layout rebuild on slotParent");
        }
        else
        {
            Debug.LogError("slotParent is null in InventoryUI!");
        }
    }

    void PickUpFromInventory(ItemSO item)
    {
        if (item.isViewable)
        {
            Debug.Log("Opening viewer for: " + item.itemName);
            ItemViewer.Instance.ShowItem(item);     // Open viewer
        }
        else
        {
            Debug.Log("Starting to hold: " + item.itemName);
            InventoryManager.Instance.StartHolding(item);   // Normal hold behavior
        }
        Refresh();
        if (panelToToggle != null) panelToToggle.SetActive(false);
    }
}