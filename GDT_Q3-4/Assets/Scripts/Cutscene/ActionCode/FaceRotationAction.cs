using System;
using System.Collections;
using UnityEngine;

public class FaceRotationAction : CutsceneAction
{
    [SerializeField] private GameObject objectToRotate;
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
                if (Vector3.Distance(objectToRotate.transform.eulerAngles, targetRotationObject.transform.eulerAngles) <= 0.01f)
                {
                    break;
                }

                objectToRotate.transform.rotation = Quaternion.Slerp(
                objectToRotate.transform.rotation,
                targetRotationObject.transform.rotation,
                rotationSpeed * Time.deltaTime
            );

                yield return null;
            }
            objectToRotate.transform.eulerAngles = targetRotationObject.transform.eulerAngles;

        }
        else
        {
            objectToRotate.transform.eulerAngles = targetRotationObject.transform.eulerAngles;
            yield return null;
        }
    }

}
