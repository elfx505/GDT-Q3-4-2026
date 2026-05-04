using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [Header("Tab Setup")]
    [Tooltip("Drag the Tab Buttons here in order")]
    public Button[] tabButtons;
    
    [Tooltip("Drag the corresponding Content Panels here in the same order")]
    public GameObject[] contentPanels;

    [Header("Visual Styling")]
    public Color activeTabColor = Color.white;
    public Color inactiveTabColor = new Color(0.8f, 0.8f, 0.8f); // Light gray

    private void Start()
    {
        // Assign click events
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i; // Cache the index for the lambda closure
            tabButtons[i].onClick.AddListener(() => SwitchToTab(index));
        }

        // Initialize the system by opening the first tab
        if (tabButtons.Length > 0)
        {
            SwitchToTab(0);
        }
    }

    public void SwitchToTab(int tabID)
    {
        for (int i = 0; i < contentPanels.Length; i++)
        {
            // Activate the panel that matches the clicked ID, deactivate all others
            bool isTargetTab = (i == tabID);
            contentPanels[i].SetActive(isTargetTab);

            // Update the visual state of the buttons
            UpdateTabVisuals(i, isTargetTab);
        }
    }

    private void UpdateTabVisuals(int index, bool isActive)
    {
        // Change the button's background color to show which is active
        Image buttonImage = tabButtons[index].GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isActive ? activeTabColor : inactiveTabColor;
        }

        // Make the active tab un-clickable so the user can't re-trigger it
        tabButtons[index].interactable = !isActive;
    }
}