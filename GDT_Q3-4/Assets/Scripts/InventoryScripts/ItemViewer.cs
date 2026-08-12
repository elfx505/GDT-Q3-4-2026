using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemViewer : MonoBehaviour
{
    public static ItemViewer Instance { get; private set; }

    [SerializeField] private GameObject viewerPanel;
    [SerializeField] private Image bigImage;
    private ItemSO viewedItem;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (viewerPanel != null)
            viewerPanel.SetActive(false);
        else
            Debug.LogError("viewerPanel is NOT assigned!");
    }

    public void ShowItem(ItemSO item)
    {
        if (item == null) return;
        viewedItem = item;

        bigImage.sprite = item.viewSprite != null ? item.viewSprite : item.icon;

        viewerPanel.SetActive(true);
    }

    public void Close()
    {
        if (viewerPanel != null)
        {
            viewerPanel.SetActive(false);
            // --- GAME STATE TRIGGER ---
            if (viewedItem.name == "Note")
            {
                GameManager.Instance.SetState(GameState.ReadNote, true);
            }
        }

    }

    private void Update()
    {
        if (viewerPanel != null && viewerPanel.activeSelf)
        {
            if (Input.GetMouseButtonUp(0)) // left click anywhere
            {
                Close();
            }
        }
    }
}