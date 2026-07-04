using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameStateCutscene
{
    public GameState state;
    public Cutscene cutscene;
}

public class CutsceneManager : Singleton<CutsceneManager>
{
    // public static H_CutsceneManager Instance;
    [SerializeField] private Cutscene cutsceneTest;
    [SerializeField] private List<GameStateCutscene> gameStateCutscenes = new();
    CutsceneContext context;

    private bool _isPlaying;

    public bool IsPlaying => _isPlaying;

    private void Start()
    {
        if (cutsceneTest)
        {
            PlayCutscene(cutsceneTest);
        }
        context =
            new CutsceneContext(Camera.main);
        GameManager.onGameStateChange += StateTriggeredCutscene;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChange -= StateTriggeredCutscene;
    }

    private void StateTriggeredCutscene(GameState state)
    {
        foreach (GameStateCutscene gameStateCutscene in gameStateCutscenes)
        {
            if (gameStateCutscene.state == state && GameManager.Instance.GetState(state))
            {
                PlayCutscene(gameStateCutscene.cutscene);
                break;
            }
        }
    }

    public void PlayCutscene(Cutscene cutscene)
    {
        if (_isPlaying)
        {
            Debug.LogWarning("A cutscene is already playing.");
            return;
        }

        StartCoroutine(RunCutscene(cutscene));
    }

    private IEnumerator RunCutscene(Cutscene cutscene)
    {
        _isPlaying = true;

        Debug.Log($"Starting cutscene: {cutscene.cutsceneID}");



        List<Coroutine> runningParallelActions = new();

        foreach (CutsceneAction action in cutscene.actions)
        {
            if (action == null)
                continue;

            if (action.runInParallel)
            {
                Coroutine routine =
                    StartCoroutine(action.Play(context));

                runningParallelActions.Add(routine);
            }
            else
            {
                yield return StartCoroutine(action.Play(context));
            }
        }

        _isPlaying = false;

        Debug.Log($"Finished cutscene: {cutscene.cutsceneID}");
    }
}