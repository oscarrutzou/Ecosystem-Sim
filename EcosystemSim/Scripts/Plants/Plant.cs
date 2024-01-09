using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;

namespace EcosystemSim
{
    public class Plant: GameObject
    {
        public float plantGrowMaxTime;
        public Tile parentTile;

        public Plant(Tile parentTile, Texture2D texture)
        {
            this.parentTile = parentTile;
            position = parentTile.position;
            isCentered = parentTile.isCentered;
            layerDepth = parentTile.layerDepth + 0.01f;
            this.texture = texture;
        }

        public override void Update()
        {
            if (parentTile.selectedPlant == null) isRemoved = true;

        }

        /// <summary>
        /// If the plant is old or if it has been eaten
        /// </summary>
        public void Die()
        {
            parentTile.selectedPlant = null;
            isRemoved = true;
        }
    }
}
