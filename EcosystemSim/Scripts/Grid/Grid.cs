using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
namespace EcosystemSim
{
    public class Grid
    {
        public int gridSizeDem = 16;
        private int basicSize = 10;
        public int currentAmountOfPlants;
        public int maxAmountOfPlants;
        private int maxAmountOfPlantsDividing = 5;
        private Vector2 scale = new Vector2(3, 3);
        public Tile[,] tiles;
        //public int[] gridSize = new int[] { 5, 5 };
        public int width, height;
        public Vector2 startPosPx;
        public bool isCentered;
        public string gridName;
        private TileType basicTileType;
        
        public float layerDepth;
        private Random rnd = new Random();

        private List<TextureNames> plantTextures = new List<TextureNames>() {
            TextureNames.GreensGrass,
            TextureNames.GreensMushroom,
            TextureNames.GreensYellowFlower,
            TextureNames.GreensRedFlower,
        };

        public Grid(string name)
        {
            this.startPosPx = Vector2.Zero;
            this.width = basicSize;
            this.height = basicSize;
            this.isCentered = true;
            this.basicTileType = TileType.Empty;
            tiles = new Tile[width, height]; // Initialize the 2D array
            gridName = name;

            InitGrid();
        }

        public Grid(TileType basicTileType, string name)
        {
            this.startPosPx = Vector2.Zero;
            this.width = basicSize;
            this.height = basicSize;   
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
            Random rnd = new Random();
            TileType tileType;

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
                maxAmountOfPlants = Math.Max(1, canGrowPlantTiles / maxAmountOfPlantsDividing);
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
            List<Tile> eligibleTiles = new List<Tile>();

            // Find all tiles that are of type TestTile and do not have a plant
            foreach (Tile tile in tiles)
            {
                if (tile.tileType == TileType.Grass || tile.tileType == TileType.Plain)
                {
                    if (tile.selectedPlant == null)
                    {
                        eligibleTiles.Add(tile);
                    }
                }
                
            }

            // Randomly select tiles from the eligible list until we reach maxAmountOfPlants or run out of eligible tiles
            while (currentAmountOfPlants < maxAmountOfPlants && eligibleTiles.Count > 0)
            {
                int index = rnd.Next(eligibleTiles.Count);
                eligibleTiles[index].selectedPlant = new Plant(eligibleTiles[index], PickRandomPlant());
                currentAmountOfPlants++;
                eligibleTiles.RemoveAt(index);  // Remove the tile from the list to avoid selecting it again
            }
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
