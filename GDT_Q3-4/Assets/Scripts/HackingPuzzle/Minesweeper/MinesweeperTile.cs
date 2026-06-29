using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using NUnit.Framework;

[RequireComponent(typeof(CanvasClickListener))]
public class MinesweeperTile : MonoBehaviour
{

    private Sprite revealedSprite;
    private Image currentImage;
    private CanvasClickListener clickListener;
    public int tileType;
    public int tileX;
    public int tileY;
    public bool isFlagged = false;
    public bool isCleared = false;
    public bool isPaused = false;
    public Sprite[] tileSprites;
    [SerializeField] private Sprite flaggedSprite;
    [SerializeField] private Sprite normalTileSprite;
    [SerializeField] private Sprite depressedSprite; // 0-tile sprite

    public static event Action<int, int> OnTileCleared;
    public static event Action<int, int> OnTileChorded;
    

    public void SetupTile(int tileType, int tileX, int tileY)
    {
        this.tileType = tileType;
        this.tileX = tileX;
        this.tileY = tileY;
        
        if (tileType >= 0 && tileType < tileSprites.Length)
        {
            revealedSprite = tileSprites[tileType];
        }

        clickListener = gameObject.GetComponent<CanvasClickListener>();
        currentImage = gameObject.GetComponent<Image>();

        // Setup the button listeners internally
        clickListener.onLeftClick.AddListener(OnButtonClicked);
        
        clickListener.onLeftDown.AddListener(OnButtonDown);
        clickListener.onPointerExit.AddListener(OnPointerExitListener);

        clickListener.onLeftUp.AddListener(OnButtonReleased);
        clickListener.onRightUp.AddListener(OnButtonRightReleased);
    }

    public void ResetTile()
    {
        currentImage.sprite = normalTileSprite;

        isCleared = false;
        isFlagged = false;
        isPaused = false;

    }

    private void OnButtonDown()
    {
        if (isPaused) return;

        if (!isCleared && !isFlagged)
        {
            // Holding click on a normal unrevealed tile
            SetDepressedState(true);
        }
        else if (isCleared && tileType > 0 && tileType < 9)
        {
            // Holding click on a revealed number (Chording)
            List<MinesweeperTile> neighbors = GameLogic.Instance.GetNeighbors(tileX, tileY);
            foreach (MinesweeperTile neighbor in neighbors)
            {
                neighbor.SetDepressedState(true);
            }
        }
    }

    private void OnPointerExitListener()
    {
        // Cancel the visual depression if the mouse leaves the tile while holding click
        if (isPaused) return;
        ClearDepressedStates();
    }

    private void ClearDepressedStates()
    {
        if (!isCleared)
        {
            SetDepressedState(false);
        }
        else if (isCleared && tileType > 0 && tileType < 9)
        {
            List<MinesweeperTile> neighbors = GameLogic.Instance.GetNeighbors(tileX, tileY);
            foreach (MinesweeperTile neighbor in neighbors)
            {
                neighbor.SetDepressedState(false);
            }
        }
    }
    
    
    private void OnButtonReleased()
    {   
        if (isPaused) return;

        // Debug.Log($"{tileType} Tile cleared at ({tileX}, {tileY})!");
        ClearDepressedStates();

        if (isFlagged) return; // Can't clear flagged tiles

        if (!isCleared)
        {
            ClearTile();

            OnTileCleared?.Invoke(tileX, tileY);
        } else
        {
            OnTileChorded?.Invoke(tileX, tileY);
        }

    }

    private void OnButtonRightReleased()
    {
        if (isPaused) return;
        
        // Debug.Log($"Tile at ({tileX}, {tileY}) flagged/unflagged!");

        ToggleFlag();

    } 

    private void OnButtonClicked()
    {
        
    }

    public void ClearTile()
    {   
        if (isCleared) return;

        isCleared = true;

        GameLogic.Instance.clearedTileCount++;
        // Change Sprite to Revealed Tile Sprite
        currentImage.sprite = revealedSprite;
    }

    public void ToggleFlag()
    {   
        if (isCleared) return;

        bool currentFlagState = isFlagged;

        isFlagged = !currentFlagState;

        // Change Sprite to Flagged or Normal based on isFlagged boolean
        currentImage.sprite = isFlagged? flaggedSprite : normalTileSprite;

        Debug.Log("Remaining Mines: " + GameLogic.Instance.RemainingMines);
    }

    public void PauseTileInput()
    {
        isPaused = true;
    }

    public void SetDepressedState(bool isDepressed)
    {
        // Safety check: We NEVER want to depress a tile that is already cleared or flagged
        if (isCleared || isFlagged) return;

        currentImage.sprite = isDepressed ? depressedSprite : normalTileSprite;
    }
    

}
