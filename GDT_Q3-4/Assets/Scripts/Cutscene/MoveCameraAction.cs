using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Cutscene/Actions/Move Camera")]
public class MoveCameraAction : CutsceneAction
{
    public Transform targetPoint;
    public float duration = 2f;

    public override IEnumerator Play()
    {
        Transform cam = Camera.main.transform;

        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;

            float lerp = t / duration;

            cam.position = Vector3.Lerp(startPos, targetPoint.position, lerp);
            cam.rotation = Quaternion.Slerp(startRot, targetPoint.rotation, lerp);

            yield return null;
        }
    }
}