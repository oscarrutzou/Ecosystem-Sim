﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace EcosystemSim
{

    public static class GridManager
    {
        public static List<Grid> grids = new List<Grid>();
        public static Grid selectedGrid {  get; private set; }
        public static int gridIndex;

        //Gen 3 grids
        //Change grids with keyboard,
        public static void InitStartGrids()
        {
            grids.Add(new Grid(Vector2.Zero, 2, 2, true, "Bottom"));
            grids.Add(new Grid(Vector2.Zero, 2, 2, true, "Middle"));
            grids.Add(new Grid(Vector2.Zero, 2, 2, true, "Top"));
            foreach (Grid grid in grids) { 
                grid.InitGrid();
            }

            UpdateGridToIndex();
        }

        public static Tile GetTileAtPos(Vector2 pos) => selectedGrid.GetTile(pos);

        public static void UpdateGridToIndex()
        {
            selectedGrid = grids[gridIndex];
        }

    }
}
