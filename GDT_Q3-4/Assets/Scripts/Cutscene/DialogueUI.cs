using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    public GameObject panel;
    public Text text;

    public bool IsComplete { get; private set; }

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string dialogue)
    {
        panel.SetActive(true);
        text.text = dialogue;
        IsComplete = false;
    }

    void Update()
    {
        if (panel.activeSelf && Input.GetMouseButtonDown(0))
        {
            panel.SetActive(false);
            IsComplete = true;
        }
    }
}