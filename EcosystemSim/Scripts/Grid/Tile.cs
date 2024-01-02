using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;

namespace EcosystemSim
{
    public enum TileType
    {
        TestTile,
    }
    public class Tile: GameObject
    {
        public bool isWalkable;
        public int[] gridPos;
        public TileType type;
        public Tile(bool isWalkable, int[] gridPos, Vector2 position, TileType type, Texture2D texture)
        {
            this.isWalkable = isWalkable;
            this.gridPos = gridPos;
            this.position = position;
            this.type = type;
            this.texture = texture;
            scale = 3;
        }

    }
}
