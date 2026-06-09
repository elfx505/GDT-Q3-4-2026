using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private bool isRepeatable = true;
    [Header("Item Interactions")]
    [SerializeField] private List<ItemInteraction> itemInteractions;
    private bool _hasBeenInteracted = false;

    [SerializeField] private Outline outline;

    // For plugging in sounds/particles inside Inspector
    [SerializeField] private UnityEvent onInteract;

    [SerializeField] private GameState unlockingGameState;

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

    private void Awake()
    {
        outline = gameObject.GetComponent<Outline>();

        if (outline == null)
        {
            Debug.LogWarning($"Missing Outline Component for {gameObject.name}!");
        }

    }

    public virtual void OnClick()
    {
        if (!isRepeatable && _hasBeenInteracted) return;

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

                    // Consume the item if it's a one-time use
                    if (heldItem.onetime)
                    {
                        InventoryManager.Instance.StopHolding();
                    }

                    _hasBeenInteracted = true;
                    return; // Exit out, we are done!
                }
            }

            // If it loops through all interactions and doesn't find a match:
            Debug.Log($"Tried to use {heldItem.name}, but that's not the correct item.");
            return; 
        }

        // If we made it here, the player's hand is empty. Do normal click.
        Debug.Log($"Default click on: {name}");

        // --- THE GATEKEEPER ---
        // If we have an unlocking state assigned, and that state is false, STOP here.
        if (!GameManager.Instance.GetState(unlockingGameState))
        {
            Debug.Log($"{name} is currently locked.");
            return; 
        }
        // ----------------------

        PerformAction();
        onInteract?.Invoke();

        _hasBeenInteracted = true;
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