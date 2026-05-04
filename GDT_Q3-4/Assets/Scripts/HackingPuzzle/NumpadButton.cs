using TMPro;
using UnityEngine;

public class NumpadButton : MonoBehaviour
{   
    public int buttonValue;

    void Awake()
    {
        if (int.TryParse(gameObject.name, out int result))
        {
            buttonValue = result;

        } else
        {
            Debug.LogWarning($"Failed to Convert Button Name to Button Value for {gameObject.name}!");
        }
    }
}
