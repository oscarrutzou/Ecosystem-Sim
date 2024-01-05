using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EcosystemSim
{
    public abstract class Agent : GameObject
    {
        private int maxHealth = 100;
        public int health;
        public int damage = 10;
        public int thirstMeter;
        public int hungermeter;
        public int searchRadPx = 100;
        public int speed = 30;
        private double changeDirectionTimer = 0;
        private Vector2 direction;

        public Agent() {
            layerDepth = 0.2f;
            health = maxHealth;
            isCentered = true;
        }

        public override void Draw()
        {
            base.Draw();
            DrawSearchRad();
            DrawDebugCollisionBox(Color.AliceBlue);
        }


        public override void Update()
        {
            base.Update();
            IdleWalk();
        }

        public void IdleWalk()
        {
            // Decrease the timer by the elapsed time
            changeDirectionTimer -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds;

            // If the timer is less than or equal to zero, change direction
            if (changeDirectionTimer <= 0)
            {
                ChangeDirection();
            }

            Vector2 tempPos = position;
            Vector2 nextPos = position + direction * speed * (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds;

            // Check if the next position is walkable
            if (CheckTileIsWalkable(nextPos))
            {
                position = nextPos;
            }
            else
            {
                position = tempPos;
                ChangeDirection();
            }
        }


        private void ChangeDirection()
        {
            // Create a random direction vector
            Random random = GameWorld.Instance.random;
            direction = new Vector2((float)(random.NextDouble() - 0.5), (float)(random.NextDouble() - 0.5));
            direction.Normalize(); // Make sure the direction vector has a length of 1                   
            changeDirectionTimer = random.Next(10, 60);// Reset the timer with a random value between 1 and 5 seconds
        }

        public void SearchForType(GameObject targetObject)
        {

        }

        private void DrawSearchRad()
        {
            Vector2 pos = new Vector2(position.X - (GlobalTextures.textures[TextureNames.UISearchRad100].Width / 2), position.Y - (GlobalTextures.textures[TextureNames.UISearchRad100].Height / 2));
            GameWorld.Instance.spriteBatch.Draw(GlobalTextures.textures[TextureNames.UISearchRad100], pos, null, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }

        public bool CheckTileIsWalkable(Vector2 nextPos)
        {
            // Calculate the positions of the four corners of the agent's bounding box
            Vector2 topLeft = nextPos + new Vector2(-texture.Width / 2 * scale, -texture.Height / 2 * scale);
            Vector2 topRight = nextPos + new Vector2(texture.Width / 2 * scale, -texture.Height / 2 * scale);
            Vector2 bottomLeft = nextPos + new Vector2(-texture.Width / 2 * scale, texture.Height / 2 * scale);
            Vector2 bottomRight = nextPos + new Vector2(texture.Width / 2 * scale, texture.Height / 2 * scale);

            // Check if any of the corners would be in a non-walkable tile
            if (!IsPositionWalkable(topLeft) || !IsPositionWalkable(topRight) || !IsPositionWalkable(bottomLeft) || !IsPositionWalkable(bottomRight))
            {
                return false;
            }

            // Otherwise, return true
            return true;
        }

        private bool IsPositionWalkable(Vector2 pos)
        {
            // Get the tile at the position
            Tile tile = GridManager.GetTileAtPos(pos);
            //Tile tile = testScene?.bgGrid.GetTile(pos);

            // If there is no tile at the position or the tile is not walkable, return false
            if (tile == null || !tile.isWalkable || tile.tileType == TileType.Empty)
            {
                return false;
            }

            // Otherwise, return true
            return true;
        }



    }
}
