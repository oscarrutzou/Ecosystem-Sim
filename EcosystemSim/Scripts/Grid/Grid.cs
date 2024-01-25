﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
namespace EcosystemSim
{
    public class Grid
    {
        public static bool isInitialized = false;
        public static Vector2 startPosPx = Vector2.Zero;
        public static int collumns = 10;
        public static int rows = 10;
        public static bool isCentered = true;


        public int gridSizeDem = Tile.NodeSize;
        public int currentAmountOfPlants;
        public int maxAmountOfPlants;
        private int maxAmountOfPlantsDividing = 5;
        public Tile[,] tiles;

        public string gridName;
        private TileType basicTileType;
        
        public float layerDepth;
        private Random rnd = new Random();

        public bool hasInitPlants;

        private List<TextureNames> plantTextures = new List<TextureNames>() {
            TextureNames.GreensGrass,
            TextureNames.GreensMushroom,
            TextureNames.GreensYellowFlower,
            TextureNames.GreensRedFlower,
        };

        public Grid(string name)
        {
            this.basicTileType = TileType.Empty;
            tiles = new Tile[rows, collumns]; // Initialize the 2D array
            gridName = name;

            InitGrid();
        }
        public Grid(TileType basicTileType, string name)
        {
            this.basicTileType = basicTileType;
            tiles = new Tile[rows, collumns]; // Initialize the 2D array
            gridName = name;

            InitGrid();
        }

        private void InitGrid()
        {
            if (collumns < 2 || rows < 2) throw new ArgumentException("It should atleast have 4 tiles");

            if (isCentered && !isInitialized)
            {
                startPosPx = new Vector2(startPosPx.X - rows * gridSizeDem / 2, startPosPx.Y - collumns * gridSizeDem / 2);
                isInitialized = true;
            }

            Random rnd = new Random();
            TileType tileType;

            Vector2 curPos = startPosPx;

            for (int y = 0; y < collumns; y++)
            {
                for (int x = 0; x < rows; x++)
                {
                    if (basicTileType == TileType.Plain)
                    {
                        if (rnd.Next(0, 3) != 0) tileType = TileType.Plain;
                        else tileType = TileType.Grass;
                    }
                    else
                    {
                        tileType = basicTileType;
                    }

                    Tile tempTile = new Tile(this, new int[] { x, y }, curPos, tileType);
                    tiles[x, y] = tempTile;
                    if (basicTileType != TileType.Empty) SceneData.gameObjectsToAdd.Add(tempTile);

                    curPos.X += gridSizeDem;
                }
                curPos.X = startPosPx.X;
                curPos.Y += gridSizeDem;
            }

            UpdateMaxAmountOfPlants();

            
        }

        public Tile GetTile(Vector2 pos)
        {
            if (pos.X < startPosPx.X || pos.Y < startPosPx.Y)
            {
                return null; // Position is negative, otherwise it will make a invisable tile in the debug, since it cast to int, then it gets rounded to 0 and results in row and column
            }

            int gridX = (int)((pos.X - startPosPx.X) / gridSizeDem);
            int gridY = (int)((pos.Y - startPosPx.Y) / gridSizeDem);

            if (0 <= gridX && gridX < rows && 0 <= gridY && gridY < collumns)
            {
                return tiles[gridX, gridY];
            }

            return null; // Position is out of bounds
        }

        //Denne her bruger så meget data. Fix maybe just have 
        public int[] GetGridPos(Vector2 pos)
        {
            if (pos.X < startPosPx.X || pos.Y < startPosPx.Y)
            {
                return null; // Position is negative, otherwise it will make a invisable tile in the debug, since it cast to int, then it gets rounded to 0 and results in row and column
            }

            int gridX = (int)((pos.X - startPosPx.X) / gridSizeDem);
            int gridY = (int)((pos.Y - startPosPx.Y) / gridSizeDem);
            return new int[] { gridX, gridY };
        }

        public void GetGridPos(Vector2 pos, out int gridX, out int gridY)
        {
            if (pos.X < startPosPx.X || pos.Y < startPosPx.Y)
            {
                gridX = gridY = -1; // Position is negative, otherwise it will make an invisible tile in the debug, since it casts to int, then it gets rounded to 0 and results in row and column
                return;
            }

            gridX = (int)((pos.X - startPosPx.X) / gridSizeDem);
            gridY = (int)((pos.Y - startPosPx.Y) / gridSizeDem);
        }


        public List<Tile> GetTilesInRadius(Vector2 pos, int radius)
        {
            List<Tile> tilesInRadius = new List<Tile>();
            Tile curTile = GetTile(pos);
            if (curTile == null) throw new Exception("Not a tile under pos");
            Vector2 absCurTile = new Vector2(Math.Abs(curTile.centerPos.X), Math.Abs(curTile.centerPos.Y));
            // Calculate the range of positions that the radius covers
            Vector2 minPos = -(absCurTile + new Vector2(radius));

            Vector2 maxPos = absCurTile + new Vector2(radius);

            // Create a single Vector2 object outside the loop
            Vector2 temp = new Vector2();

            // Iterate over the positions in the range
            for (float x = minPos.X; x <= maxPos.X; x += gridSizeDem)
            {
                for (float y = minPos.Y; y <= maxPos.Y; y += gridSizeDem)
                {
                    // Update the values of the existing Vector2 object
                    temp.X = x;
                    temp.Y = y;
                    int gridX, gridY;
                    GetGridPos(temp, out gridX, out gridY);
                    if (gridX == curTile.gridPos[0] && gridY == curTile.gridPos[1]) continue;

                    // Check if the grid position is within the grid bounds
                    if (gridX >= 0 && gridX < rows && gridY >= 0 && gridY < collumns)
                    {
                        Tile tile = GetTile(gridX, gridY);

                        // Check if the tile is not null and not 'Empty'
                        if (tile != null && tile.isWalkable)
                        {
                            // Check if the position is within the radius from the center position
                            if ((int)Vector2.Distance(curTile.Center, temp) <= radius)
                            {
                                tilesInRadius.Add(tile);
                            }
                        }
                    }
                }
            }

            return tilesInRadius;
        }

        public Tile GetTile(int x, int y) => tiles[x, y];
        public void UpdateTileLayerDepths()
        {
            foreach (Tile tile in tiles)
            {
                tile.layerDepth = layerDepth;
            }
        }

        /// <summary>
        /// If the tiles that can grow plants is changed, then it also need to change the maxAmountOfPlants in the grid.
        /// </summary>
        public void UpdateMaxAmountOfPlants()
        {
            //maxAmountOfPlants = Math.Max(width, width * height / 5);
            int canGrowPlantTiles = tiles.Cast<Tile>().Count(tile => tile.canGrowPlants);
            if (canGrowPlantTiles > 0)
            {
                maxAmountOfPlants = Math.Max(1, canGrowPlantTiles / maxAmountOfPlantsDividing);
            }
            else
            {
                maxAmountOfPlants = canGrowPlantTiles;
            }
        }

        public void InitPlants()
        {
            List<Tile> eligibleTiles = new List<Tile>();
            eligibleTiles = SceneData.tiles.Where(tile => tile.isWalkable && tile.selectedPlant == null && Tile.IsTileTypeGrowableGrass(tile.tileType)).ToList();

            // Randomly select tiles from the eligible list until we reach maxAmountOfPlants or run out of eligible tiles
            while (currentAmountOfPlants < maxAmountOfPlants && eligibleTiles.Count > 0)
            {
                int index = rnd.Next(eligibleTiles.Count);
                eligibleTiles[index].selectedPlant = new Plant(eligibleTiles[index], PickRandomPlant());
                currentAmountOfPlants++;
                eligibleTiles.RemoveAt(index);  // Remove the tile from the list to avoid selecting it again
            }
        }

        public void PlantNewPlant(Tile curTile)
        {
            if (currentAmountOfPlants >= maxAmountOfPlants) return;

            List<Tile> eligibleTiles = GetTilesInRadius(curTile.centerPos, 100);
            List<Tile> closeTiles = eligibleTiles.Where(t => t.selectedPlant == null && Tile.IsTileTypeGrowableGrass(t.tileType)).ToList();

            int index = rnd.Next(closeTiles.Count);
            closeTiles[index].selectedPlant = new Plant(closeTiles[index], PickRandomPlant());
            currentAmountOfPlants++;
        }


        private Texture2D PickRandomPlant()
        {
            int index;
            if (rnd.Next(0,2) == 0)
            {
                index = 0;
            }
            else
            {
                index = rnd.Next(1, plantTextures.Count);
            }
            return GlobalTextures.textures[plantTextures[index]];
        }
    }
}
