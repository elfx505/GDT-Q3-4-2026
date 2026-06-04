using UnityEngine;
public class MusicTest : MonoBehaviour
{
    public AudioTrack testTrack;

    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayTrack(testTrack);
    }
}