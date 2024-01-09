using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;

namespace EcosystemSim
{
    public enum TileType
    {
        Empty,
        TestTile,
        TestTileNonWalk,
        Grass,
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

                #region TestTiles
                case TileType.TestTile:
                    texture = GlobalTextures.textures[TextureNames.TestTile];
                    isWalkable = true;
                    canGrowPlants = true;

                    break;
                case TileType.TestTileNonWalk:
                    texture = GlobalTextures.textures[TextureNames.TestTileNonWalk];
                    SetCollisionBox(16,5);
                    break;
                #endregion

                case TileType.Grass:
                    texture = GlobalTextures.textures[TextureNames.TestTile];
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
            if (!canGrowPlants || selectedPlant == null) return;
            //if (tileType != TileType.TestTile || hasPlant) return;

            plantTimer += (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds;

            if (plantTimer >= plantTimeToGrow)
            {
                plantTimer = 0;
                SceneData.gameObjectsToAdd.Add(selectedPlant);
            }
        }
    }
}
