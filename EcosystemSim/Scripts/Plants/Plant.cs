using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.MediaFoundation;
using System;

namespace EcosystemSim
{
    public class Plant: GameObject
    {
        public float plantGrowMaxTime;
        public Tile parentTile;
        //public bool isEaten;
        public bool isBeingEaten;
        public Plant(Tile parentTile, Texture2D texture)
        {
            layerDepth = parentTile.layerDepth + 0.01f;
            this.texture = texture;
            this.parentTile = parentTile;
            position = parentTile.position;
            isCentered = !parentTile.isCentered;
            CenterPosition();
        }

        public override void Update()
        {
            if (parentTile.selectedPlant == null) isRemoved = true;

        }

        public void CenterPosition()
        {
            Texture2D drawTexture = texture ?? animation?.frames[animation.currentFrame];
            if (drawTexture is null)
                throw new InvalidOperationException("GameObject must have a valid texture or animation.");

            int width = drawTexture.Width;
            int height = drawTexture.Height;

            origin = isCentered ? new Vector2(width / 2, height / 2) : Vector2.Zero;
            position = new Vector2((int)(position.X + origin.X * scale), (int)(position.Y + origin.Y * scale));
        }

        /// <summary>
        /// If the plant is old or if it has been eaten
        /// </summary>
        public void DeletePlant() => parentTile.DeletePlant();
    }
}
