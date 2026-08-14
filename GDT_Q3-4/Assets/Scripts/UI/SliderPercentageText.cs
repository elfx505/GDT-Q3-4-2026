using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderPercentageText : MonoBehaviour
{   
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        if (text == null)
        {
            Debug.LogWarning($"[SliderPercentageText] {gameObject.name}: Text Component not found!");
            return;
        }

        if (slider == null)
        {
            Debug.LogWarning($"[SliderPercentageText] {gameObject.name}: Slider Component not set!");
            return;
        }

        SetText(slider.value);
    }

    public void SetText(float value) // Set in the Slider
    {
        text.text = value.ToString("F3");
    }
}
