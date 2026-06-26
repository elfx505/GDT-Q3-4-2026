using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscenes/Actions/Camera Focus")]

public class CameraFocusActionSO : CutsceneActionSO
{
    // [SerializeField] private GameObject focusTarget;
    [SerializeField] private float rotationSpeed = 10f;

    public override IEnumerator Play(CutsceneContext context)
    {
        GameManager.Instance.ToggleCameraFocused();

        CameraManager.Instance.SetCameraTarget(context.GetCurrentFocusTarget(), rotationSpeed);
        yield break;
    }
}
