using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;
using System.Collections;

public class GameLogic : Singleton<GameLogic>
{
    [Header("UI References")]
    [Tooltip("The parent object with the Layout Group")]
    public Transform contentPanel;

    private int rows = 9;
    private int cols = 15;
    [SerializeField] private int mines = 35;

    private int[,] gameGrid;
    private MinesweeperTile[,] tileGrid = new MinesweeperTile[9, 15];
    public GameObject tilePrefab;
    public RestartButton restartButton;
    public int clearedTileCount;
    private int nonBombCount;
    private MinesweeperGenerator minesweeperGenerator;

    [SerializeField] private MinesweeperCounter mineCounter;
    [SerializeField] private MinesweeperCounter countdownCounter;
    [Header("Countdown Settings")]
    private int currentTime = 0; 
    private Coroutine countdownCoroutine; // Holds the reference to our running timer
    public int countdownTime = 30;
    private bool isGameActive = false;
    System.Random random = new();

    // Cache the directional arrays at the class level so they are only created once
    private readonly int[] dx = { -1, -1, -1,  0, 0,  1, 1, 1 };
    private readonly int[] dy = { -1,  0,  1, -1, 1, -1, 0, 1 };

    // Cache a single list to hold neighbors. Capacity is locked to 8.
    private List<MinesweeperTile> cachedNeighbors = new List<MinesweeperTile>(8);

    public enum MinesweeperGameState
    {
        WIN,
        LOSE,
        NORMAL
    };

    private async void OnEnable()
    {
        MinesweeperTile.OnTileCleared += ClearTileGroup;
        MinesweeperTile.OnTileCleared += CheckGameState;

        MinesweeperTile.OnTileChorded += ChordTile;
        MinesweeperTile.OnTileChorded += CheckGameState;

        // Restart the timer if the component is re-enabled mid-game
        if (isGameActive && countdownCounter != null)
        {
            countdownCoroutine = StartCoroutine(TimerRoutine());
        }
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

        minesweeperGenerator = new MinesweeperGenerator(cols, rows, mines);

        clearedTileCount = 0;
        nonBombCount = (rows * cols) - mines;

    }

    private async void Start()
    {   
        if (mineCounter == null || countdownCounter == null)
        {
            Debug.LogWarning("[GameLogic] (Minesweeper) Counter not set!");
        }

        int random_row = random.Next(0, rows);
        int random_col = random.Next(0, cols);

        await GenerateBoard(random_col, random_row);

        // Clear Initial Tile
        ClearTileGroup(random_row, random_col);
        int totalMines = RemainingMines;

        countdownCounter.UpdateCounter(countdownTime); 
        isGameActive = true;

        countdownCoroutine = StartCoroutine(TimerRoutine());

        
    }

    public async Task GenerateBoard(int startX, int startY)
    {
        if (minesweeperGenerator == null)
        {
            Debug.LogWarning("Minesweeper Generator not instantiated!");
            return;
        }

        // --- BACKGROUND THREAD START ---
        // Push the heavy brute-force generation to a background thread.
        // Unity will continue running smoothly at 60+ FPS while this calculates.
        bool[] boolGrid = await Task.Run(() => 
        {
            // Give it a high attempt limit (e.g., 2000) because 40 mines is tough!
            return minesweeperGenerator.GenerateSolvableBoard(startX, startY, 2000);
        });
        // --- BACKGROUND THREAD END ---

        // We are now safely back on the main Unity thread!
        gameGrid = TranslateBoolGrid(boolGrid, cols, rows);

        for (int i = 0; i < rows; i++) 
        {
            for (int j = 0; j < cols; j++)
            {
                CreateTile(gameGrid[i, j], i, j);
            }
        }
    }

    private int[,] TranslateBoolGrid(bool[] boolGrid, int width, int height)
    {
        // Initialize as [rows, cols] to match your dummyGrid and tileGrid
        int[,] grid = new int[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                if (boolGrid[index])
                {
                    // Assign using [y, x] to match [row, col]
                    grid[y, x] = 9; 
                }
                else
                {
                    int mineCount = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int neighborX = x + dx;
                            int neighborY = y + dy;

                            if (neighborX >= 0 && neighborX < width && 
                                neighborY >= 0 && neighborY < height)
                            {
                                int neighborIndex = neighborY * width + neighborX;
                                if (boolGrid[neighborIndex])
                                {
                                    mineCount++;
                                }
                            }
                        }
                    }
                    
                    // Assign using [y, x] to match [row, col]
                    grid[y, x] = mineCount; 
                }
            }
        }

        return grid;
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

    /// <summary>
    /// Fast, zero-allocation method to grab valid neighbors.
    /// WARNING: Do not store the returned list in a variable that lasts longer than a single frame, 
    /// as it gets overwritten on the next call!
    /// </summary>
    public List<MinesweeperTile> GetNeighbors(int cx, int cy)
    {
        // Wipe the previous results instantly without destroying the memory block
        cachedNeighbors.Clear(); 

        for (int i = 0; i < 8; i++)
        {
            int nx = cx + dx[i];
            int ny = cy + dy[i];

            // Boundary check: ensure we don't look outside the grid
            if (nx >= 0 && nx < rows && ny >= 0 && ny < cols)
            {
                // Only add it if the tile actually exists (safety check for async loading)
                if (tileGrid[nx, ny] != null)
                {
                    cachedNeighbors.Add(tileGrid[nx, ny]);
                }
            }
        }

        return cachedNeighbors;
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
                continue;
            }

            // If the tile is exactly 0, fetch and explore valid neighbors
            List<MinesweeperTile> neighbors = GetNeighbors(cx, cy);

            foreach (MinesweeperTile neighbor in neighbors)
            {
                int nx = neighbor.tileX;
                int ny = neighbor.tileY;

                // If we haven't visited it yet, and it's not a bomb
                if (!visited[nx, ny] && gameGrid[nx, ny] != 9)
                {
                    visited[nx, ny] = true;
                    queue.Enqueue((nx, ny));
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

        List<MinesweeperTile> neighbors = GetNeighbors(startX, startY);
        int flagCount = 0;

        // 1. Count the flagged neighbors
        foreach (MinesweeperTile neighbor in neighbors)
        {
            if (neighbor.isFlagged) flagCount++;
        }

        // 2. If the number of flags matches the tile's number, reveal the rest
        if (flagCount == tileValue)
        {
            // CRITICAL: We must store the coordinates we want to clear FIRST.
            // If we call ClearTileGroup while looping, it will overwrite the GetNeighbors cache!
            List<Vector2Int> tilesToClear = new List<Vector2Int>();

            foreach (MinesweeperTile neighbor in neighbors)
            {
                if (!neighbor.isFlagged && !neighbor.isCleared)
                {
                    tilesToClear.Add(new Vector2Int(neighbor.tileX, neighbor.tileY));
                }
            }

            // 3. Now it is safe to actually clear them
            foreach (Vector2Int coord in tilesToClear)
            {
                if (gameGrid[coord.x, coord.y] == 0)
                {
                    // Trigger the flood fill
                    ClearTileGroup(coord.x, coord.y);
                }
                else
                {
                    // If it's a number (or a bomb), just clear that specific tile.
                    tileGrid[coord.x, coord.y].ClearTile();
                    CheckGameState(coord.x, coord.y); 
                }
            }
        }
    }

    public async void RestartGame()
    {   
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);

        foreach (MinesweeperTile tile in tileGrid)
        {
            if (tile != null) Destroy(tile.gameObject);
        }

        clearedTileCount = 0;       

        int random_row = random.Next(0, rows);
        int random_col = random.Next(0, cols);

        await GenerateBoard(random_col, random_row);

        // Clear Initial Tile
        ClearTileGroup(random_row, random_col);

        int totalMines = RemainingMines;

        restartButton.ChangeSprite(MinesweeperGameState.NORMAL);

        countdownCounter.UpdateCounter(countdownTime); 
        isGameActive = true;

        countdownCoroutine = StartCoroutine(TimerRoutine());

    }

    private void CheckGameState(int tileX, int tileY)
    {   
        // Check if a bomb has been cleared, i.e. game over
        if (tileGrid[tileX, tileY].tileType == 9)
       {    
            LoseGame();
        } 

        // Check if Cleared tiles == non-bomb tiles
        if (nonBombCount == clearedTileCount)
        {   
            isGameActive = false;
            if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);

            // Pause Tile Inputs
            foreach (MinesweeperTile tile in tileGrid)
            {
                if (tile.tileType == 9 && !tile.isFlagged)
                {
                    tile.ToggleFlag();
                }
                tile.PauseTileInput();
            }

            // Change Reset Button Sprite
            restartButton.ChangeSprite(MinesweeperGameState.WIN);
        }
    }

    private void LoseGame()
    {
        isGameActive = false;
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
            // Pause Tile Inputs
            foreach (MinesweeperTile tile in tileGrid)
            {
                tile.PauseTileInput();
            }

            // Change Reset Button Sprite
            restartButton.ChangeSprite(MinesweeperGameState.LOSE);
    }

    public int RemainingMines 
    {
        get 
        {
            int flagCount = 0;
            
            // C# can foreach through a 2D array effortlessly
            foreach (MinesweeperTile tile in tileGrid)
            {
                if (tile != null && tile.isFlagged)
                {
                    flagCount++;
                }
            }

            int remainingMines = mines - flagCount;
            mineCounter.UpdateCounter(remainingMines);

            return remainingMines;
        }
    }

    private IEnumerator TimerRoutine()
    {
        while (countdownCounter != null && countdownCounter.CurrentValue > 0)
        { 
            // Wait exactly 1 real-time second
            yield return new WaitForSeconds(1f);

            // Read the exact time currently displayed on the UI 
            int timeRemaining = countdownCounter.CurrentValue;

            timeRemaining--;

            countdownCounter.UpdateCounter(timeRemaining);

            // Check for game over
            if (timeRemaining <= 0)
            {
                LoseGame();
                break; // Exit the loop so it stops ticking
            }
        }
    }

 
}
