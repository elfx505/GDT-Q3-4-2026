using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlinkSequenceLight : MonoBehaviour
{
    public Light signalLight;

    public float blinkOnTime = 0.25f;
    public float blinkOffTime = 0.25f;
    public float pauseBetweenNumbers = 0.8f;
    public float pauseBetweenLoops = 2f;

    public List<int> sequence = new List<int>();

    private Coroutine blinkCoroutine;

    private int numberIndex = 0;
    private int blinkCount = 0;

    private enum State
    {
        TurnOn,
        TurnOff,
        PauseNumber,
        PauseLoop
    }

    private State state = State.TurnOn;

    private void OnEnable()
    {
        PuzzleManager.OnSequenceModeChanged += HandleSequenceMode;
        PuzzleManager.OnSequenceCompleteChanged += HandleSequenceComplete;
    }

    private void OnDisable()
    {
        PuzzleManager.OnSequenceModeChanged -= HandleSequenceMode;
        PuzzleManager.OnSequenceCompleteChanged -= HandleSequenceComplete;
    }

    private void Start()
    {
        HandleSequenceMode(PuzzleManager.sequenceMode);
        HandleSequenceComplete(PuzzleManager.sequenceComplete);
    }

    private void HandleSequenceComplete(bool isComplete)
    {
        if (isComplete)
        {
            // Stop the coroutine and turn the light on permanently
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }

            if (signalLight != null)
            {
                signalLight.enabled = true;
            }
        }
    }

    private void HandleSequenceMode(bool isSequenceMode)
    {
        // Don't restart blinking if the puzzle is already solved
        if (PuzzleManager.sequenceComplete) return;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (isSequenceMode)
        {
            // Fetch the sequence from the puzzle manager
            sequence.Clear();
            foreach (int slotIndex in PuzzleManager.Instance.Sequence)
            {
                sequence.Add(slotIndex + 1);
            }

            if (sequence != null && sequence.Count > 0 && signalLight != null)
            {
                blinkCoroutine = StartCoroutine(BlinkRoutine());
            }
        }
        else
        {
            // When PuzzleManager sets sequenceMode = false, this runs!
            if (signalLight != null)
            {
                signalLight.enabled = true;
            }
        }
    }

    private IEnumerator BlinkRoutine()
    {
        numberIndex = 0;
        blinkCount = 0;
        state = State.TurnOn;

        while (true)
        {
            switch (state)
            {
                case State.TurnOn:
                    signalLight.enabled = true;
                    state = State.TurnOff;
                    yield return new WaitForSeconds(blinkOnTime);
                    break;

                case State.TurnOff:
                    signalLight.enabled = false;
                    blinkCount++;

                    if (blinkCount >= sequence[numberIndex])
                    {
                        blinkCount = 0;
                        state = State.PauseNumber;
                        yield return new WaitForSeconds(pauseBetweenNumbers);
                    }
                    else
                    {
                        state = State.TurnOn;
                        yield return new WaitForSeconds(blinkOffTime);
                    }
                    break;

                case State.PauseNumber:
                    numberIndex++;

                    if (numberIndex >= sequence.Count)
                    {
                        numberIndex = 0;
                        state = State.PauseLoop;
                        yield return new WaitForSeconds(pauseBetweenLoops);
                    }
                    else
                    {
                        state = State.TurnOn;
                        yield return new WaitForSeconds(blinkOnTime);
                    }
                    break;

                case State.PauseLoop:
                    state = State.TurnOn;
                    yield return new WaitForSeconds(blinkOnTime);
                    break;
            }
        }
    }
}