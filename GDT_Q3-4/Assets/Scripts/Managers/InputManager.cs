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
            GameManager.Instance.SetState("completed_tutorial", true);
        }

    }
}
