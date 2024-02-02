using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EcosystemSim
{
    public enum TileType
    {
        Empty,
        TestTileNonWalk,
        Plain,
        Grass,
        Water,
    }

    //Fjern switch og brug delegates.

    public class Tile: GameObject
    {
        public static int NodeSize = 48;
        public Grid parentGrid;
        public bool isWalkable;
        public bool canGrowPlants;
        public int[] gridPos;
        public int colm, row;
        public TileType tileType;

        private float plantTimer;
        private float plantTimeToGrow;
        public Plant selectedPlant;
        public bool hasBeenPlanted;

        public Tile parent;
        public Vector2 Center
        {
            get
            {
                return new Vector2(position.X + NodeSize / 2, position.Y + NodeSize / 2);
            }
        }
        public float distanceToTarget;
        public float cost; //Cost like diffuculty or effort that the tile should have. Maybe stuff like mud has a higher cost, so it will avoid mud.
        public float weight;
        public float F
        {
            get
            {
                if (distanceToTarget != -1 && cost != -1)
                    return distanceToTarget + cost;
                else
                    return -1;
            }
        }

        public static List<TileType> grassTileTypes = new List<TileType>() { 
            TileType.Plain,
            TileType.Grass,
        };

        public static List<TileType> waterTileTypes = new List<TileType>() {
            TileType.Water,
        };

        public Tile(Grid parentGrid,int[] gridPos, Vector2 position, TileType type, float weight = 1)
        {
            this.parentGrid = parentGrid;
            this.gridPos = gridPos;
            colm = gridPos[0];
            row = gridPos[1];
            this.position = position;
            distanceToTarget = -1;
            cost = 1;
            this.weight = weight;
            parent = null;
            tileType = type;
            ChangeTileTexture(type);

            Random random = new Random();
            plantTimeToGrow = random.Next(1, 4);
        }

        private void ChangeTileTexture(TileType type)
        {
            isWalkable = false;

            if (selectedPlant != null)
            {
                parentGrid.currentAmountOfPlants--;
                selectedPlant = null;
            }

            plantTimer = 0;
            canGrowPlants = false;

            switch (type)
            {
                case TileType.Empty:
                    texture = GlobalTextures.textures[TextureNames.TileEmpty];
                    //isWalkable = true;
                    break;

                case TileType.Water:
                    texture = GlobalTextures.textures[TextureNames.TileWater];
                    isWalkable = true;
                    break;
                #region TestTiles
                case TileType.TestTileNonWalk:
                    texture = GlobalTextures.textures[TextureNames.TestTileNonWalk];
                    //SetCollisionBox(16,5);
                    break;
                #endregion

                case TileType.Plain:
                    texture = GlobalTextures.textures[TextureNames.TilePlain];
                    isWalkable = true;
                    canGrowPlants = true;
                    //cost = 1;
                    break;
                case TileType.Grass:
                    texture = GlobalTextures.textures[TextureNames.TileGrassy];
                    isWalkable = true;
                    canGrowPlants = true;
                    //cost = 300;
                    break;
            }
        }

        public void ChangeTile(TileType newType)
        {
            if (tileType == newType) return;

            ChangeTileTexture(newType);

            if (tileType != TileType.Empty && newType == TileType.Empty)
            {
                isRemoved = true;
            }

            // Check if the tile is changing from Empty to another type
            if (tileType == TileType.Empty && newType != TileType.Empty)
            {
                // Add the tile to the game objects list
                isRemoved = false;
                SceneData.gameObjectsToAdd.Add(this);
            }

            // Change the tile type
            tileType = newType;
        }
        


        public override void Update()
        {
            if (!canGrowPlants || selectedPlant == null || hasBeenPlanted) return;

            plantTimer += (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

            if (plantTimer >= plantTimeToGrow)
            {
                plantTimer = 0;
                hasBeenPlanted = true;
                SceneData.gameObjectsToAdd.Add(selectedPlant);
            }
        }

        public void DeletePlant()
        {
            if (selectedPlant == null) return;
            hasBeenPlanted = false;
            selectedPlant.isRemoved = true;
            selectedPlant = null;
            parentGrid.currentAmountOfPlants--;

            parentGrid.PlantNewPlant(this);
        }

        //// Check if the tile's type is in the list of water tile types
        //public static bool IsTileTypeGrowableGrass(TileType tileType) => grassTileTypes.Contains(tileType);
        //// Check if the tile's type is in the list of water tile types
        //public static bool IsTileTypeWater(TileType tileType) => waterTileTypes.Contains(tileType);

        public static bool IsTileTypeGrowableGrass(TileType tileType)
        {
            bool temp = grassTileTypes.Contains(tileType); 
            return temp;
        }

        public static bool IsTileTypeWater(TileType tileType)
        {
            bool temp = waterTileTypes.Contains(tileType);
            return temp;
        }

    }
}
