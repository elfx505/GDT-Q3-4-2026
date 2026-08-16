using UnityEngine;
using System.Collections.Generic;

public class PipeGridManager : Singleton<PipeGridManager>
{

    public GameObject[] prefabs;
    public static int size = 7;
    public Pipe[,] grid = new Pipe[7,7];
    public float spacing = 1.5f;
    public bool random_level = false;
    public int levels;
    private int lastLoadedLevel;


    void Start()
    {   

        for (int i = 1; i < 4; i++) // Levels 1 to 3
        {
            GenerateRandomLevels(i);
        }

    }

    void ClearGrid()
    {
        // Destroy all pipe GameObjects
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }
    }

    public void LoadLevel(int level)
    {
        // Remove old pipes before creating new ones
        ClearGrid();

        char[,] data = LevelLoader.LoadLevel("level"+level.ToString());

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                PipeType type = CharToType(data[x, y]);
                GameObject prefab = GetPrefab(type);

                float offset = (size - 1) / 2f;
                
                // 1. Calculate local position (this remains the same)
                Vector3 localPos = new Vector3(
                    (x - offset) * spacing,
                    (y - offset) * spacing,
                    0
                );

                // 2. Instantiate as child. Passing 'false' prevents Unity from doing 
                // weird World-Space offsets if the Manager is rotated.
                GameObject obj = Instantiate(prefab, transform, false);

                // 3. Apply the exact local position
                obj.transform.localPosition = localPos;

                // Force the baseline rotation to (0,0,0) locally
                obj.transform.localRotation = Quaternion.identity;

                Pipe pipe = obj.GetComponent<Pipe>();
                pipe.type = type;

                int r = Random.Range(0, 4);
                if (!random_level) r = 0;
                pipe.rotation = r;

                // 5. Apply the puzzle piece rotation 
                obj.transform.Rotate(0, 0, -90 * r, Space.Self);

                grid[x, y] = pipe;
            }
        }

        lastLoadedLevel = level;

        RecalculatePower();
    }


    PipeType CharToType(char c)
    {
        switch (c)
        {
            case 'S': return PipeType.Source;
            case 'K': return PipeType.Sink;
            case 'I': return PipeType.Straight;
            case 'L': return PipeType.Corner;
            case 'T': return PipeType.T;
            case 'X': return PipeType.Cross;
            case 'E': return PipeType.Empty;
        }
        return PipeType.Straight;
    }

    GameObject GetPrefab(PipeType type)
    {
        return prefabs[(int)type];
    }

    public void RecalculatePower()
    {
        foreach (var pipe in grid)
            if (pipe != null) pipe.SetPowered(false);

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // find source
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] != null && grid[x, y].type == PipeType.Source)
                {
                    queue.Enqueue(new Vector2Int(x, y));
                    grid[x, y].SetPowered(true);
                }
            }

        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int pos = queue.Dequeue();
            Pipe pipe = grid[pos.x, pos.y];

            foreach (var dir in dirs)
            {
                Vector2Int next = pos + dir;

                if (next.x < 0 || next.y < 0 || next.x >= size || next.y >= size)
                    continue;

                Pipe neighbor = grid[next.x, next.y];
                if (neighbor == null) continue;

                if (neighbor.isPowered)
                    continue;

                if (pipe.HasConnection(dir) && neighbor.HasConnection(-dir))
                {
                    neighbor.SetPowered(true);
                    queue.Enqueue(next);
                }
            }
        }

        CheckWin();
    }


    void CheckWin()
    {
        foreach (var pipe in grid)
        {
            if (pipe != null && pipe.type == PipeType.Sink && !pipe.isPowered)
                return;
        }

        Debug.Log("LEVEL COMPLETE!");

        WinBehaviour();
    }

    private void WinBehaviour()
    {  
        PrinterPuzzleManager.Instance.BlockDrawer(lastLoadedLevel - 1);

        ToggleCorrectGameState(lastLoadedLevel);

        // Block Inputs
        foreach (var pipe in grid)
        {
            pipe.BlockInputs();
            pipe.UpdateVisual();
        }

    }

    public void GenerateRandomLevels(int currentDifficulty)
    {

        // Ensure randomization is turned on so the pieces get scrambled
        random_level = true;

        char[,] data = LevelGenerator.Generate(size, currentDifficulty);

        LevelSaver.SaveLevel(data, "level" + currentDifficulty.ToString());

    }

    private void ToggleCorrectGameState(int completedLevel)
    {   
        
        
        switch(completedLevel)
        {
            case 1:
                GameManager.Instance.SetState(GameState.PipePuzzleComplete1, true);
                break;
            case 2:
                GameManager.Instance.SetState(GameState.PipePuzzleComplete2, true);
                break;
            case 3:
                GameManager.Instance.SetState(GameState.PipePuzzleComplete3, true);
                break;
            default:
                Debug.LogWarning($"[PipeGridManager]: ToggleCorrectGameState: Invalid completed level index submitted ({completedLevel})!");
                break;
        }

        if (GameManager.Instance.GetState(GameState.PipePuzzleComplete1) && GameManager.Instance.GetState(GameState.PipePuzzleComplete2) && GameManager.Instance.GetState(GameState.PipePuzzleComplete3))
        {
            GameManager.Instance.SetState(GameState.AllPipePuzzlesCompleted, true);
        }
    }
}
