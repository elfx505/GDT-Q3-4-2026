using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] protected bool isRepeatable = true;
    [Header("Item Interactions")]
    [SerializeField] private List<ItemInteraction> itemInteractions;
    protected bool hasBeenInteracted = false;

    [SerializeField] protected Outline outline;

    // For plugging in sounds/particles inside Inspector
    [SerializeField] private UnityEvent onInteract;

    [SerializeField] protected GameState unlockingGameState;
    [TextArea(3, 5)]
    [SerializeField] private string objectLockedDialogue;
    [SerializeField] private AudioClip interactionSFX;
    [SerializeField] private float sfxStartTime = 0f;
    [SerializeField] private float sfxEndTime = -1f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private AudioClip lockedInteractionSFX;
    [SerializeField] private float lockedSFXStartTime = 0f;
    [SerializeField] private float lockedSFXEndTime = -1f;
    [SerializeField] private float lockedSFXVolume = 1f;

    [SerializeField] private AudioClip incorrectInteractionSFX;
    [SerializeField] private float incorrectSFXStartTime = 0f;
    [SerializeField] private float incorrectSFXEndTime = -1f;
    [SerializeField] private float incorrectSFXVolume = 1f;


    // The Reset method is called automatically in the Unity Editor when the script is added
#if UNITY_EDITOR
    private void Reset()
    {
        // Check if the Outline component already exists to avoid duplicates
        Outline outline = GetComponent<Outline>();

        if (outline == null)
        {
            // 2. Add the component if it's missing
            outline = gameObject.AddComponent<Outline>();
        }

        // Apply default settings
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 5f;

        // Disable it by default so it only turns on during hover
        outline.enabled = false;

    }
#endif

    protected virtual void Awake()
    {
        // Keeping the fix from the previous step as well!
        if (outline == null)
        {
            outline = GetComponentInChildren<Outline>();
        }

        if (outline == null)
        {
            Debug.LogWarning($"Missing Outline Component for {gameObject.name}!");
        }
    }

    public virtual void OnClick()
    {
        if (!isRepeatable && hasBeenInteracted) return;

        ItemSO heldItem = InventoryManager.Instance.heldItem;

        if (heldItem != null)
        {
            foreach (var interaction in itemInteractions)
            {
                // Bulletproof string comparison
                if (interaction.requiredItem.name == heldItem.name)
                {
                    Debug.Log($"Successfully used {heldItem.itemName} on {name}");
                    interaction.onSuccess?.Invoke();

                    if (interaction.successSFX != null)
                    {
                        AudioManager.Instance.PlaySFX(interaction.successSFX, volume: interaction.sfxVolume, startTime: interaction.sfxStartTime, endTime: interaction.sfxEndTime);
                    }

                    // Consume the item if it's a one-time use
                    if (heldItem.onetime)
                    {
                        InventoryManager.Instance.StopHolding();
                    }
                    if (heldItem.consummable)
                    {
                        InventoryManager.Instance.RemoveItem(heldItem);
                    }

                    hasBeenInteracted = true;
                    return; // Exit out, we are done!
                }
            }

            // If it loops through all interactions and doesn't find a match:
            Debug.Log($"Tried to use {heldItem.name}, but that's not the correct item.");

            // Play Default Incorrect SFX
            if (incorrectInteractionSFX != null)
            {
                Debug.Log($"Playing SFX: {incorrectInteractionSFX.name} at volume {incorrectSFXVolume}");
                AudioManager.Instance.PlaySFX(incorrectInteractionSFX, volume: incorrectSFXVolume, startTime: incorrectSFXStartTime, endTime: incorrectSFXEndTime);
            }
            else
            {
                Debug.LogWarning("The incorrectInteractionSFX is NULL! Please assign it in the Inspector.");
            }
            return;
        }

        // If we made it here, the player's hand is empty. Do normal click.
        Debug.Log($"Default click on: {name}");

        // --- THE GATEKEEPER ---
        // If we have an unlocking state assigned, and that state is false, STOP here.
        if (!GameManager.Instance.GetState(unlockingGameState))
        {
            if (lockedInteractionSFX != null)
            {
                AudioManager.Instance.PlaySFX(lockedInteractionSFX, volume: lockedSFXVolume, startTime: lockedSFXStartTime, endTime: lockedSFXEndTime);
            }
            GameTextController.Instance.HandleDialogue(objectLockedDialogue);
            Debug.Log($"{name} is currently locked.");
            return;
        }
        // ----------------------

        PerformAction();
        onInteract?.Invoke();

        if (interactionSFX != null)
        {
            AudioManager.Instance.PlaySFX(interactionSFX, volume: sfxVolume, startTime: sfxStartTime, endTime: sfxEndTime);
        }

        hasBeenInteracted = true;
    }

    public virtual void OnHoverEnter()
    {
        // Hover Visual indicator via outline asset by Chris Nolet
        if (outline == null) return;

        outline.enabled = true;
    }

    public virtual void OnHoverExit()
    {
        // Hover Visual indicator via outline asset by Chris Nolet
        if (outline == null) return;

        outline.enabled = false;
    }


    protected virtual void PerformAction()
    {
        // Default behavior for interactable objects
    }
}