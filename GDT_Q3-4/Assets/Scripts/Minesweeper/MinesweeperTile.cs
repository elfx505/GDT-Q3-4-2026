using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

[RequireComponent(typeof(CanvasClickListener))]
public class MinesweeperTile : MonoBehaviour
{

    private Sprite revealedSprite;
    private Image currentImage;
    private CanvasClickListener clickListener;
    private int tileType;
    private int tileX;
    private int tileY;
    public bool isFlagged = false;
    public bool isCleared = false;
    public Sprite[] tileSprites;
    [SerializeField] private Sprite flaggedSprite;
    [SerializeField] private Sprite normalTileSprite;

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

        // Setup the button listener internally
        clickListener.onLeftClick.AddListener(OnButtonClicked);
        clickListener.onRightClick.AddListener(OnButtonRightClicked);
    }
    
    
    private void OnButtonClicked()
    {
        Debug.Log($"{tileType} Tile cleared at ({tileX}, {tileY})!");

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

    private void OnButtonRightClicked()
    {
        Debug.Log($"Tile at ({tileX}, {tileY}) flagged/unflagged!");

        ToggleFlag();

    } 

    public void ClearTile()
    {   
        isCleared = true;
        // Change Sprite to Revealed Tile Sprite
        currentImage.sprite = revealedSprite;
    }

    private void ToggleFlag()
    {   
        if (isCleared) return;

        bool currentFlagState = isFlagged;

        isFlagged = !currentFlagState;

        // Change Sprite to Flagged or Normal based on isFlagged boolean
        currentImage.sprite = isFlagged? flaggedSprite : normalTileSprite;

    }
    

}
