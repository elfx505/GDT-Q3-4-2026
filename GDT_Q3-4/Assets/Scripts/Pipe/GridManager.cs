using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour    
{
    public static GridManager Instance;
    public GameObject[] prefabs;
    public static int size = 7;
    public Pipe[,] grid = new Pipe[7,7];
    public float spacing = 1.5f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadLevelByDifficulty(1); // start at level 1
    }

    public void LoadLevel(string levelName)
    {
        char[,] data = LevelLoader.LoadLevel(levelName);

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                PipeType type = CharToType(data[x,y]);

                GameObject prefab = GetPrefab(type);

                float offset = (size - 1) / 2f;

                Vector3 pos = new Vector3(
                    (x - offset) * spacing,
                    (y - offset) * spacing,
                    0
                );

                GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

                Pipe pipe = obj.GetComponent<Pipe>();
                pipe.type = type;

                // random rotation (except source)
                if (type != PipeType.Source)
                {
                    int r = Random.Range(0, 4);
                    pipe.rotation = r;
                    obj.transform.Rotate(0, 0, -90 * r);
                }

                grid[x,y] = pipe;
            }
        }

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
            pipe.SetPowered(false);

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // find source
        for (int x = 0; x < 7; x++)
        for (int y = 0; y < 7; y++)
        {
            if (grid[x,y].type == PipeType.Source)
            {
                queue.Enqueue(new Vector2Int(x,y));
                grid[x,y].SetPowered(true);
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

                if (next.x < 0 || next.y < 0 || next.x >= 7 || next.y >= 7)
                    continue;

                Pipe neighbor = grid[next.x, next.y];

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

    public void LoadLevelByDifficulty(int level)
    {
        if (level == 1) LoadLevel("level1");
        if (level == 2) LoadLevel("level2");
        if (level == 3) LoadLevel("level3");
    }
    
    void CheckWin()
    {
        foreach (var pipe in grid)
        {
            if (pipe.type == PipeType.Sink && !pipe.isPowered)
                return;
        }

        Debug.Log("LEVEL COMPLETE!");
        UIManager.Instance.ShowWinText();
    }
}