using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;

namespace EcosystemSim
{
    public enum TileType
    {
        TestTile,
        TestTileNonWalk,
    }
    public class Tile: GameObject
    {
        public bool isWalkable;
        public int[] gridPos;
        public TileType type;
        public Tile(int[] gridPos, Vector2 position, TileType type)
        {
            this.gridPos = gridPos;
            this.position = position;
            this.type = type;
            ChangeTileTexture();
        }
        private void ChangeTileTexture()
        {
            switch (type)
            {
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

        public void ChangeTile(TileType type)
        {
            this.type = type;
            ChangeTileTexture();
        }
    }
}
