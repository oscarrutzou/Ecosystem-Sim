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
        public bool isWalkable;
        public bool canGrowPlants;
        public bool hasPlant;
        public int[] gridPos;
        public TileType tileType;

        private float plantTimer;
        private float plantTimeToGrow = 1f;
        private Plant selectedPlant;
        public Tile(int[] gridPos, Vector2 position, TileType type)
        {
            this.gridPos = gridPos;
            this.position = position;
            tileType = type;
            ChangeTileTexture(type);
        }

        private void ChangeTileTexture(TileType type)
        {
            isWalkable = false;

            plantTimer = 0;
            canGrowPlants = false;
            hasPlant = false;

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

            if (canGrowPlants)
            {
                Random rnd = new Random();
                int rndNmb = rnd.Next(0, 5);
                if (rndNmb != 0) canGrowPlants = false;
                else
                {
                    selectedPlant = new Plant(this, GlobalTextures.textures[TextureNames.GreensMushroom]);
                }
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
            if (!canGrowPlants || hasPlant) return;
            //if (tileType != TileType.TestTile || hasPlant) return;

            plantTimer += (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds;

            if (plantTimer >= plantTimeToGrow)
            {
                plantTimer = 0;
                hasPlant = true;
                SceneData.gameObjectsToAdd.Add(selectedPlant);
            }
        }
    }
}
