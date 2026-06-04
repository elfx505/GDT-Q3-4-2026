using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Track", menuName = "Audio/Audio Track")]
public class AudioTrack : ScriptableObject
{
    public AudioClip clip;

    [Header("Loop Settings")]
    [Tooltip("Time in seconds where the loop starts")]
    public float loopStartTime = 0f;

    [Tooltip("Time in seconds where the loop ends (0 = full clip)")]
    public float loopEndTime = 0f;

    [Header("Playback Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.5f, 2f)]
    public float pitch = 1f;

    public bool playOnLoad;
    public bool fromStart;
}