using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;

namespace EcosystemSim
{
    public class Grid
    {
        public int gridSizeDem = 16;
        private Vector2 scale = new Vector2(3, 3);
        public Tile[,] tiles;
        //public int[] gridSize = new int[] { 5, 5 };
        public int width, height;
        public Vector2 startPosPx;
        public bool isCentered;
        public string gridName;
        public Grid(Vector2 startPosPx, int width, int height, bool isCentered, string name)
        {
            gridSizeDem *= (int)scale.X;
            this.startPosPx = startPosPx;
            this.width = width;
            this.height = height;
            this.isCentered = isCentered;
            tiles = new Tile[width, height]; // Initialize the 2D array
            gridName = name;
        }

        public void InitGrid()
        {
            Vector2 curPos = startPosPx;

            if (isCentered)
            {
                curPos = new Vector2(curPos.X - width * gridSizeDem / 2, curPos.Y - height * gridSizeDem / 2);
            }
            this.startPosPx = curPos;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Tile tempTile = new Tile(new int[] { x, y }, curPos, TileType.Empty);
                    tiles[x, y] = tempTile;
                    //if (type != TileType.Empty) SceneData.gameObjectsToAdd.Add(tempTile);

                    curPos.X += gridSizeDem;
                }
                curPos.X = this.startPosPx.X;
                curPos.Y += gridSizeDem;
            }
        }

        public Tile GetTile(Vector2 pos)
        {
            if (pos.X < startPosPx.X || pos.Y < startPosPx.Y)
            {
                return null; // Position is negative, otherwise it will make a invisable tile in the debug, since it cast to int, then it gets rounded to 0 and results in row and column
            }

            int gridX = (int)((pos.X - startPosPx.X) / gridSizeDem);
            int gridY = (int)((pos.Y - startPosPx.Y) / gridSizeDem);

            if (0 <= gridX && gridX < width && 0 <= gridY && gridY < height)
            {
                return tiles[gridX, gridY];
            }

            return null; // Position is out of bounds
        }

    }
}
