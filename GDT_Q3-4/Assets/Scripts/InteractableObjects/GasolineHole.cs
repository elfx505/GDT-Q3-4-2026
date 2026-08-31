using UnityEngine;

public class GasolineHole : InteractableObject
{
    
    [SerializeField] private GameObject paper;
    [SerializeField] private AudioTrack engineSound;

    protected override void Awake()
    {   
        base.Awake();

        if (paper == null)
        {
            Debug.LogWarning($"[GasolineHole] {gameObject.name}: Paper Child Object not set!");
            return;
        }

        paper.SetActive(false);
    }
    
    protected override void PerformAction()
    {
        base.PerformAction();

    }

    public void PrintPaper()
    {   
        if (GameManager.Instance.GetState(GameState.AllPipePuzzlesCompleted))
        {
            GameManager.Instance.SetState(GameState.PrinterFueled, true);
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayTrack(engineSound);
            paper.SetActive(true);
        }

        // Else Play Default Dialogue for the previously completed GameState
    }
}
