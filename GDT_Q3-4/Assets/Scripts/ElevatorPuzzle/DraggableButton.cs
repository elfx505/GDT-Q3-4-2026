using UnityEngine;

public class DraggableButton : MonoBehaviour
{
    public string symbol;

    private Renderer rend;
    private Vector3 offset;
    private Camera cam;
    private Vector3 startPos;
    private Slot currentSlot;

    public AudioClip clickSFX;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        SetColor();
    }

    void Start()
    {
        cam = Camera.main;
        startPos = transform.position;
    }

    void OnMouseDown()
    {
        AudioManager.Instance.PlaySFX(clickSFX, 1f, Random.Range(0.9f, 1.1f));

        if (PuzzleManager.sequenceMode) 
        {
            PuzzleManager.Instance.PressButton(currentSlot.index);
        } else 
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(cam.transform.position.z - transform.position.z);
            mousePos = cam.ScreenToWorldPoint(mousePos);
            offset = transform.position - mousePos;

            // Free current slot when picked up
            if (currentSlot != null)
            {
                currentSlot.currentButton = null;
                currentSlot = null;
            }
            PuzzleManager.Instance.CheckWin();
        }
    }

    void OnMouseDrag()
    {
        if (PuzzleManager.sequenceMode) 
        {
            return;
        }
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        mousePos = cam.ScreenToWorldPoint(mousePos);
        transform.position = mousePos + offset;
    }

    void OnMouseUp()
    {
        if (PuzzleManager.sequenceMode) 
        {
            return;
        }
        CheckDrop();
    }

    void CheckDrop()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);

        foreach (var hit in hits)
        {
            Slot slot = hit.GetComponent<Slot>();
            if (slot != null && slot.currentButton == null)
            {
                // Snap to slot
                transform.position = slot.transform.position;
                slot.currentButton = this;
                currentSlot = slot;

                PuzzleManager.Instance.CheckWin();
                return;
            }
        }

        // Return to start if dropped in invalid place
        transform.position = startPos;
    }

    void SetColor()
    {
        if (rend == null) return;

        Material mat = rend.material;   // Important: creates instance

        switch (symbol)
        {
            case "!": mat.SetColor("_BaseColor", Color.red); break;
            case "@": mat.SetColor("_BaseColor", Color.blue); break;
            case "#": mat.SetColor("_BaseColor", Color.green); break;
            case "$": mat.SetColor("_BaseColor", Color.yellow); break;
            case "%": mat.SetColor("_BaseColor", Color.magenta); break;
            case "^": mat.SetColor("_BaseColor", Color.cyan); break;
            default:  mat.SetColor("_BaseColor", Color.white); break;
        }
    }
}