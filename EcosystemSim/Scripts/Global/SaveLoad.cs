using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace EcosystemSim
{
    public static class SaveLoad
    {
        public static void SaveGrid(Grid grid)
        {
            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appdataPath, "EcoSystemSimData");
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "gridData.txt");
            FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            try
            {
                stream.SetLength(0);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine($"{grid.width},{grid.height}");
                for (int y = 0; y < grid.height; y++)
                {
                    for (int x = 0; x < grid.width; x++)
                    {
                        Tile tile = grid.tiles[x, y];
                        writer.WriteLine($"{tile.gridPos[0]},{tile.gridPos[1]},{tile.position.X},{tile.position.Y},{tile.type}");
                    }
                }
                writer.Flush();
            }
            finally
            {
                stream.Close();
            }
        }

        public static Grid LoadGrid()
        {
            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appdataPath, "EcoSystemSimData");
            string path = Path.Combine(folder, "gridData.txt");
            if (!File.Exists(path))
            {
                return null;
            }
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            try
            {
                StreamReader reader = new StreamReader(stream);
                string[] sizeParts = reader.ReadLine().Split(',');
                int width = int.Parse(sizeParts[0]);
                int height = int.Parse(sizeParts[1]);
                List<Tile> tiles = new List<Tile>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    int[] gridPos = new int[] { int.Parse(parts[0]), int.Parse(parts[1]) };
                    Vector2 position = new Vector2(float.Parse(parts[2]), float.Parse(parts[3]));
                    TileType type = (TileType)Enum.Parse(typeof(TileType), parts[4]);
                    Tile tempTile = new Tile(gridPos, position, type);
                    tiles.Add(tempTile);
                    SceneData.gameObjectsToAdd.Add(tempTile); // Add the tile to your game objects list
                }
                Grid grid = new Grid(tiles[0].position, width, height, false);
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

