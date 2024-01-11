using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;

namespace EcosystemSim
{
    public enum TileType
    {
        Empty,
        //TestTile,
        TestTileNonWalk,
        Plain,
        Grass,
        Water,
    }

    public class Tile: GameObject
    {
        public Grid parentGrid;
        public bool isWalkable;
        public bool canGrowPlants;
        public int[] gridPos;
        public TileType tileType;

        private float plantTimer;
        private float plantTimeToGrow;
        public Plant selectedPlant;
        public bool hasBeenPlanted;
        public Tile(Grid parentGrid,int[] gridPos, Vector2 position, TileType type)
        {
            this.parentGrid = parentGrid;
            this.gridPos = gridPos;
            this.position = position;
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
                    texture = null;
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
                    break;
                case TileType.Grass:
                    texture = GlobalTextures.textures[TextureNames.TileGrassy];
                    isWalkable = true;
                    canGrowPlants = true;
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

            plantTimer += (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds;

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
        }
    }
}
