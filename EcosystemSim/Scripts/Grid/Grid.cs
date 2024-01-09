using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
namespace EcosystemSim
{
    public class Grid
    {
        public int gridSizeDem = 16;
        public int currentAmountOfPlants;
        public int maxAmountOfPlants;
        private Vector2 scale = new Vector2(3, 3);
        public Tile[,] tiles;
        //public int[] gridSize = new int[] { 5, 5 };
        public int width, height;
        public Vector2 startPosPx;
        public bool isCentered;
        public string gridName;
        private TileType basicTileType;
        
        public float layerDepth;

        public Grid(string name)
        {
            this.startPosPx = Vector2.Zero;
            this.width = 10;
            this.height = 10;
            this.isCentered = true;
            this.basicTileType = TileType.Empty;
            tiles = new Tile[width, height]; // Initialize the 2D array
            gridName = name;

            InitGrid();
        }
        public Grid(TileType basicTileType, string name)
        {
            this.startPosPx = Vector2.Zero;
            this.width = 10;
            this.height = 10;
            this.isCentered = true;
            this.basicTileType = basicTileType;
            tiles = new Tile[width, height]; // Initialize the 2D array
            gridName = name;

            InitGrid();
        }

        public Grid(Vector2 startPosPx, int width, int height, bool isCentered, string name)
        {
            this.startPosPx = startPosPx;
            this.width = width;
            this.height = height;
            this.isCentered = isCentered;
            this.basicTileType = TileType.Empty;
            tiles = new Tile[width, height]; // Initialize the 2D array
            gridName = name;

            InitGrid();

        }
        public Grid(Vector2 startPosPx, int width, int height, bool isCentered, TileType basicTileType, string name)
        {

            this.startPosPx = startPosPx;
            this.width = width;
            this.height = height;
            this.isCentered = isCentered;
            this.basicTileType = basicTileType;
            tiles = new Tile[width, height]; // Initialize the 2D array
            gridName = name;

            InitGrid();
        }

        private void InitGrid()
        {
            gridSizeDem *= (int)scale.X;
            
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
                    Tile tempTile = new Tile(this, new int[] { x, y }, curPos, basicTileType);
                    tiles[x, y] = tempTile;
                    if (basicTileType != TileType.Empty) SceneData.gameObjectsToAdd.Add(tempTile);

                    curPos.X += gridSizeDem;
                }
                curPos.X = this.startPosPx.X;
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

            if (0 <= gridX && gridX < width && 0 <= gridY && gridY < height)
            {
                return tiles[gridX, gridY];
            }

            return null; // Position is out of bounds
        }
        public void UpdateMaxAmountOfPlants()
        {
            //maxAmountOfPlants = Math.Max(width, width * height / 5);
            int canGrowPlantTiles = tiles.Cast<Tile>().Count(tile => tile.canGrowPlants);
            if (canGrowPlantTiles > 0)
            {
                maxAmountOfPlants = Math.Max(1, canGrowPlantTiles / 5);
            }
            else
            {
                maxAmountOfPlants = canGrowPlantTiles;
            }
        }
        public void UpdateTileLayerDepths()
        {
            foreach (Tile tile in tiles)
            {
                tile.layerDepth = layerDepth;
            }
        }

        public void UpdatePlantTiles()
        {
            Random rnd = new Random();
            List<Tile> eligibleTiles = new List<Tile>();

            // Find all tiles that are of type TestTile and do not have a plant
            foreach (Tile tile in tiles)
            {
                if (tile.tileType == TileType.TestTile && tile.selectedPlant == null)
                {
                    eligibleTiles.Add(tile);
                }
            }

            // Randomly select tiles from the eligible list until we reach maxAmountOfPlants or run out of eligible tiles
            while (currentAmountOfPlants < maxAmountOfPlants && eligibleTiles.Count > 0)
            {
                int index = rnd.Next(eligibleTiles.Count);
                eligibleTiles[index].selectedPlant = new Plant(eligibleTiles[index], GlobalTextures.textures[TextureNames.GreensMushroom]);
                currentAmountOfPlants++;
                eligibleTiles.RemoveAt(index);  // Remove the tile from the list to avoid selecting it again
            }
        }

    }
}
