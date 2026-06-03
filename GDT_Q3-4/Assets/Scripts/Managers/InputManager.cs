using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : Singleton<InputManager>
{
    Vector2 mousePosition;
    RaycastHit raycastHit2D;
    public static event Action onEKey;

    // Whiteboard specific input actions
    public event Action<Vector2> OnDrawStart;
    public event Action<Vector2> OnDrawHold;
    public event Action OnDrawEnd;
    public event Action OnDrawClear;
    public event Action OnTrainAI;
    public event Action OnDeleteAIDatabase;

    // Event for Camera Look
    public event Action<Vector2> OnLookRotate;

    void Update()
    {
        if (Mouse.current == null) return;

        mousePosition = Mouse.current.position.ReadValue();

        // Left Click Logic (Raycasting for Interactables)
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
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            
            if (Physics.Raycast(ray, out raycastHit2D))
            {
                Transform clickObj = raycastHit2D.collider.transform;

                // Check if the clicked object is interactable
                IInteractable interactable = raycastHit2D.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.OnClick();
                }
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
    }
}
