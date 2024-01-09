using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace EcosystemSim
{
    public static class SaveLoad
    {
        public static void SaveGrid(Grid grid, int index, string description)
        {
            // Check if the description already starts with "grid" + index
            string prefix = "grid" + index;
            if (!description.StartsWith(prefix))
            {
                // If not, add the prefix to the description
                description = prefix + "_" + description;
            }

            string gridName = description;
            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appdataPath, "EcoSystemSimData");
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, $"{gridName}.txt"); // Use the grid name in the file name
            FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            try
            {
                stream.SetLength(0);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine($"{grid.tiles[0,0].position.X}, {grid.tiles[0, 0].position.Y}"); //Start pos
                writer.WriteLine($"{grid.width},{grid.height}"); //Width, height

                for (int y = 0; y < grid.height; y++)
                {
                    for (int x = 0; x < grid.width; x++)
                    {
                        Tile tile = grid.tiles[x, y];
                        writer.WriteLine($"{tile.gridPos[0]},{tile.gridPos[1]},{tile.position.X},{tile.position.Y},{tile.tileType}");
                    }
                }
                writer.Flush();
            }
            finally
            {
                stream.Close();
            }
        }

        public static void LoadGrids()
        {
            foreach (Tile tile in SceneData.tiles)
            {
                tile.isRemoved = true;
            }

            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appdataPath, "EcoSystemSimData");

            // Get all files that start with "grid"
            string[] files = Directory.GetFiles(folder, "grid*.txt");

            // Sort the files by the number after "grid"
            Array.Sort(files, (x, y) => int.Parse(Path.GetFileNameWithoutExtension(x).Split('_')[0].Substring(4)) - int.Parse(Path.GetFileNameWithoutExtension(y).Split('_')[0].Substring(4)));

            // If there are too many grids, remove the extra ones
            while (GridManager.grids.Count > files.Length)
            {
                GridManager.grids.RemoveAt(GridManager.grids.Count - 1);
            }

            // If there are not enough grids, add new ones
            while (GridManager.grids.Count < files.Length)
            {
                GridManager.grids.Add(new Grid("Empty"));
            }

            for (int i = 0; i < files.Length; i++)
            {
                // Extract the grid name from the file name
                string gridName = Path.GetFileNameWithoutExtension(files[i]);

                // Load the grid
                Grid loadedGrid = SaveLoad.LoadGrid(i, gridName);

                // Replace the existing grid with the loaded one
                GridManager.grids[i] = loadedGrid;
            }

            GridManager.OnGridIndexChanged();
        }



        private static Grid LoadGrid(int index, string description)
        {
            string gridName = description;
            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appdataPath, "EcoSystemSimData");
            string path = Path.Combine(folder, $"{gridName}.txt"); // Use the grid name in the file name
            if (!File.Exists(path))
            {
                return null;
            }
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            try
            {
                StreamReader reader = new StreamReader(stream);
                
                string[] gridPosStart = reader.ReadLine().Split(',');
                Vector2 gridStartPos = new Vector2(int.Parse(gridPosStart[0]), int.Parse(gridPosStart[1]));

                string[] firstLineSizeParts = reader.ReadLine().Split(',');
                int width = int.Parse(firstLineSizeParts[0]);
                int height = int.Parse(firstLineSizeParts[1]);

                //Need to make it here so the tiles can use the grid
                Grid grid = new Grid(gridStartPos, width, height, false, description);
                
                List<Tile> tiles = new List<Tile>();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    int[] gridPos = new int[] { int.Parse(parts[0]), int.Parse(parts[1]) };
                    Vector2 position = new Vector2(float.Parse(parts[2]), float.Parse(parts[3]));

                    TileType type;
                    if (Enum.TryParse(parts[4], out type))  // Try to parse the tile type
                    {
                        Tile tempTile = new Tile(grid, gridPos, position, type);
                        tiles.Add(tempTile);
                        if (type != TileType.Empty)
                        {
                            SceneData.gameObjectsToAdd.Add(tempTile);
                        }
                    }
                    else
                    {
                        //If there is a error, where the tiletype is no longere there, use a default empty tile insted.
                        Tile tempTile = new Tile(grid, gridPos, position, TileType.Empty);
                        tiles.Add(tempTile);
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        grid.tiles[x, y] = tiles[y * width + x];
                    }
                }
                return grid;
            }
            finally
            {
                stream.Close();
            }
        }


    }

}

