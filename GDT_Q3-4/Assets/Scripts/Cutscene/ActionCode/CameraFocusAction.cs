using System.Collections;
using UnityEngine;

public class CameraFocusAction : CutsceneAction
{
    [SerializeField] private Transform focusTarget;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool useCinematicBars = true;

    public override IEnumerator Play(CutsceneContext context)
    {
        if (useCinematicBars) CutsceneUI.Instance.ShowCinematicBars();

        CameraManager.Instance.SetCameraTarget(focusTarget, rotationSpeed);
        Debug.Log("FOCUSING CAMERA");
        GameManager.Instance.ToggleCameraFocused();

        yield break;
    }
}
