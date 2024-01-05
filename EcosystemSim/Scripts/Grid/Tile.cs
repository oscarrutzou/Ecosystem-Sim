using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;

namespace EcosystemSim
{
    public enum TileType
    {
        Empty,
        TestTile,
        TestTileNonWalk,
    }

    public class Tile: GameObject
    {
        public bool isWalkable;
        public int[] gridPos;
        public TileType tileType;
        public Tile(int[] gridPos, Vector2 position, TileType type)
        {
            this.gridPos = gridPos;
            this.position = position;
            tileType = type;
            ChangeTileTexture(type);
        }
        private void ChangeTileTexture(TileType type)
        {
            switch (type)
            {
                case TileType.Empty: 
                    texture = null;
                    isWalkable = false;
                    break;
                case TileType.TestTile:
                    texture = GlobalTextures.textures[TextureNames.TestTile];
                    isWalkable = true;
                    break;
                case TileType.TestTileNonWalk:
                    texture = GlobalTextures.textures[TextureNames.TestTileNonWalk];
                    isWalkable = false;
                    break;
            }
        }

        public void ChangeTile(TileType newType)
        {
            ChangeTileTexture(newType);
            // Check if the tile is changing from Empty to another type
            if (tileType == TileType.Empty && newType != TileType.Empty)
            {
                // Add the tile to the game objects list
                SceneData.gameObjectsToAdd.Add(this);
            }

            // Change the tile type
            tileType = newType;
        }

    }
}
