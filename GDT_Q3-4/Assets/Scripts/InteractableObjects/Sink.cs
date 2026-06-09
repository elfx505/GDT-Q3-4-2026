using Unity.VisualScripting;
using UnityEngine;

public class Sink : InteractableObject
{   
    private bool isBroken;
    [SerializeField] private Transform water;

    private void Start()
    {
        water = gameObject.transform.GetChild(0);

        if (water == null)
        {
            Debug.LogWarning($"[Sink] {gameObject.name}: Water object not set!");
            return;
        }

        water.gameObject.SetActive(false);


    }

    protected override void PerformAction()
    {
        base.PerformAction();

        if (!isBroken)
        {
            isBroken = true;
            water.gameObject.SetActive(true);
            GameManager.Instance.SetState(GameState.SinkBroken, true);

        }
    }

    public void RepairSink()
    {
        if (isBroken)
        {   
            water.gameObject.SetActive(false);
            GameManager.Instance.SetState(GameState.SinkRepaired, true);
        }
    }
}
