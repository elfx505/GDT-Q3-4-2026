using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : Singleton<InputManager>
{
    Vector2 mousePosition;
    RaycastHit2D raycastHit2D;


    // Update is called once per frame
    void Update()
    {
        if (Mouse.current == null) return;

        mousePosition = Mouse.current.position.ReadValue();

        // Check for click release        
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Convert mousePosition 2D vector to WorldSpace
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

            raycastHit2D = Physics2D.Raycast(worldPos2D, Vector2.zero);
            
            Transform clickObj = raycastHit2D.collider != null ? raycastHit2D.collider.transform : null;

            if (!clickObj) return;
            
            // Check if the clicked object is interactable
            IInteractable interactable = raycastHit2D.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.OnClick();
            } 
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("r Key pressed!");
            GameManager.Instance.ChangeView();
        }

    }
}
