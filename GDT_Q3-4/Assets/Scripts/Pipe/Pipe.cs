using UnityEngine;

public enum PipeType
{
    Source,
    Sink,
    Straight,
    Corner,
    T,
    Cross
}

public class Pipe : MonoBehaviour
{
    public PipeType type;
    public int rotation;

    public bool isPowered = false;

    public Renderer rend;

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
                       return (r % 2 == 0 && (dir == Vector2Int.up || dir == Vector2Int.down)) ||
                       (r % 2 == 1 && (dir == Vector2Int.left || dir == Vector2Int.right));
            case PipeType.Sink:
                return (r % 2 == 0 && (dir == Vector2Int.up || dir == Vector2Int.down)) ||
                       (r % 2 == 1 && (dir == Vector2Int.left || dir == Vector2Int.right));
            case PipeType.Cross:
                return true;

            case PipeType.Straight:
                return (r % 2 == 0 && (dir == Vector2Int.up || dir == Vector2Int.down)) ||
                       (r % 2 == 1 && (dir == Vector2Int.left || dir == Vector2Int.right));

            case PipeType.Corner:
                if (r == 0) return dir == Vector2Int.right || dir == Vector2Int.down;
                if (r == 1) return dir == Vector2Int.down || dir == Vector2Int.left;
                if (r == 2) return dir == Vector2Int.left || dir == Vector2Int.up;
                if (r == 3) return dir == Vector2Int.up || dir == Vector2Int.right;
                break;

            case PipeType.T:
                if (r == 0) return dir != Vector2Int.down;
                if (r == 1) return dir != Vector2Int.left;
                if (r == 2) return dir != Vector2Int.up;
                if (r == 3) return dir != Vector2Int.right;
                break;
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
        if (rend != null)
        {
            rend.material.color = isPowered ? Color.yellow : Color.white;
        }
    }
}