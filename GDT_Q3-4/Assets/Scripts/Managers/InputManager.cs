using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using UnityEditor;

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


    // Update is called once per frame
    void Update()
    {
        if (Mouse.current == null) return;

        mousePosition = Mouse.current.position.ReadValue();

        // Check for click release        
        if (Mouse.current.leftButton.wasReleasedThisFrame)
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
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("r Key pressed!");
            GameManager.Instance.ChangeView();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("e Key pressed!");
            onEKey?.Invoke();
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Debug.Log("t Key pressed!");
            GameManager.Instance.SetState(GameState.CompletedTutorial, true);
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            Debug.Log("c Key pressed!");
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


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {   
            OnDrawStart?.Invoke(mousePosition);
            Debug.Log("Draw Start");
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            OnDrawHold?.Invoke(mousePosition);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            OnDrawEnd?.Invoke();
            Debug.Log("Draw End");
        }

    }
}
