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
        public int[] gridSize = new int[] { 5, 5 };
        public Vector2 startPosPx;

        public Grid()
        {
            gridSizeDem *= (int)scale.X;
            tiles = new Tile[gridSize[0], gridSize[1]]; // Initialize the 2D array
        }

        public void InitGrid(Vector2 startPosPx, bool isCentered)
        {
            Vector2 curPos = startPosPx;

            if (isCentered)
            {
                curPos = new Vector2(curPos.X - gridSize[0] * gridSizeDem / 2, curPos.Y - gridSize[1] * gridSizeDem / 2);
            }
            this.startPosPx = curPos;

            for (int y = 0; y < gridSize[1]; y++)
            {
                for (int x = 0; x < gridSize[0]; x++)
                {
                    Tile tempTile = new Tile(true, new int[] { x, y }, curPos, TileType.TestTile, GlobalTextures.textures[TextureNames.TestTile]);
                    tiles[x, y] = tempTile;
                    SceneData.gameObjectsToAdd.Add(tempTile);
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

            if (0 <= gridX && gridX < gridSize[0] && 0 <= gridY && gridY < gridSize[1])
            {
                return tiles[gridX, gridY];
            }

            return null; // Position is out of bounds
        }

    }



    /*
     *         
     */
}
