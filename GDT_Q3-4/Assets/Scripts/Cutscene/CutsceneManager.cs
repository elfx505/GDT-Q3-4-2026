using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI dialogueText; 
    // This panel holds your text (so we can hide it during the timer)
    [SerializeField] private GameObject dialoguePanel; 

    [Header("Cutscene Configuration")]
    [SerializeField] private CutsceneData cutsceneData;

    private int currentEventIndex = 0;
    private bool isWaitingForClick = false;

    // A fast reference to the required AspectRatioFitter
    private AspectRatioFitter backgroundAspect;

    void Start()
    {
        // Safety checks
        if (cutsceneData == null || backgroundImage == null || dialogueText == null || dialoguePanel == null)
        {
            Debug.LogError("CutsceneManager: Missing references in the inspector!");
            return;
        }

        // Initialize background fitting (prevents stretching)
        backgroundAspect = backgroundImage.GetComponent<AspectRatioFitter>();
        if (backgroundAspect == null)
        {
            backgroundAspect = backgroundImage.gameObject.AddComponent<AspectRatioFitter>();
        }
        backgroundAspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;

        // Hide the text panel initially
        dialoguePanel.SetActive(false);

        // Start the master timeline coroutine
        if (cutsceneData.orderedEvents.Count > 0)
        {
            StartCoroutine(MasterCutsceneRoutine());
        }
    }

    void Update()
    {
        // Monitor for input only when the script is actively waiting
        if (isWaitingForClick && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            AdvanceSequentially();
        }
    }

    private IEnumerator MasterCutsceneRoutine()
    {
        // --- LOOP through every defined event sequentially ---
        while (currentEventIndex < cutsceneData.orderedEvents.Count)
        {
            CutsceneEvent currentEvent = cutsceneData.orderedEvents[currentEventIndex];

            // 1. SETUP the visual scene (the PNG)
            backgroundImage.sprite = currentEvent.backgroundSprite;
            // Update the aspect ratio dynamically for this new sprite
            if (currentEvent.backgroundSprite != null)
            {
                backgroundAspect.aspectRatio = currentEvent.backgroundSprite.rect.width / currentEvent.backgroundSprite.rect.height;
            }

            // 2. WAIT for the specified time (e.g., 5 seconds of just the image)
            // The dialogue panel remains hidden during this delay.
            dialoguePanel.SetActive(false); 
            Debug.Log($"Displaying Sprite {currentEventIndex} for {currentEvent.displayDuration}s...");
            yield return new WaitForSeconds(currentEvent.displayDuration);

            // 3. SHOW TEXT & FREEZE progress
            Debug.Log($"Timer up. Showing text for Sprite {currentEventIndex}. Waiting for click...");
            
            // Populate and enable the dialogue (if any exists)
            if (!string.IsNullOrWhiteSpace(currentEvent.dialogueText))
            {
                dialogueText.text = currentEvent.dialogueText;
                dialoguePanel.SetActive(true); // Show the text box

                // Now we enter the 'freeze' state.
                isWaitingForClick = true;

                // **Freeze execution right here** until the player clicks 
                // and `AdvanceSequentially()` is triggered.
                yield return new WaitUntil(() => !isWaitingForClick); 
            }
            else
            {
                // Optional: if dialogue is empty, skip the wait entirely
                AdvanceSequentially();
            }

            // The loop will now advance the index and start the next event's timer.
        }

        // --- All events finished ---
        EndCutscene();
    }

    // This is called by the Update loop when the user clicks
    private void AdvanceSequentially()
    {
        isWaitingForClick = false; // This releases the WaitUntil() lock
        currentEventIndex++;       // Move to the next event
    }

    private void EndCutscene()
    {
        Debug.Log("Master Cutscene Sequence Complete!");

        // 1. Turn off the background image so it disappears
        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(false);
        }

        // 2. Turn off the dialogue panel so the text box disappears
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // 3. OPTIONAL: If you added the 'BlackBackdrop' from the last step, 
        // you can turn off the entire Canvas GameObject to clear everything at once!
        transform.parent.gameObject.SetActive(false); 
    
        // 4. Trigger whatever happens next in your game here (e.g., enable player movement)
    }
}