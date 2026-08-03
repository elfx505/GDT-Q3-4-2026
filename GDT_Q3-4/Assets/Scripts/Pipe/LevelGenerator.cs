using System.Collections.Generic;
using UnityEngine;

public static class LevelGenerator
{
    public static char[,] Generate(int size, int difficulty)
    {
        char[,] grid = new char[size, size];
        
        // 1. Initialize empty grid
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                grid[x, y] = 'E';

        // 2. Pick Start (Source on the left) and End (Sink on the right)
        Vector2Int startPos = new Vector2Int(0, Random.Range(0, size));
        Vector2Int endPos = new Vector2Int(size - 1, Random.Range(0, size));

        // 3. Generate a guaranteed solvable path
        List<Vector2Int> path = new List<Vector2Int>();
        bool[,] visited = new bool[size, size];
        FindPath(startPos, endPos, size, visited, path);

        // Arrays to figure out what shape each pipe on the path needs to be
        int[,] connectionCounts = new int[size, size];
        bool[,] isStraight = new bool[size, size];

        // 4. Map the path to required connections
        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int current = path[i];

            // Ignore the very first and last blocks (Source and Sink)
            if (i > 0 && i < path.Count - 1) 
            {
                Vector2Int prev = path[i - 1];
                Vector2Int next = path[i + 1];

                Vector2Int dir1 = prev - current;
                Vector2Int dir2 = next - current;

                connectionCounts[current.x, current.y] = 2; // In and Out
                isStraight[current.x, current.y] = (dir1 == -dir2); // True if opposite directions
            }
        }

        // 5. Add "Fake" Branches for Difficulty (Converts I/L into T/X)
        // Higher difficulty = more deceptive branching
        int extraBranches = difficulty * 2; 
        for (int i = 1; i < path.Count - 1; i++)
        {
            if (extraBranches <= 0) break;

            Vector2Int pos = path[i];
            if (Random.value > 0.4f && connectionCounts[pos.x, pos.y] < 4)
            {
                connectionCounts[pos.x, pos.y]++;
                extraBranches--;
            }
        }

        // 6. Convert logical shapes back to your Character system
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (pos == startPos) grid[x, y] = 'S';
                else if (pos == endPos) grid[x, y] = 'K';
                else if (connectionCounts[x, y] > 0)
                {
                    int conns = connectionCounts[x, y];
                    if (conns == 2) grid[x, y] = isStraight[x, y] ? 'I' : 'L';
                    else if (conns == 3) grid[x, y] = 'T';
                    else if (conns >= 4) grid[x, y] = 'X';
                }
                else
                {
                    // Fill remaining dead space with random noise pipes
                    if (Random.value < 0.8f) // 80% chance for a fake piece
                    {
                        char[] randomPipes = { 'I', 'L', 'T', 'X' };
                        grid[x, y] = randomPipes[Random.Range(0, randomPipes.Length)];
                    }
                }
            }
        }

        return grid;
    }

    // Randomized Depth-First Search algorithm
    static bool FindPath(Vector2Int current, Vector2Int target, int size, bool[,] visited, List<Vector2Int> path)
    {
        path.Add(current);
        visited[current.x, current.y] = true;

        if (current == target) return true; // Path found!

        List<Vector2Int> dirs = new List<Vector2Int>
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        // Shuffle directions to create a random, winding path instead of a straight line
        for (int i = 0; i < dirs.Count; i++)
        {
            Vector2Int temp = dirs[i];
            int randomIndex = Random.Range(i, dirs.Count);
            dirs[i] = dirs[randomIndex];
            dirs[randomIndex] = temp;
        }

        // Recursively check neighbors
        foreach (var dir in dirs)
        {
            Vector2Int next = current + dir;
            // Check bounds and make sure we haven't visited it yet
            if (next.x >= 0 && next.x < size && next.y >= 0 && next.y < size && !visited[next.x, next.y])
            {
                if (FindPath(next, target, size, visited, path))
                    return true;
            }
        }

        // Backtrack if we hit a dead end
        path.RemoveAt(path.Count - 1);
        return false;
    }
}
