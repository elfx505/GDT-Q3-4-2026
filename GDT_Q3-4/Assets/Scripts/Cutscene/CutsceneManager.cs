using UnityEngine;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void PlayCutscene(Cutscene cutscene)
    {
        StartCoroutine(RunCutscene(cutscene));
    }

    IEnumerator RunCutscene(Cutscene cutscene)
    {
        foreach (var action in cutscene.actions)
        {
            if (action.runInParallel)
            {
                StartCoroutine(action.Play());
            }
            else
            {
                yield return StartCoroutine(action.Play());
            }
        }

        Debug.Log("Cutscene complete");
    }
}