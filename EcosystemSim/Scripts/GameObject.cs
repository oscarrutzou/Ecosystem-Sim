using System;
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
        public int scale = 3;
        internal float layerDepth = 0;
        
        public bool isRemoved;
        public bool isVisible = true;

        public Vector2 origin;
        public bool isCentered;

        private int collisionBoxWidth;
        private int collisionBoxHeight;
        private Vector2 offset;
        public Rectangle collisionBox
        {
            get
            {
                // Try to get the width and height of the texture or the current frame of the animation.
                Texture2D drawTexture = texture ?? animation?.frames[animation.currentFrame];
                if (drawTexture is null)
                    throw new InvalidOperationException("GameObject must have a valid texture or animation.");

                // If the collision box width or height is bigger 0, use the width and height of the texture.
                int width = collisionBoxWidth > 0 ? collisionBoxWidth : drawTexture.Width;
                int height = collisionBoxHeight > 0 ? collisionBoxHeight : drawTexture.Height;

                origin = isCentered ? new Vector2(width / 2, height / 2) : Vector2.Zero;
                return new Rectangle(
                    (int)(position.X + offset.X - origin.X * scale),
                    (int)(position.Y + offset.Y - origin.Y * scale),
                    (width * scale),
                    (height * scale)
                );
            }
            set { }
        }

        public virtual void Update() { }


        public virtual void Draw()
        {
            if (!isVisible) return;
            Texture2D drawTexture = texture ?? animation?.frames[animation.currentFrame];
            //Check if the drawTexture is null in the collisionBox, so there is no need to do it here too.

            //If the bool is true, choose the option on the left, if not then it chooses the right
            origin = isCentered ? new Vector2(drawTexture.Width / 2, drawTexture.Height / 2) : Vector2.Zero;

            //Draw the animation texture or the staic texture 
            if (animation != null)
                GameWorld.Instance.spriteBatch.Draw(drawTexture, position, null, color, rotation, origin, scale, spriteEffects, layerDepth);

            else if (texture != null)
                GameWorld.Instance.spriteBatch.Draw(texture, position, null, color, rotation, origin, scale, spriteEffects, layerDepth);

            //DrawDebugCollisionBox(Color.Black);
        }

        public void SetCollisionBox(int width, int height)
        {
            collisionBoxWidth = width;
            collisionBoxHeight = height;
        }

        public void SetCollisionBox(int width, int height, Vector2 offset)
        {
            collisionBoxWidth = width;
            collisionBoxHeight = height;
            this.offset = offset;
        }
        public virtual void CheckCollisionBox() { }

        public void RotateTowardsTarget(Vector2 target)
        {
            if (position == target) return;

            Vector2 dir = target - position;
            rotation = (float)Math.Atan2(-dir.Y, -dir.X) + MathHelper.Pi;
        }

        internal void DrawDebugCollisionBox(Color color)
        {
            //This has been done in a weird way, because we at the start tried to use rotatiting box colliders.
            // Draw debug collision box
            Texture2D pixel = new Texture2D(GameWorld.Instance.gfxDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            // Get the corners of the rectangle
            Vector2[] corners = new Vector2[4];
            corners[0] = new Vector2(collisionBox.Left, collisionBox.Top);
            corners[1] = new Vector2(collisionBox.Right, collisionBox.Top);
            corners[2] = new Vector2(collisionBox.Right, collisionBox.Bottom);
            corners[3] = new Vector2(collisionBox.Left, collisionBox.Bottom);

            // Rotate the corners around the center of the rectangle
            Vector2 origin = new Vector2(collisionBox.Center.X, collisionBox.Center.Y);
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = corners[i] - origin;
                corners[i] = dir + origin;
            }

            // Draw the rotated rectangle
            for (int i = 0; i < 4; i++)
            {
                Vector2 start = corners[i];
                Vector2 end = corners[(i + 1) % 4];
                DrawLine(pixel, start, end, color);
            }
        }

        private void DrawLine(Texture2D pixel, Vector2 start, Vector2 end, Color color)
        {
            float length = Vector2.Distance(start, end);
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            GameWorld.Instance.spriteBatch.Draw(pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 1);
        }
    }
}
