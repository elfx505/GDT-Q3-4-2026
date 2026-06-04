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

    public void changeColor() 
    {
        if (rend == null) return;
        StartCoroutine(ChangeColorRoutine());
    }

    private IEnumerator ChangeColorRoutine()
    {
        Material mat = rend.material;

        // Change to Red
        mat.SetColor("_BaseColor", Color.red);

        // Pause for 0.5 seconds
        yield return new WaitForSeconds(0.5f);

        // Change to White
        mat.SetColor("_BaseColor", Color.white);
    }
}