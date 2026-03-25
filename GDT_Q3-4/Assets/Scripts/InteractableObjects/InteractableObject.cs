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

    // For plugging in sounds/particles inside Inspector
    [SerializeField] private UnityEvent onInteract;

    public virtual void OnClick()
    {
        if (!isRepeatable && _hasBeenInteracted) return;

        ItemSO heldItem = InventoryManager.Instance.heldItem;

        if (heldItem != null)
        {
            foreach (var interaction in itemInteractions)
            {
                if (interaction.requiredItem == heldItem)
                {
                    Debug.Log($"Used {heldItem.itemName} on {name}");
                    interaction.onSuccess?.Invoke();

                    if (heldItem.onetime)
                        InventoryManager.Instance.StopHolding();

                    _hasBeenInteracted = true;
                    return;
                }
            }

            Debug.Log("That's not the correct item.");
            return;
        }

        Debug.Log($"Default click on: {name}");

        PerformAction();
        onInteract?.Invoke();

        _hasBeenInteracted = true;
    }


    protected virtual void PerformAction()
    {
        // Default behavior for interactable objects
    }
}