using UnityEngine;

using System.Collections;

public class PrinterPuzzleManager : Singleton<PrinterPuzzleManager>
{
    
    public int currentOpenDrawerIndex;

    private Animator animator;

    [Header("Drawer References")]
    public DefectivePrinterDrawer[] allDrawers; 

    [Header("Settings")]
    [Tooltip("How long the opening animation takes in seconds")]
    public float openAnimationDuration = 1.0f; // Adjust this in the Inspector to match your animation length

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("[PrinterPuzzleManager] Animator component is null!");
        }
    }



    public void OpenDrawer(int clickedDrawerIndex)
    {   
        if (currentOpenDrawerIndex != 0)
        {
            allDrawers[currentOpenDrawerIndex - 1].SetInnerItemsInteractable(false);
        }

        // If the drawer we clicked is ALREADY the open one, close it
        if (currentOpenDrawerIndex == clickedDrawerIndex)
        {
            currentOpenDrawerIndex = 0;
        }
        // Otherwise, open the newly clicked drawer 
        else
        {
            currentOpenDrawerIndex = clickedDrawerIndex;
        }

        animator.SetInteger("ActiveDrawer", currentOpenDrawerIndex);

        if (currentOpenDrawerIndex != 0)
        {
            StartCoroutine(EnableItemsAfterDelay(currentOpenDrawerIndex, openAnimationDuration));
        }
    
    }

    private IEnumerator EnableItemsAfterDelay(int targetDrawerIndex, float delay)
    {
        // Wait for the animation duration
        yield return new WaitForSeconds(delay);
        
        // SAFETY CHECK: Ensure the player didn't click a different drawer while we were waiting.
        // We only enable the colliders if this drawer is STILL the active one.
        if (currentOpenDrawerIndex == targetDrawerIndex)
        {
            // Array index is the drawer number minus 1
            allDrawers[targetDrawerIndex - 1].SetInnerItemsInteractable(true);
        }
    }

    public void BlockDrawer(int targetLevelIndex) // Called from PipeGridManager
    {
        OpenDrawer(0); // Close Drawer
        allDrawers[targetLevelIndex].isLevelComplete = true;
    }

    

}
