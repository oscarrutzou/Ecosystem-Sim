using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosystemSim
{
    public class Grid
    {
        public int gridSizeDem = 16;
        private Vector2 scale = new Vector2(3, 3);
        public Tile tiles;
        public int[] gridSize = new int[] { 5,5};
        public Tile hoverOverTile;
        public int[] hoverGridPos;
        
        public Grid()
        {
            gridSizeDem *= (int)scale.X;
        }
        
        public void InitGrid(Vector2 startPosPx)
        {
            Vector2 curPos = startPosPx;

            for (int x = 0; x < gridSize[0];  x++)
            {
                for (int y = 0; y < gridSize[1]; y++)
                {
                    Tile tempTile = new Tile(true, new int[] {x,y}, curPos, TileType.TestTile, GlobalTextures.textures[TextureNames.TestTile]);
                    SceneData.gameObjectsToAdd.Add(tempTile);
                    
                    curPos.X += gridSizeDem;
                }
                curPos.X = startPosPx.X;
                curPos.Y += gridSizeDem;
            }
        }

        public Tile GetTileAtPos(Vector2 pos)
        {
            int gridX = (int)(pos.X / gridSizeDem);
            int gridY = (int)(pos.Y / gridSizeDem);
            if (0 <= gridX && gridX < gridSize[0] && 0 <= gridY && gridY < gridSize[1])
            {
                foreach (Tile tile in SceneData.tiles)
                {
                    if (tile.gridPos[0] == gridX && tile.gridPos[1] == gridY)
                    {
                        hoverGridPos = new int[] { gridX, gridY };
                        return tile;
                    }
                }
            }
            return null;
        }
    }
}
