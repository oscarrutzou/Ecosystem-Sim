using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using Microsoft.Xna.Framework;


namespace EcosystemSim
{

    public static class GridManager
    {
        public static List<Grid> grids = new List<Grid>();
        public static Grid selectedGrid {  get; private set; }
        
        public static int gridIndex;
        private static float maxLayerDepth = 0.8f;



        //Gen 3 grids
        //Change grids with keyboard,
        public static void InitStartGrids()
        {
            grids.Add(new Grid(TileType.TestTile, "Bottom"));
            grids.Add(new Grid("Middle"));
            grids.Add(new Grid("Top"));

            UpdateGridToIndex();
        }

        public static Tile GetTileAtPos(Vector2 pos) => selectedGrid.GetTile(pos);


        public static bool IsWalkable(Vector2 pos)
        {
            // Create a list to store the tiles that are not 'Empty'
            List<Tile> nonEmptyTiles = new List<Tile>();

            foreach (Grid grid in grids)
            {
                Tile tile = grid.GetTile(pos);
                // If the tile is not null and not 'Empty', add it to the list
                if (tile != null && tile.tileType != TileType.Empty)
                {
                    nonEmptyTiles.Add(tile);
                }
            }

            // If there's any tile in the list that is not walkable, return false
            foreach (Tile tile in nonEmptyTiles)
            {
                if (!tile.isWalkable)
                {
                    return false;
                }
            }
            //Since there is that if there is none, it would be the border or a TileType.Empty.
            if (nonEmptyTiles.Count > 0) { return true; } else return false;            
        }


        public static Tile GetTileAtPosTest(Vector2 pos)
        {
            foreach (Grid grid in grids)
            {
                Tile tile = grid.GetTile(pos);
                if (tile != null) return tile;
            }
            return null;
        }


        public static void UpdateGridToIndex()
        {
            selectedGrid = grids[gridIndex];

            float depthIncrement = maxLayerDepth / grids.Count; // Calculate depth increment

            for (int i = 0; i < grids.Count; i++)
            {
                grids[i].layerDepth = i * depthIncrement; // Assign layer depth
            }

            foreach (Grid grid in grids)
            {
                grid.UpdateTileLayerDepths();
            }
        }


        public static void ChangeGrid(Grid newGrid)
        {
            selectedGrid = newGrid;

        }

    }
}
