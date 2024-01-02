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
        public int[,] gridSize = new int[5, 5];
        public Grid()
        {
            gridSizeDem *= (int)scale.X;
        }
        
        public void InitGrid(Vector2 startPosPx)
        {
            Vector2 curPos = startPosPx;
            
            for (int x = 0; x < gridSize.GetLength(0);  x++)
            {
                for (int y = 0; y < gridSize.GetLength(1); y++)
                {
                    Tile tempTile = new Tile(true, new int[x,y], curPos, GlobalTextures.textures[TextureNames.TestTile]);
                    SceneData.gameObjects.Add(tempTile);
                    
                    curPos.X += gridSizeDem;
                }
                curPos.X = startPosPx.X;
                curPos.Y += gridSizeDem;
            }
        }

        //public Tile GetTileAtPos(Vector2 pos)
        //{
            
        //}
    }
}
