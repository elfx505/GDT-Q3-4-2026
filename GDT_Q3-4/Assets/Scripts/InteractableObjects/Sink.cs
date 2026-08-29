using Unity.VisualScripting;
using UnityEngine;

public class Sink : InteractableObject
{
    private bool isBroken;
    [SerializeField] private Transform water;
    public bool waterActive = false;

    [Header("Sink Audio Settings")]
    [SerializeField] private AudioSource sinkAudioSource;
    [SerializeField] private AudioClip sinkRunningClip;
    [Tooltip("Base volume for this specific sink (multiplied by Master Volume)")]
    [Range(0f, 1f)]
    [SerializeField] private float sinkMaxVolume = 1f;
    [SerializeField] private float loopStartTime = 2f;  // Where the middle loop starts
    [SerializeField] private float loopEndTime = 6f;    // Where the middle loop ends

    [Tooltip("The exact time the audio should completely stop playing")]
    [SerializeField] private float outroEndTime = 8f;

    private bool isCustomLooping = false;

    private void OnEnable()
    {
        // Adjust this if your event is in a different script, but based on your
        // AudioManager code, it looks like it's in PauseMenuManager
        PauseMenuManager.onMasterVolumeChanged += UpdateVolume;
    }

    private void OnDisable()
    {
        PauseMenuManager.onMasterVolumeChanged -= UpdateVolume;
    }

    private void Start()
    {
        water = gameObject.transform.GetChild(0);

        if (water == null)
        {
            Debug.LogWarning($"[Sink] {gameObject.name}: Water object not set!");
            return;
        }

        if (sinkAudioSource == null)
        {
            sinkAudioSource = GetComponent<AudioSource>();
        }

        if (AudioManager.Instance != null)
        {
            UpdateVolume(AudioManager.Instance.GetVolume());
        }

        water.gameObject.SetActive(waterActive);


    }

    private void Update()
    {
        if (sinkAudioSource != null && sinkAudioSource.isPlaying)
        {
            if (isCustomLooping)
            {
                // STAGE 1: We are trapping the audio in the middle loop
                if (sinkAudioSource.time >= loopEndTime)
                {
                    sinkAudioSource.time = loopStartTime;
                }
            }
            else
            {
                // STAGE 2: We stopped looping and are playing the outro. 
                // Stop the audio completely once it hits our custom end time limit.
                if (sinkAudioSource.time >= outroEndTime)
                {
                    sinkAudioSource.Stop();
                }
            }
        }
    }

    private void UpdateVolume(float masterVolume)
    {
        if (sinkAudioSource != null)
        {
            sinkAudioSource.volume = masterVolume * sinkMaxVolume;
        }
    }

    protected override void PerformAction()
    {
        base.PerformAction();

        if (!isBroken)
        {
            isBroken = true;
            SetWaterActive(true);
            GameManager.Instance.SetState(GameState.SinkBroken, true);
        }
        else
        {
            if (!GameManager.Instance.GetState(GameState.SinkRepaired))
            {
                GameTextController.Instance.HandleDialogue("Shoichi: Huh?[s]Shoichi: ...why won't it turn off??[s]Shoichi: The Janitor has been missing even before I took my vacation.[s]*Sigh*[s]Shoichi: What's going on in this place?[s]Shoichi: ...maybe I can try to fix it.");
            }
            else
            {
                SetWaterActive(!waterActive);
            }
        }
    }

    public void RepairSink()
    {
        if (isBroken)
        {
            SetWaterActive(false);
            GameManager.Instance.SetState(GameState.SinkRepaired, true);
        }
    }

    private void SetWaterActive(bool active)
    {
        waterActive = active;
        water.gameObject.SetActive(waterActive);
        
        if (waterActive)
        {
            // Start the water audio
            if (sinkAudioSource != null && sinkRunningClip != null)
            {
                sinkAudioSource.clip = sinkRunningClip;
                sinkAudioSource.time = 0f; // Start from the very beginning (Intro)
                sinkAudioSource.Play();
                isCustomLooping = true;    // Trap it in the middle loop
            }
        }
        else
        {
            // Turn off the water audio
            if (sinkAudioSource != null)
            {
                // Disable constraint and allow audio source to continue until outro timestamp
                isCustomLooping = false; 

                if (sinkAudioSource.time < loopEndTime)
                {
                    sinkAudioSource.time = loopEndTime; 
                }
            }
        }
    }
}
