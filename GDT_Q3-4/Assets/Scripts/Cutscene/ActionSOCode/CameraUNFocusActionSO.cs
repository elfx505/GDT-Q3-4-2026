using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Cutscenes/Actions/Camera UNfocus")]
public class CameraUNFocusActionSO : CutsceneActionSO
{

    public override IEnumerator Play(CutsceneContext context)
    {
        GameManager.Instance.ToggleCameraFocused();

        CameraManager.Instance.SetCameraTarget(null, 0f);
        yield break;
    }
}
