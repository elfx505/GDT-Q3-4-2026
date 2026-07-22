using UnityEngine;

public class ColorSwap : MonoBehaviour
{
    [SerializeField] private Color primary, secondary;
    void Start()
    {
        SetColor();
    }

    private void SetColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_Primary", primary);
        spriteRenderer.material.SetColor("_Secondary", secondary);
    }
}
