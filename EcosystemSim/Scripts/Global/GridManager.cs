using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using Microsoft.Xna.Framework;


namespace EcosystemSim
{

    public static class GridManager
    {
        public static List<Grid> grids = new List<Grid>();
        public static Grid selectedGrid { get; private set; }

        private static int _gridIndex;
        public static int GridIndex
        {
            get { return _gridIndex; }
            set
            {
                if (_gridIndex != value)
                {
                    _gridIndex = value;
                    OnGridIndexChanged();
                }
            }
        }
        private static float maxLayerDepth = 0.8f;
        private static Color tintColor = Color.Gray;

        public static void InitStartGrids()
        {
            grids.Add(new Grid(TileType.Plain, "Bottom"));
            grids.Add(new Grid("Middle"));
            grids.Add(new Grid("Top"));


            OnGridIndexChanged();
        }

        public static Tile GetTileAtPos(Vector2 pos) => selectedGrid?.GetTile(pos);
        public static List<Tile> GetTilesAtPos(Vector2 pos)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (Grid grid in grids)
            {
                tiles.Add(grid.GetTile(pos));
            }
            return tiles;
        }
        //Liste af vand tiles.
        //Ellers brug plant, og herbivore liste for lister
        public static List<Tile> GetTilesInRadius(Vector2 startPos, int radius)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (Grid grid in grids)
            {
                tiles.AddRange(grid.GetTilesInRadius(startPos, radius));
            }
            return tiles;
        }

        public static int[] GetGridPosAtPos(Vector2 pos) => grids[0].GetGridPos(pos);

        public static Vector2 GetCenterPosInGridLeftTop() => Grid.startPosPx;
        public static Vector2 GetCenterPosInGridRightTop() => Grid.startPosPx + (new Vector2(Tile.NodeSize, Tile.NodeSize) * new Vector2(Grid.collumns - 1, 0));
        public static Vector2 GetCenterPosInGridLeftBottom() => Grid.startPosPx + (new Vector2(Tile.NodeSize, Tile.NodeSize) * new Vector2(0, Grid.rows - 1));
        public static Vector2 GetCenterPosInGridRightBottom() => Grid.startPosPx + (new Vector2(Tile.NodeSize, Tile.NodeSize) * new Vector2(Grid.collumns - 1, Grid.rows - 1));

        public static bool IsWalkable(Vector2 pos)
        {
            // Create a list to store the tiles that are not 'Empty'
            List<Tile> nonEmptyTiles = new List<Tile>();

            foreach (Grid grid in grids)
            {
                if (grid == null)
                {
                    continue;
                }
                Tile tile = grid.GetTile(pos);
                // If the tile is not null and not 'Empty', add it to the list
                if (tile != null && tile.tileType != TileType.Empty)
                {
                    nonEmptyTiles.Add(tile);
                }
            }

            foreach (Tile tile in nonEmptyTiles)
            {
                // Check if the position is within the collision box of the tile
                if (tile.collisionBox.Contains((int)pos.X, (int)pos.Y))
                {
                    // If the tile is not walkable, return false
                    if (!tile.isWalkable)
                    {
                        return false;
                    }
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


        public static void Update()
        {
            foreach (Grid grid in grids)
            {
                grid.UpdateMaxAmountOfPlants();

                if (!grid.hasInitPlants)
                {
                    grid.InitPlants();
                    grid.hasInitPlants = true;
                }
            }
        }

        public static void OnGridIndexChanged()
        {
            if (selectedGrid != null) ChangeTileTing(selectedGrid.tiles, Color.White);

            selectedGrid = grids[GridIndex];

            ChangeTileTing(selectedGrid.tiles, tintColor);

            float depthIncrement = maxLayerDepth / grids.Count; // Calculate depth increment

            for (int i = 0; i < grids.Count; i++)
            {
                if (grids[i] == null) continue;
                grids[i].layerDepth = i * depthIncrement; // Assign layer depth
            }

            foreach (Grid grid in grids)
            {
                if (grid == null) continue;
                grid.UpdateTileLayerDepths();
            }
        }

        private static void ChangeTileTing(Tile[,] tiles, Color tint)
        {
            foreach (Tile tile in tiles)
            {
                tile.color = tint;
                if (tile.selectedPlant != null) tile.selectedPlant.color = tint;
            }
        }

    }
}
