using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName = "Item";
    public ItemType itemType;
    public Sprite icon;
    public bool onetime = false;
    public bool consummable = true;
    [Header("Item Behavior")]
    public bool isViewable = false;
    public Sprite viewSprite;
}