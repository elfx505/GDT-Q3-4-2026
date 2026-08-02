using UnityEngine;
using System.Collections;

public class Slot : MonoBehaviour
{
    public DraggableButton currentButton;
    public int index;
    private Renderer rend;
    private GameObject elevatorButtonModel; // Child Object

    public bool IsFilled() => currentButton != null;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        elevatorButtonModel = transform.GetChild(0).gameObject;
        elevatorButtonModel.SetActive(false);
    }

    private void OnEnable()
    {
        PuzzleManager.OnSequenceModeChanged += ActivateSequenceModeVisuals;
    }

    private void OnDisable()
    {
        PuzzleManager.OnSequenceModeChanged -= ActivateSequenceModeVisuals;
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

    private void ActivateSequenceModeVisuals(bool isSequenceMode)
    {   
        if (!isSequenceMode) return;

        currentButton.transform.GetComponent<MeshRenderer>().enabled = false;

        elevatorButtonModel.SetActive(true);
    }
}