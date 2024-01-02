using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EcosystemSim
{
    public abstract class GameObject
    {
        public Vector2 position;
        public Color color = Color.White;
        public float rotation;
        public SpriteEffects spriteEffects = SpriteEffects.None;
        public Texture2D texture;
        public Animation animation;
        public int scale = 1;
        private float layerDepth = 0;
        
        public bool isRemoved;
        public bool isVisible = true;

        public Vector2 origin;
        public bool centerOrigin;

        public virtual void Update() { }


        public virtual void Draw()
        {
            if (!isVisible) return;
            Texture2D drawTexture = texture ?? animation?.frames[animation.currentFrame];
            //Check if the drawTexture is null in the collisionBox, so there is no need to do it here too.

            //If the bool is true, choose the option on the left, if not then it chooses the right
            origin = centerOrigin ? new Vector2(drawTexture.Width / 2, drawTexture.Height / 2) : Vector2.Zero;

            //Draw the animation texture or the staic texture 
            if (animation != null)
                GameWorld.Instance.spriteBatch.Draw(drawTexture, position, null, color, rotation, origin, scale, spriteEffects, layerDepth);

            else if (texture != null)
                GameWorld.Instance.spriteBatch.Draw(texture, position, null, color, rotation, origin, scale, spriteEffects, layerDepth);
        }
    }
}
