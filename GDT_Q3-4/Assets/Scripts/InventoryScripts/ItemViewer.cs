using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemViewer : MonoBehaviour
{
    public static ItemViewer Instance { get; private set; }

    [SerializeField] private GameObject viewerPanel;
    [SerializeField] private Image bigImage;

    private void Awake()
    {
        Instance = this;
        viewerPanel.SetActive(false);
    }

    public void ShowItem(ItemSO item)
    {
        if (item == null) return;

        bigImage.sprite = item.viewSprite != null ? item.viewSprite : item.icon;

        viewerPanel.SetActive(true);
    }

    public void Close()
    {
        if (viewerPanel != null)
            viewerPanel.SetActive(false);
    }

    private void Update()
    {
        // Close with Q key (same as dropping held item)
        if (viewerPanel.activeSelf && Input.GetKeyDown(KeyCode.Q))
        {
            Close();
        }
    }
}