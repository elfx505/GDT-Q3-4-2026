using UnityEngine;

public class DraggableButton : MonoBehaviour
{
    public string symbol;
    public float dropRadius = 0.1f;
    public AudioClip clickSFX;

    private Renderer rend;
    private Vector3 offset;
    private Camera cam;
    private Vector3 startPos;
    private Slot currentSlot;
    
    // We replace the float zCoord with an infinite mathematical 3D plane
    private Plane dragPlane;

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
        } 
        else 
        {
            // 1. Create an invisible plane at the button's position. 
            // We use transform.up so the plane aligns with how the button is rotated.
            dragPlane = new Plane(transform.up, transform.position);

            // 2. Shoot a ray from the mouse position into the 3D scene
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // 3. Check where the ray hits our invisible dragPlane
            if (dragPlane.Raycast(ray, out float enter))
            {
                // Calculate offset from the actual hit point on the 3D plane
                offset = transform.position - ray.GetPoint(enter);
            }

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
        if (PuzzleManager.sequenceMode) return;

        // Shoot a ray continuously while dragging
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Move the button to exactly where the ray hits the 3D plane
        if (dragPlane.Raycast(ray, out float enter))
        {
            transform.position = ray.GetPoint(enter) + offset;
        }
    }

    void OnMouseUp()
    {
        if (PuzzleManager.sequenceMode) return;
        CheckDrop();
    }

    void CheckDrop()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, dropRadius);

        foreach (var hit in hits)
        {
            Slot slot = hit.GetComponent<Slot>();
            if (slot != null && slot.currentButton == null)
            {
                transform.position = slot.transform.position;
                slot.currentButton = this;
                currentSlot = slot;

                PuzzleManager.Instance.CheckWin();
                return;
            }
        }

        transform.position = startPos;
    }

    void SetColor()
    {
        if (rend == null) return;
        Material mat = rend.material;

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropRadius);
    }
}