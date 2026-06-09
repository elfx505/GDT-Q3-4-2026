using UnityEngine;
using System.Collections;

/// Represents a single location the player can stand in.
public class CameraAnchor : InteractableObject
{   
    
    [SerializeField] private Collider anchorCollider;
    public CameraAnchor infiniteStairwellCameraAnchor;
    [SerializeField] private SpriteRenderer visualIndicatorSprite;
    [SerializeField] private Transform childObjTransform; // Contains visual indicator sprite component

    [Header("Visual Indicator Bobbing Animation Params")]
    [SerializeField] private float speed = 2f;    
    [SerializeField] private float height = 0.5f; 

    private Vector3 startPos;
    private Coroutine bobCoroutine;

    public bool isDrawAnchor;
    [SerializeField] private bool isHallwayAnchor;
    private void Awake()
    {
        
        anchorCollider = GetComponent<Collider>();
        
        if (anchorCollider == null)
        {
            Debug.LogWarning($"[CameraAnchor] {gameObject.name} is missing a Collider!");
        }

        if (gameObject.CompareTag("LoopAnchor") && infiniteStairwellCameraAnchor == null)
        {
            Debug.LogWarning($"[CameraAnchor] Infinite Stairwell Destination Anchor is not assigned!");
        }

        childObjTransform = transform.GetChild(0).transform;
        if (childObjTransform == null)
        {
            Debug.LogWarning($"[CameraAnchor] {gameObject.name} is missing a Child Object!");
        }


        visualIndicatorSprite = GetComponentInChildren<SpriteRenderer>();
        if (visualIndicatorSprite == null)
        {
            Debug.LogWarning($"[CameraAnchor] {gameObject.name} is missing a Visual Indicator Sprite component in Child Object!");
        }

        visualIndicatorSprite.enabled = false;

        if (isHallwayAnchor)
        {
            ToggleActiveState(false); 
        }
        

    }

    private void Start()
    {
        GameManager.onGameStateChange += TryEnableHallwayAnchorCollider;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChange -= TryEnableHallwayAnchorCollider;
    }

    private void TryEnableHallwayAnchorCollider(GameState gameState)
    {
        if (gameState == GameState.JanitorDoorUnlocked)
        {
            ToggleActiveState(true);
        }
    }

    public void ToggleActiveState(bool isActive)
    {
        if (anchorCollider != null)
        {
            anchorCollider.enabled = isActive;
        }
    }


    protected override void PerformAction()
    {
        base.PerformAction();
        
        if (gameObject.CompareTag("LoopAnchor"))
        {   
            GameManager.Instance.MoveToAnchor(infiniteStairwellCameraAnchor);
        }
        else
        {
            GameManager.Instance.MoveToAnchor(this);
        }

        if (isDrawAnchor)
        {
            GameManager.Instance.canDraw = true;
        }
        else
        {
            GameManager.Instance.canDraw = false;
        }
    }

    public override void OnHoverEnter()
    {
       if (childObjTransform == null) return;
       StartBobbing(); 
        
        if (visualIndicatorSprite == null) return;
        visualIndicatorSprite.enabled = true;

    }

    public override void OnHoverExit()
    {
        if (childObjTransform == null) return;
        StopBobbing();

        if (visualIndicatorSprite == null) return;
        visualIndicatorSprite.enabled = false;

    }

    public void StartBobbing()
    {
        // Prevent multiple coroutines from running at the exact same time
        if (bobCoroutine == null)
        {
            bobCoroutine = StartCoroutine(BobSequence());
        }
    }

    public void StopBobbing()
    {
        if (bobCoroutine != null)
        {
            StopCoroutine(bobCoroutine);
            bobCoroutine = null; // Reset the reference
        }
    }

    private IEnumerator BobSequence()
    {
        // while true with yield statement simply acts exactly like Update()
        while (true)
        {
            float newY = startPos.y + (Mathf.Sin(Time.time * speed) * height);
            childObjTransform.localPosition = new Vector3(startPos.x, newY, startPos.z);

            // Pause this loop here, and resume it on the next frame
            yield return null; 
        }
    }
}
