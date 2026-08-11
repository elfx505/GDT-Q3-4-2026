using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    private AudioSource source;
    private AudioTrack currentTrack;

    private bool isLoopingCustom = false;
    [SerializeField] private float defaultVolume = .1f;

    protected override void Awake()
    {
        base.Awake();
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

        currentTrack = track;

        source.clip = track.clip;
        // source.volume = track.volume;
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
        sfxSource.volume = Mathf.Clamp(volume, volume, source.volume);
        sfxSource.pitch = pitch;
        sfxSource.Play();

        Destroy(sfxSource, clip.length);
    }

    private void SetVolume(float newVolume)
    {
        source.volume = newVolume;
    }

    public float GetVolume()
    {
        return source.volume;
    }
}