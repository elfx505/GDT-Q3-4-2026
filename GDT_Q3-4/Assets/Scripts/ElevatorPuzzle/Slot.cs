using UnityEngine;
using System.Collections;

public class Slot : MonoBehaviour
{
    public DraggableButton currentButton;
    public int index;
    private Renderer rend;

    public bool IsFilled() => currentButton != null;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void changeColor(bool isCorrectButton) 
    {
        if (rend == null) return;
        StartCoroutine(ChangeColorRoutine(isCorrectButton));
    }

    private IEnumerator ChangeColorRoutine(bool isCorrectButton)
    {
        Material mat = rend.material;

        // Change to Red or Green based on Bool
        Color color = isCorrectButton ? Color.green : Color.red;

        mat.SetColor("_BaseColor", color);

        // Pause for 0.5 seconds
        yield return new WaitForSeconds(0.5f);

        // Change to White
        mat.SetColor("_BaseColor", Color.white);
    }
}