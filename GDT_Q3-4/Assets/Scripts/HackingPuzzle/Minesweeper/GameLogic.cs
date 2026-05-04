using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GameLogic : Singleton<GameLogic>
{
    [Header("UI References")]
    [Tooltip("The parent object with the Layout Group")]
    public Transform contentPanel;

    private int rows = 9;
    private int cols = 15;

    private int[,] gameGrid;
    private MinesweeperTile[,] tileGrid = new MinesweeperTile[9, 15];
    public GameObject tilePrefab;
    public RestartButton restartButton;
    public int clearedTileCount;
    private int nonBombCount;

    public enum MinesweeperGameState
    {
        WIN,
        LOSE,
        NORMAL
    };

    private void OnEnable()
    {
        MinesweeperTile.OnTileCleared += ClearTileGroup;
        MinesweeperTile.OnTileCleared += CheckGameState;

        MinesweeperTile.OnTileChorded += ChordTile;
        MinesweeperTile.OnTileChorded += CheckGameState;
    }

    private void OnDisable()
    {
        MinesweeperTile.OnTileCleared -= ClearTileGroup;
        MinesweeperTile.OnTileCleared -= CheckGameState;

        MinesweeperTile.OnTileChorded -= ChordTile;
        MinesweeperTile.OnTileChorded -= CheckGameState;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {   
        base.Awake();

        clearedTileCount = 0;
        
        // Hardcoded for now, might change later
        gameGrid = new int[,] 
        {
            {9, 1, 1, 1, 1, 1, 2, 9, 1, 0, 0, 0, 2, 9, 2}, // Initial Tile: (0, 10)
            {1, 2, 2, 9, 1, 1, 9, 3, 2, 0, 1, 2, 4, 9, 2},
            {2, 3, 9, 4, 3, 2, 2, 9, 2, 2, 2, 9, 9, 3, 2},
            {9, 9, 4, 9, 9, 2, 2, 3, 9, 2, 9, 5, 5, 9, 2},
            {4, 9, 3, 4, 9, 3, 1, 9, 2, 2, 3, 9, 9, 3, 9},
            {9, 2, 1, 2, 9, 2, 1, 1, 2, 1, 4, 9, 5, 3, 1},
            {2, 2, 1, 2, 2, 1, 0, 0, 1, 9, 3, 9, 9, 1, 0},
            {9, 2, 3, 9, 3, 2, 1, 2, 3, 4, 4, 3, 3, 3, 2},
            {2, 9, 3, 9, 9, 2, 9, 2, 9, 9, 9, 1, 1, 9, 9}
        };

        // Count bombs
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if (gameGrid[i, j] != 9) nonBombCount++;
            }
        }

    }


    void Start()
    {
        // Take game board info and instantiate a "Tile" Object on the canvas for each
        // Each Tile object contains the information of the type of tile they are and the clearable group of tiles they have on click
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 15; j++)
            {
                CreateTile(gameGrid[i, j], i, j);
            }
        }

        // Clear Initial Tile (0, 10)
        ClearTileGroup(0, 10);
    }


    public void CreateTile(int tileType, int tileX, int tileY)
    {
        // Instantiate the prefab and set the contentPanel as its parent
        GameObject newTile = Instantiate(tilePrefab, contentPanel);

        MinesweeperTile tileScript = newTile.GetComponent<MinesweeperTile>();
        // Feed Tile info from game grid into the actual tile to tell it which type it is
        if (tileScript != null)
        {
            tileScript.SetupTile(tileType, tileX, tileY);

            // Add tileScript to the MinesweeperTile grid
            tileGrid[tileX, tileY] = tileScript;
        }
        else
        {
            Debug.LogError("The prefab is missing the MinesweeperTile script!");
        }

        
    }

    private void ClearTileGroup(int startX, int startY)
    {   

        // Return empty if out of bounds or if the clicked tile is a bomb (9)
        if (startX < 0 || startX >= rows || startY < 0 || startY >= cols || gameGrid[startX, startY] == 9)
        {
            return;
        }

        var visited = new bool[rows, cols];
        var queue = new Queue<(int x, int y)>();

        // 8-way directional arrays (Up, Down, Left, Right, and Diagonals)
        int[] dx = { -1, -1, -1,  0, 0,  1, 1, 1 };
        int[] dy = { -1,  0,  1, -1, 1, -1, 0, 1 };

        queue.Enqueue((startX, startY));
        visited[startX, startY] = true;

        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();

            // Clear the Tile inside its own script
            tileGrid[cx, cy].ClearTile(); 

            // If the current tile is a number greater than 0, it acts as a boundary.
            // We reveal it, but we DO NOT explore its neighbors.
            if (gameGrid[cx, cy] != 0) 
            {   
                tileGrid[cx, cy].ClearTile(); 
                continue;
            }

            // If the tile is exactly 0, explore all 8 adjacent neighbors
            for (int i = 0; i < 8; i++)
            {
                int nx = cx + dx[i];
                int ny = cy + dy[i];

                // Check grid boundaries
                if (nx >= 0 && nx < rows && ny >= 0 && ny < cols)
                {
                    // If we haven't visited it yet, and it's not a bomb
                    if (!visited[nx, ny] && gameGrid[nx, ny] != 9)
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue((nx, ny));
                    }
                }
            }
        }
    }

    private void ChordTile(int startX, int startY)
    {
        // Boundary check
        if (startX < 0 || startX >= rows || startY < 0 || startY >= cols) return;

        MinesweeperTile clickedTile = tileGrid[startX, startY];
        int tileValue = gameGrid[startX, startY];

        // Chording is only valid on already cleared, numbered tiles (1-8)
        if (!clickedTile.isCleared || tileValue == 0 || tileValue >= 9) return;

        // 8-way directional arrays
        int[] dx = { -1, -1, -1,  0, 0,  1, 1, 1 };
        int[] dy = { -1,  0,  1, -1, 1, -1, 0, 1 };

        int flagCount = 0;

        // Count the flagged neighbors
        for (int i = 0; i < 8; i++)
        {
            int nx = startX + dx[i];
            int ny = startY + dy[i];

            if (nx >= 0 && nx < rows && ny >= 0 && ny < cols)
            {
                if (tileGrid[nx, ny].isFlagged)
                {
                    flagCount++;
                }
            }
        }

        // If the number of flags matches the tile's number, reveal the rest
        if (flagCount == tileValue)
        {
            for (int i = 0; i < 8; i++)
            {
                int nx = startX + dx[i];
                int ny = startY + dy[i];

                if (nx >= 0 && nx < rows && ny >= 0 && ny < cols)
                {
                    MinesweeperTile neighborTile = tileGrid[nx, ny];

                    // Skip tiles that are already handled
                    if (neighborTile.isFlagged || neighborTile.isCleared) continue;

                    // If the neighbor is a 0, trigger the flood fill
                    if (gameGrid[nx, ny] == 0)
                    {
                        ClearTileGroup(nx, ny);
                    }
                    else
                    {
                        // If it's a number (or a bomb), just clear that specific tile.
                        neighborTile.ClearTile();
                    }
                }
            }
        }
    }

    public void RestartGame()
    {
        clearedTileCount = 0;       

        foreach (MinesweeperTile tile in tileGrid)
        {
            tile.ResetTile();
        }

        restartButton.ChangeSprite(MinesweeperGameState.NORMAL);

        // Clear Initial Tile (0, 10)
        ClearTileGroup(0, 10);
    }

    private void CheckGameState(int tileX, int tileY)
    {   
        // Check if a bomb has been cleared, i.e. game over
        if (tileGrid[tileX, tileY].tileType == 9)
       {
            // Pause Tile Inputs
            foreach (MinesweeperTile tile in tileGrid)
            {
                tile.PauseTileInput();
            }

            // Change Reset Button Sprite
            restartButton.ChangeSprite(MinesweeperGameState.LOSE);

        }       

        // Check if Cleared tiles == non-bomb tiles
        if (nonBombCount == clearedTileCount)
        {
            // Pause Tile Inputs
            foreach (MinesweeperTile tile in tileGrid)
            {
                tile.PauseTileInput();
            }

            // Change Reset Button Sprite
            restartButton.ChangeSprite(MinesweeperGameState.WIN);
        }
    }

 
}
