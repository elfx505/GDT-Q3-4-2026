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
        if (viewerPanel != null && viewerPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0)) // left click anywhere
            {
                Close();
            }
        }
    }
}