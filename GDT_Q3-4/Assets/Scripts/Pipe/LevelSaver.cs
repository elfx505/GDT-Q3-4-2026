using UnityEngine;
using System.IO;

public static class LevelSaver
{
    public static void SaveLevel(char[,] grid, string fileName)
    {
        // Get the size of the grid (assuming it's a square)
        int size = grid.GetLength(0); 
        string levelContent = "";

        // Loop through the grid row by row (y) and column by column (x)
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                levelContent += grid[x, y];
            }
            
            // Add a new line after every row except the last one
            if (y < size - 1)
            {
                levelContent += "\n";
            }
        }

        // Define the save path to your Assets/Resources/Levels folder
        string folderPath = Application.dataPath + "/Resources/Levels/";

        // Create the directory if it doesn't exist yet
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fullPath = folderPath + fileName + ".txt";

        // Write the text file
        File.WriteAllText(fullPath, levelContent);
        Debug.Log("Saved new level to: " + fullPath);

        // Tell the Unity Editor to refresh so the file appears immediately in the Project window
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
