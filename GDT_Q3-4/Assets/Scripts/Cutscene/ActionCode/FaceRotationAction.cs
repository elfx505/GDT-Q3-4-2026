using System;
using System.Collections;
using UnityEngine;

public class FaceRotationAction : CutsceneAction
{
    [SerializeField] private GameObject rotateObject;
    [SerializeField] private GameObject targetRotationObject;
    [SerializeField] private bool transitionSmooth;
    [SerializeField] private float rotationSpeed = 2f;

    public override IEnumerator Play(CutsceneContext context)
    {
        Debug.Log("Rotating Object");
        if (transitionSmooth)
        {
            while (true)
            {
                if (Vector3.Distance(rotateObject.transform.eulerAngles, targetRotationObject.transform.eulerAngles) <= 0.01f)
                {
                    break;
                }

                rotateObject.transform.rotation = Quaternion.Slerp(
                rotateObject.transform.rotation,
                targetRotationObject.transform.rotation,
                rotationSpeed * Time.deltaTime
            );

                yield return null;
            }
        }
        else
        {
            rotateObject.transform.eulerAngles = targetRotationObject.transform.eulerAngles;
            yield return null;
        }
    }

}
