using UnityEngine;

public class PrinterPuzzleManager : Singleton<PrinterPuzzleManager>
{
    
    public int currentOpenDrawerIndex;

    private Animator animator;

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
    
    }

}
