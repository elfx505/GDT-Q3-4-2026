using UnityEngine;

public class Vent : InteractableObject
{
    [SerializeField] private bool ventUnlocked = false;
    protected override void PerformAction()
    {
        Debug.Log("NEEDS SCREWDRIVER");

    }

    public void UnlockVent()
    {
        Debug.Log("Hi");
        if (!ventUnlocked)
        {
            ventUnlocked = true;
            Debug.Log("Vent unlocked!");
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
