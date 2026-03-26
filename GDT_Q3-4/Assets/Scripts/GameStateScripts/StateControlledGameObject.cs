using UnityEngine;

public class StateControlledGameObject : MonoBehaviour
{
    public string stateKey;    
    public bool activeIfTrue = true; // When state value is true, should the object be active or not
    private Transform spriteChildObj;


    void Awake()
    {
        spriteChildObj = gameObject.transform.GetChild(0);
    }
    void Start()
    {
        Refresh();
    }

    void OnEnable()
    {
        GameManager.onGameStateChange += HandleStateChange;
        Refresh();
    }
    
    void OnDisable()
    {
        GameManager.onGameStateChange -= HandleStateChange;
    }

    public void Refresh()
    {
        bool currentState = GameManager.Instance.GetState(stateKey);
        
        spriteChildObj.gameObject.SetActive(currentState == activeIfTrue);
    }

    private void HandleStateChange(string changedKey)
    {
        // Only react if the key that changed is the one we care about
        if (changedKey == stateKey)
        {
            Refresh();
        }
    }
}