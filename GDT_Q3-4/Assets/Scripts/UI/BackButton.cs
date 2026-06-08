using UnityEngine;
using System;

public class BackButton : MonoBehaviour
{
    public static event Action onBackButton;

    public void ButtonClicked()
    {
        onBackButton?.Invoke();
    }
}
