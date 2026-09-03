using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonPersistent<AudioManager>
{

    private AudioSource source;
    private AudioTrack currentTrack;

    private bool isLoopingCustom = false;
    [SerializeField] private float defaultVolume = .1f;

    // Tracker for currently playing SFX to prevent spam stacking
    private HashSet<AudioClip> activeSFX = new HashSet<AudioClip>();

    protected override void Awake()
    {
        base.Awake();
        if (IsDuplicate)
            return;
        source = gameObject.AddComponent<AudioSource>();

        SetVolume(PlayerPrefs.GetFloat("MasterVolume", defaultVolume));
    }

    void Start()
    {
        PauseMenuManager.onMasterVolumeChanged += SetVolume;
    }

    void OnDestroy()
    {

        PauseMenuManager.onMasterVolumeChanged -= SetVolume;
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
        bool isSameTrack = currentTrack == track;

        currentTrack = track;

        source.clip = track.clip;
        source.volume = Mathf.Clamp01(track.volume * PlayerPrefs.GetFloat("MasterVolume"));
        source.pitch = track.pitch;
        source.loop = false; // we handle looping manually

        if (!isSameTrack)
        {
            if (track.fromStart)
            {
                source.time = 0f;
            }
            else
            {
                source.time = track.loopStartTime;
            }

            source.Play();
            isLoopingCustom = true;
        }
    }

    public void Stop()
    {
        source.Stop();
        currentTrack = null;
        isLoopingCustom = false;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f, float startTime = 0f, float endTime = -1f, bool canSpam = false)
    {
        if (clip == null) return;

        // If this exact clip is already playing, ignore the new request
        if (activeSFX.Contains(clip) && !canSpam) return;

        // Register the clip as currently playing
        activeSFX.Add(clip);

        AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.clip = clip;

        // Multiply the individual SFX volume by the master volume
        sfxSource.volume = Mathf.Clamp01(volume * source.volume);

        sfxSource.pitch = pitch;

        // Start at the requested position
        sfxSource.time = startTime;
        sfxSource.Play();

        StartCoroutine(StopSFXAtTime(sfxSource, endTime, clip));
    }

    public void StopSFX(AudioClip clip)
    {
        if (clip == null) return;

        // Get all AudioSources attached to the AudioManager
        AudioSource[] allSources = GetComponents<AudioSource>();

        foreach (AudioSource sfxSource in allSources)
        {
            // Ignore the main music track source
            if (sfxSource == source) continue;

            // If this source is playing the clip we want to interrupt
            if (sfxSource.clip == clip)
            {
                sfxSource.Stop();
                Destroy(sfxSource);
            }
        }

        // Remove from the active tracking list so it can be played again later
        if (activeSFX.Contains(clip))
        {
            activeSFX.Remove(clip);
        }
    }

    private IEnumerator StopSFXAtTime(
    AudioSource audioSource,
    float endTime,
    AudioClip clip)
    {
        // -1 means play until the end of the clip
        if (endTime < 0f)
        {
            yield return new WaitForSeconds(clip.length - audioSource.time);
        }
        else
        {
            float duration = endTime - audioSource.time;

            if (duration > 0f)
                yield return new WaitForSeconds(duration);
        }

        // If the audio source was manually destroyed by StopSFX(), abort early
        if (audioSource == null) yield break;

        // The sound is finished, remove it from the active list so it can be played again
        activeSFX.Remove(clip);

        audioSource.Stop();
        Destroy(audioSource);
    }

    public void SetVolume(float newVolume)
    {
        if (currentTrack)
        {
            source.volume = Mathf.Clamp01(newVolume * currentTrack.volume);
        }
        source.volume = newVolume;
    }

    public float GetVolume()
    {
        return source.volume;
    }
}