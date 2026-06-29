using UnityEngine;
using UnityEngine.UI;

public class MinesweeperCounter : MonoBehaviour
{
    [Tooltip("The 3 UI Images on your canvas (Hundreds, Tens, Units)")]
    public Image[] digitImages = new Image[3]; 

    [Tooltip("Your 0-9 sprites, strictly in order!")]
    public Sprite[] numberSprites = new Sprite[10];

    public void UpdateCounter(int value)
    {
        // Ensure digits don't go below 0 or above 999
        int clampedValue = Mathf.Clamp(value, 0, 999);

        // Isolate each digit
        int hundreds = clampedValue / 100;
        int tens = (clampedValue / 10) % 10;
        int units = clampedValue % 10;

        // Swap the sprites
        digitImages[0].sprite = numberSprites[hundreds];
        digitImages[1].sprite = numberSprites[tens];
        digitImages[2].sprite = numberSprites[units];
    }
}