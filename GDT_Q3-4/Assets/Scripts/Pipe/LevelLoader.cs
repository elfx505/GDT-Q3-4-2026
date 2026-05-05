using UnityEngine;

public class LevelLoader
{
    public static char[,] LoadLevel(string levelName)
    {
        TextAsset file = Resources.Load<TextAsset>("Levels/" + levelName);
        string[] lines = file.text.Split('\n');

        char[,] grid = new char[7, 7];

        for (int y = 0; y < 7; y++)
        {
            string line = lines[y].Trim();

            for (int x = 0; x < 7; x++)
            {
                grid[x, y] = line[x];
            }
        }

        return grid;
    }
}