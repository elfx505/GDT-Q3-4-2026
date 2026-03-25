using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemInteraction
{
    public ItemSO requiredItem;
    public UnityEvent onSuccess;
}