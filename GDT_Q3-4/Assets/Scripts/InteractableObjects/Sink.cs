using Unity.VisualScripting;
using UnityEngine;

public class Sink : InteractableObject
{
    private bool isBroken;
    [SerializeField] private Transform water;
    private bool waterActive = false;

    private void Start()
    {
        water = gameObject.transform.GetChild(0);

        if (water == null)
        {
            Debug.LogWarning($"[Sink] {gameObject.name}: Water object not set!");
            return;
        }

        water.gameObject.SetActive(waterActive);


    }

    protected override void PerformAction()
    {
        base.PerformAction();

        if (!isBroken)
        {
            isBroken = true;
            SetWaterActive(true);
            GameManager.Instance.SetState(GameState.SinkBroken, true);
        }
        else
        {
            if (!GameManager.Instance.GetState(GameState.SinkRepaired))
            {
                GameTextController.Instance.HandleDialogue("Huh...why won't it turn off??[s]The Janitor has been missing even before I took my vacation.[s]*Sigh*[s]What's going on in this place?[s]...maybe I can try to fix it.");
            }
            else
            {
                SetWaterActive(!waterActive);
            }
        }
    }

    public void RepairSink()
    {
        if (isBroken)
        {
            SetWaterActive(false);
            GameManager.Instance.SetState(GameState.SinkRepaired, true);
        }
    }

    private void SetWaterActive(bool active)
    {
        waterActive = active;
        water.gameObject.SetActive(waterActive);
        // TODO: Play sink running sound.
    }
}
