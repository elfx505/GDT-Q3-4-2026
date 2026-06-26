using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscenes/Actions/Traverse Locations")]

public class TraverseLocationsActionSO : CutsceneActionSO
{
    [Header("Movement")]
    [SerializeField] private List<Transform> locations;
    [SerializeField] private GameObject objectToMove;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float closeDistance = 0.1f;

    [Header("Rotation (Optional)")]
    [SerializeField] private Transform rotationTarget;   // the visual model to rotate
    [SerializeField] private float rotationSpeed = 10f;

    public override IEnumerator Play(CutsceneContext context)
    {
        foreach (Transform location in locations)
        {
            Vector3 targetPos3D = location.position;

            while (true)
            {
                Vector3 currentPos3D = objectToMove.transform.position;

                Vector2 current = new Vector2(currentPos3D.x, currentPos3D.z);
                Vector2 target = new Vector2(targetPos3D.x, targetPos3D.z);

                float distance = Vector2.Distance(current, target);

                if (distance <= closeDistance)
                    break;

                Vector2 direction2D = (target - current).normalized;


                Vector3 moveDir = new Vector3(direction2D.x, 0, direction2D.y);
                objectToMove.transform.position += moveDir * moveSpeed * Time.deltaTime;


                if (rotationTarget != null && direction2D.sqrMagnitude > 0.0001f)
                {
                    Vector3 lookDirection = new Vector3(direction2D.x, 0, direction2D.y);

                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

                    rotationTarget.rotation = Quaternion.Slerp(
                        rotationTarget.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }

                yield return null;
            }
        }
    }

}
