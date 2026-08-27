using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemInteraction
{
    public ItemSO requiredItem;
    public UnityEvent onSuccess;

    public AudioClip successSFX;
    public float sfxStartTime;
    public float sfxEndTime;
    public float sfxVolume;
}