using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneUI : Singleton<CutsceneUI>
{
    [SerializeField] private Image cutsceneImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private GameObject cinematicBotomPanel;
    [SerializeField] private GameObject cinematicTopPanel;

    private AspectRatioFitter backgroundAspect;

    protected override void Awake()
    {
        base.Awake();

        backgroundAspect =
            cutsceneImage.GetComponent<AspectRatioFitter>();

        if (backgroundAspect == null)
        {
            backgroundAspect =
                cutsceneImage.gameObject.AddComponent<AspectRatioFitter>();
        }

        backgroundAspect.aspectMode =
            AspectRatioFitter.AspectMode.FitInParent;

        HideAll();
    }

    public void ShowImage(Sprite sprite)
    {
        cutsceneImage.sprite = sprite;

        if (sprite != null)
        {
            backgroundAspect.aspectRatio =
                sprite.rect.width / sprite.rect.height;
        }

        cutsceneImage.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(true);
    }

    public void ShowCinematicBars()
    {
        cinematicBotomPanel.SetActive(true);
        cinematicTopPanel.SetActive(true);
    }

    public void HideCinematicBars()
    {
        cinematicBotomPanel.SetActive(false);
        cinematicTopPanel.SetActive(false);
    }

    public void HideImage()
    {
        cutsceneImage.gameObject.SetActive(false);

    }

    public void HideAll()
    {
        HideImage();
        backgroundImage.gameObject.SetActive(false);
        centerText.gameObject.SetActive(false);
        cinematicBotomPanel.gameObject.SetActive(false);
        cinematicTopPanel.gameObject.SetActive(false);
    }
}