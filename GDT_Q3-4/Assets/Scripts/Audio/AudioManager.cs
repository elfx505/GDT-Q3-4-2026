using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonPersistent<AudioManager>
{

    private AudioSource source;
    private AudioTrack currentTrack;

    private bool isLoopingCustom = false;
    [SerializeField] private float defaultVolume = .1f;

    // Tracker for currently playing SFX to prevent spam stacking
    private HashSet<AudioClip> activeSFX = new HashSet<AudioClip>();
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    private Coroutine volumeFadeCoroutine;
    private bool isPaused;

    protected override void Awake()
    {
        base.Awake();
        if (IsDuplicate)
            return;
        source = gameObject.AddComponent<AudioSource>();

        source.outputAudioMixerGroup = musicMixerGroup;

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
        else if (!source.isPlaying && !isPaused)
        {
            source.time = currentTrack.loopStartTime;
            source.Play(); // Must call Play() to wake it up from a full stop
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
        source.pitch = track.pitch;
        source.loop = false; // we handle looping manually

        source.volume = Mathf.Clamp01(PlayerPrefs.GetFloat("MasterVolume"));

        // Assuming track.volumeMultiplier is a float (e.g., 0.5 = quieter, 2.0 = louder)
        float trackMultiplier = track.volumeMultiplier;

        // Convert the linear multiplier to Decibels (dB) for the mixer
        // If multiplier is 0, set to -80dB (muted). Otherwise, calculate the dB shift.
        float volumeDb = trackMultiplier > 0.001f ? 20f * Mathf.Log10(trackMultiplier) : -80f;

        mainMixer.SetFloat("MusicVolume", volumeDb);

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
        else if (!source.isPlaying)
        {
            // If it IS the same track, but it's not playing, resume it
            source.UnPause();
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
        sfxSource.volume = Mathf.Clamp01(volume * PlayerPrefs.GetFloat("MasterVolume", 1f));

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
        source.volume = newVolume;
        if (currentTrack)
        {
            source.volume = Mathf.Clamp01(newVolume * currentTrack.volume);
        }
    }

    public float GetVolume()
    {
        return source.volume;
    }

    public void PauseTrack(float fadeDuration = 1f)
    {
        if (source != null && source.isPlaying)
        {
            isPaused = true;

            if (volumeFadeCoroutine != null) StopCoroutine(volumeFadeCoroutine);

            // Fade to 0, then pause at the end
            volumeFadeCoroutine = StartCoroutine(FadeVolumeRoutine(0f, fadeDuration, true));
        }
    }

    public void ResumeTrack(float fadeDuration = 1f)
    {
        if (source != null && currentTrack != null && !source.isPlaying)
        {
            isPaused = false;
            if (volumeFadeCoroutine != null) StopCoroutine(volumeFadeCoroutine);

            // Unpause immediately so the audio starts playing while faded out
            source.UnPause();

            // Calculate the target volume based on your master settings
            float targetVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("MasterVolume", defaultVolume));

            // Fade up to the target, do not pause at the end
            volumeFadeCoroutine = StartCoroutine(FadeVolumeRoutine(targetVolume, fadeDuration, false));
        }
    }

    private IEnumerator FadeVolumeRoutine(float targetVolume, float duration, bool pauseOnComplete)
    {
        float startVolume = source.volume;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            // Smoothly transition between the start and target volume
            source.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / duration);

            yield return null; // Wait for the next frame before looping
        }

        // Guarantee the volume hits the exact target value at the end
        source.volume = targetVolume;

        // If this was a pause fade, pause the actual AudioSource now
        if (pauseOnComplete)
        {
            source.Pause();
        }
    }
}