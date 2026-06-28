using System.Collections;
using UnityEngine;


public class CameraUNFocusAction : CutsceneAction
{

    public override IEnumerator Play(CutsceneContext context)
    {
        CameraManager.Instance.SetCameraTarget(null, 0f);

        CutsceneUI.Instance.HideCinematicBars();

        GameManager.Instance.ToggleCameraFocused();

        yield break;
    }
}
