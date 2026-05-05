using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI winText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowWinText()
    {
        winText.text = "You Win!";
    }
}