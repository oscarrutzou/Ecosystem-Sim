using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace EcosystemSim
{
    public static class GridManager
    {
        public static List<Grid> grids = new List<Grid>();

        public static Tile GetTileAtPos(Vector2 pos)
        {
            foreach (Grid grid in grids)
            {
                Tile tile = grid.GetTileAtPos(pos);
                if (tile != null && tile.isWalkable)
                {
                    return tile;
                }
            }
            return null;
        }

        public static Grid GetGridAtPos(Vector2 pos)
        {
            foreach (Grid grid in grids)
            {
                Vector2 gridDimensions = new Vector2(grid.gridSize[0] * grid.gridSizeDem, grid.gridSize[1] * grid.gridSizeDem);
                if (pos.X >= grid.startPosPx.X && pos.Y >= grid.startPosPx.Y && pos.X < grid.startPosPx.X + gridDimensions.X && pos.Y < grid.startPosPx.Y + gridDimensions.Y)
                {
                    return grid;
                }
            }
            return null;
        }

    }
}
