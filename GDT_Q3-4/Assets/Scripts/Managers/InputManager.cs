using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : Singleton<InputManager>
{
    Vector2 mousePosition;
    RaycastHit raycastHit3D;
    public event Action onEKey;
    public event Action onEscape;

    // Whiteboard specific input actions
    public event Action<Vector2> OnDrawStart;
    public event Action<Vector2> OnDrawHold;
    public event Action OnDrawEnd;
    public event Action OnDrawClear;
    public event Action OnTrainAI;
    public event Action OnDeleteAIDatabase;

    // Event for Camera Look
    public event Action<Vector2> OnLookRotate;

    private IInteractable currentHoveredInteractable;

    void Update()
    {
        if (Mouse.current == null) return;

        mousePosition = Mouse.current.position.ReadValue();

        // Continuous Ray Hover Check
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out raycastHit3D))
        {
            IInteractable interactable = raycastHit3D.collider.GetComponent<IInteractable>();

            // Did the object we are looking at change?
            if (interactable != currentHoveredInteractable)
            {
                // Tell the old object we left it
                if (currentHoveredInteractable != null)
                {
                    currentHoveredInteractable.OnHoverExit();
                }

                // Update to the new object
                currentHoveredInteractable = interactable;

                // Tell the new object we entered it
                if (currentHoveredInteractable != null)
                {
                    currentHoveredInteractable.OnHoverEnter();
                }
            }
        }
        else
        {
            // The raycast hit nothing. If we were hovering over something, clear it.
            if (currentHoveredInteractable != null)
            {
                currentHoveredInteractable.OnHoverExit();
                currentHoveredInteractable = null;
            }
        }

        // Left Click Logic
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {  
            if (!GameManager.Instance.canDraw) return;
            OnDrawStart?.Invoke(mousePosition);
            Debug.Log("Draw Start");
        }
        else if (Mouse.current.leftButton.isPressed)
        {   
            if (!GameManager.Instance.canDraw) return;
            OnDrawHold?.Invoke(mousePosition);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // We already know what we are hovering over from the code above
            if (currentHoveredInteractable != null)
            {
                currentHoveredInteractable.OnClick();
            }

            if (!GameManager.Instance.canDraw) return;
            OnDrawEnd?.Invoke();
            Debug.Log("Draw End");
        }

        // Right Click Drag Logic for Camera Look
        if (Mouse.current.rightButton.isPressed)
        {
            // Read the delta (how much the mouse moved this frame)
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            OnLookRotate?.Invoke(mouseDelta);
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            onEKey?.Invoke();
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            GameManager.Instance.SetState(GameState.CompletedTutorial, true);
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            OnDrawClear?.Invoke();
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            OnTrainAI?.Invoke();
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            OnDeleteAIDatabase?.Invoke();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            onEscape?.Invoke();
            Debug.Log("Escape Pressed this frame!");
        }
    }
}
