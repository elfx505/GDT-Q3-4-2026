using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting; // Required for IScrollHandler

// Add the IScrollHandler interface to the class
public class MinesweeperCounter : MonoBehaviour, IScrollHandler
{
    [Header("UI References")]
    [Tooltip("The 3 UI Images on your canvas (Hundreds, Tens, Units)")]
    public Image[] digitImages = new Image[3]; 

    [Tooltip("Your 0-9 sprites, strictly in order!")]
    public Sprite[] numberSprites = new Sprite[10];

    [Header("Scroll Settings")]
    [Tooltip("Enable this if you want the player to change the value by scrolling (e.g., in a settings menu).")]
    [SerializeField] private int minScrollValue = 0;
    [SerializeField] private int maxScrollValue = 999;
    [SerializeField] private bool isCountdownCounter = false;

    // A public getter so other scripts (like GameLogic) can read the value the user scrolled to
    public int CurrentValue { get; private set; }

    // Call this from GameLogic (for the timer/mine counter) or internally via scrolling
    public void UpdateCounter(int value)
    {
        // Clamp the value to keep it safely within 3 digits
        CurrentValue = Mathf.Clamp(value, 0, 999);

        // Math trick to isolate each digit
        int hundreds = CurrentValue / 100;
        int tens = (CurrentValue / 10) % 10;
        int units = CurrentValue % 10;

        // Swap the sprites
        digitImages[0].sprite = numberSprites[hundreds];
        digitImages[1].sprite = numberSprites[tens];
        digitImages[2].sprite = numberSprites[units];
    }

    // Automatically triggered by Unity when the mouse scrolls over this GameObject
    public void OnScroll(PointerEventData eventData)
    {
        if (!isCountdownCounter) return;

        float scrollAmount = eventData.scrollDelta.y;

        if (scrollAmount > 0f)
        {
            // Scrolled UP: Increase value, clamped to max
            int value = Mathf.Min(CurrentValue + 1, maxScrollValue);
            UpdateCounter(value);

        }
        else if (scrollAmount < 0f)
        {
            // Scrolled DOWN: Decrease value, clamped to min
            int value = Mathf.Max(CurrentValue - 1, minScrollValue);
            UpdateCounter(value);
        }
    }
}