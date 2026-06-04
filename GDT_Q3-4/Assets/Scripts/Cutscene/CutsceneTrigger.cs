using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public Cutscene cutscene;

    void Start()
    {
        CutsceneManager.Instance.PlayCutscene(cutscene);
    }
}