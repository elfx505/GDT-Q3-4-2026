using System.Collections.Generic;
using UnityEngine;

public class BlinkSequenceLight : MonoBehaviour
{
    public Light signalLight;

    public float blinkOnTime = 0.25f;
    public float blinkOffTime = 0.25f;
    public float pauseBetweenNumbers = 0.8f;
    public float pauseBetweenLoops = 2f;

    public List<int> sequence;
    private int numberIndex = 0;
    private int blinkCount = 0;

    private float timer = 0f;

    private enum State
    {
        TurnOn,
        TurnOff,
        PauseNumber,
        PauseLoop
    }

    private State state = State.TurnOn;

    void Start()
    {
        if (signalLight != null)
            signalLight.enabled = true;
    }

    void Update()
    {

        if (!PuzzleManager.sequenceMode)
        {
            if (signalLight != null)
                signalLight.enabled = true;
                Debug.Log("On");
            return;
        }
        if (sequence == null || sequence.Count == 0 || signalLight == null)
            return;

        timer -= Time.deltaTime;

        if (timer > 0f) return;

        switch (state)
        {
            case State.TurnOn:
                signalLight.enabled = true;
                timer = blinkOnTime;
                state = State.TurnOff;
                break;

            case State.TurnOff:
                signalLight.enabled = false;
                blinkCount++;

                if (blinkCount >= sequence[numberIndex])
                {
                    blinkCount = 0;
                    state = State.PauseNumber;
                    timer = pauseBetweenNumbers;
                }
                else
                {
                    state = State.TurnOn;
                    timer = blinkOffTime;
                }
                break;

            case State.PauseNumber:
                numberIndex++;

                if (numberIndex >= sequence.Count)
                {
                    numberIndex = 0;
                    state = State.PauseLoop;
                    timer = pauseBetweenLoops;
                }
                else
                {
                    state = State.TurnOn;
                    timer = blinkOnTime;
                }
                break;

            case State.PauseLoop:
                state = State.TurnOn;
                timer = blinkOnTime;
                break;
        }
    }
}