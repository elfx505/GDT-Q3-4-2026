using UnityEngine;
using UnityEngine.Events; 

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private bool isRepeatable = true;
    
    private bool _hasBeenInteracted = false;

    // For plugging in sounds/particles inside Inspector
    [SerializeField] private UnityEvent onInteract;

    public virtual void OnClick()
    {
        if (!isRepeatable && _hasBeenInteracted) return;

        Debug.Log($"Interacted with: {gameObject.transform.name}");
        
        PerformAction(); // Execute Action

        onInteract?.Invoke(); // Invoke Sounds/VFX

        _hasBeenInteracted = true;
    }

    protected virtual void PerformAction()
    {
        // Default behavior for interactable objects
    }
}