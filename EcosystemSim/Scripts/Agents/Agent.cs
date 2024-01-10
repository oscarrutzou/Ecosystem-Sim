using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace EcosystemSim
{
    public enum AgentState
    {
        IdleWalk, //Random around, herbivore should maneuver away from predators.
        Search, //Can be for food
        Eating, //Stand still and play eat animation
        Drinking, //Stand stil and drink
        AgentSpecific, //Predetor hunting, herbivore fleeing
    }
    public abstract class Agent : GameObject
    {
        private int maxHealth = 100;
        public int health;
        public int damage = 10;
        public int thirstMeter;
        public int hungermeter;
        public int searchRadPx = 150;
        public int speed = 30;
        private double changeDirectionTimer = 0;
        internal double eatingTimer = 0;
        private Vector2 direction;
        
        internal AgentState currentState;
        public GameObject target;
        public List<GameObject> targetObjectInRad = new List<GameObject>();

        private Random rnd = new Random();
        public Agent() {
            layerDepth = 0.2f;
            health = maxHealth;
            isCentered = true;
            currentState = AgentState.IdleWalk;
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
            HandleState();
        }

        private void HandleState()
        {
            switch (currentState)
            {
                case AgentState.IdleWalk:
                    IdleWalk();
                    break;
                case AgentState.Search:
                    AgentSpecificSearch();
                    WalkTowardsTarget();
                    break;
                case AgentState.Eating:
                    Eating();
                    break;
                case AgentState.Drinking:
                    Drinking();
                    break;
                case AgentState.AgentSpecific:
                    AgentSpecificAction();
                    break;
            }
        }


        internal virtual void Eating()
        {
            // Decrease the timer by the elapsed time
            eatingTimer -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds;

            // If the timer is less than or equal to zero, change state to IdleWalk
            if (eatingTimer <= 0 && target != null)
            {
                if (target is Plant plant)
                {
                    target = null;
                    plant.DeletePlant();
                }

                currentState = AgentState.Search;
            }
        }

        internal virtual void Drinking()
        {

        }
        
        internal virtual void AgentSpecificSearch() { }

        internal virtual void AgentSpecificAction() { }
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
            direction = new Vector2((float)(rnd.NextDouble() - 0.5), (float)(rnd.NextDouble() - 0.5));
            direction.Normalize(); // Make sure the direction vector has a length of 1                   
            changeDirectionTimer = rnd.Next(10, 60);// Reset the timer with a random value between 1 and 5 seconds
        }

        private void WalkTowardsTarget()
        {
            if (targetObjectInRad.Count == 0) return;

            // Get the first target in the list
            target = targetObjectInRad[0];

            // Calculate the distance to the target
            float distanceToTarget = Vector2.Distance(position, target.position);

            // If the agent is close enough to the target, change its state and return
            if (distanceToTarget <= 10)
            {
                currentState = AgentState.Eating;
                eatingTimer = 1;
                return;
            }

            // Calculate the direction vector from the agent to the target
            direction = target.position - position;
            direction.Normalize();

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

                //Check i stedet for origin point (center af plant) 
                //Brug a* i stedet for.
                currentState = AgentState.Eating;
                eatingTimer = 1;
            }
        }


        internal void SearchForType(List<GameObject> targetObjects)
        {
            targetObjectInRad.Clear();
            foreach (GameObject obj in targetObjects)
            {
                if (!obj.isRemoved && Vector2.Distance(this.position, obj.position) <= this.searchRadPx)
                {
                    // Check if the agent is a Herbivore and the object is a Plant
                    if (this is Herbivore && obj is Plant)
                    {
                        targetObjectInRad.Add(obj);
                    }
                    // Check if the agent is a Predator and the object is a Herbivore
                    else if (this is Predator && obj is Herbivore)
                    {
                        targetObjectInRad.Add(obj);
                    }
                }
            }
            targetObjectInRad = targetObjectInRad.OrderBy(o => Vector2.Distance(this.position, o.position)).ToList();
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

            // Calculate the number of points to check on each edge of the bounding box
            int numPointsToCheck = Math.Max(texture.Width / 2, texture.Height / 2);

            // Check if any point on the bounding box is in a non-walkable tile
            for (int i = 0; i <= numPointsToCheck; i++)
            {
                float t = (float)i / numPointsToCheck;
                if (!IsPositionWalkable(Vector2.Lerp(topLeft, topRight, t)) ||
                    !IsPositionWalkable(Vector2.Lerp(topLeft, bottomLeft, t)) ||
                    !IsPositionWalkable(Vector2.Lerp(topRight, bottomRight, t)) ||
                    !IsPositionWalkable(Vector2.Lerp(bottomLeft, bottomRight, t)))
                {
                    return false;
                }
            }

            // If no non-walkable tile was found, return true
            return true;
        }


        private bool IsPositionWalkable(Vector2 pos)
        {
            return GridManager.IsWalkable(pos);
        }



    }
}
