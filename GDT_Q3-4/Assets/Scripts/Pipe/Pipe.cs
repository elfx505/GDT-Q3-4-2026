using UnityEngine;

public enum PipeType
{
    Source,
    Sink,
    Straight,
    Corner,
    T,
    Cross,
    Empty
}

public class Pipe : MonoBehaviour
{
    public PipeType type;
    public int rotation;
    public bool isPowered = false;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        
        if (rend == null)
            Debug.LogError($"Renderer NOT found on {gameObject.name}!");
    }

    void Start()
    {
        UpdateVisual();
    }

    public void Rotate()
    {
        rotation = (rotation + 1) % 4;
        transform.Rotate(0, 0, -90);
        GridManager.Instance.RecalculatePower();
    }

    void OnMouseDown()
    {
        Rotate();
    }

    public bool HasConnection(Vector2Int dir)
    {
        int r = rotation % 4;

        switch (type)
        {
            case PipeType.Source:
            case PipeType.Sink:
            case PipeType.Straight:
                return (r % 2 == 0 && (dir == Vector2Int.up || dir == Vector2Int.down)) ||
                       (r % 2 == 1 && (dir == Vector2Int.left || dir == Vector2Int.right));

            case PipeType.Cross:
                return true;

            case PipeType.Corner:
                if (r == 0) return dir == Vector2Int.right || dir == Vector2Int.down;
                if (r == 1) return dir == Vector2Int.down || dir == Vector2Int.left;
                if (r == 2) return dir == Vector2Int.left || dir == Vector2Int.up;
                if (r == 3) return dir == Vector2Int.up || dir == Vector2Int.right;
                break;

            case PipeType.T:
                if (r == 0) return dir != Vector2Int.up;
                if (r == 1) return dir != Vector2Int.right;
                if (r == 2) return dir != Vector2Int.down;
                if (r == 3) return dir != Vector2Int.left;
                break;

            case PipeType.Empty:
                return false;
        }
        return false;
    }

    public void SetPowered(bool powered)
    {
        isPowered = powered;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (rend == null) return;

        Color targetColor = isPowered ? Color.green : Color.red;

        foreach (var mat in rend.materials)
        {
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", targetColor);
            else if (mat.HasProperty("_Color"))
                mat.color = targetColor;
        }
    }
}