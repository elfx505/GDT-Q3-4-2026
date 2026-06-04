using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource source;
    private AudioTrack currentTrack;

    private bool isLoopingCustom = false;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (currentTrack == null || !isLoopingCustom) return;

        // Handle custom loop
        float loopEnd = currentTrack.loopEndTime > 0f 
            ? currentTrack.loopEndTime 
            : source.clip.length;

        if (source.time >= loopEnd)
        {
            source.time = currentTrack.loopStartTime;
        }
    }

    public void PlayTrack(AudioTrack track)
    {
        if (track == null || track.clip == null)
        {
            Debug.LogWarning("AudioTrack is null or missing clip!");
            return;
        }

        currentTrack = track;

        source.clip = track.clip;
        source.volume = track.volume;
        source.pitch = track.pitch;
        source.loop = false; // we handle looping manually

        // 🎯 NEW FEATURE: fromStart
        if (track.fromStart)
        {
            source.time = 0f;
        }
        else
        {
            source.time = track.loopStartTime;
        }

        source.Play();

        // Setup looping
        isLoopingCustom = true;
    }

    public void Stop()
    {
        source.Stop();
        currentTrack = null;
        isLoopingCustom = false;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.clip = clip;
        sfxSource.volume = volume;
        sfxSource.pitch = pitch;
        sfxSource.Play();

        Destroy(sfxSource, clip.length);
    }
}