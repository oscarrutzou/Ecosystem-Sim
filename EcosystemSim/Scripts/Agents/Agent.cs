using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Net.NetworkInformation;

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
        public double thirstMeter;
        public int thirstMaxMeter = 100;
        public double hungermeter;
        public int hungerMaxmeter = 100;
        private int thirstHungerScale = 3;
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
            thirstMeter = thirstMaxMeter;
            hungermeter = hungerMaxmeter;
            currentState = AgentState.IdleWalk;
        }

        public override void Update()
        {
            base.Update();

            CheckThirstHunger();

            HandleState();
        }
        private void CheckThirstHunger()
        {
            thirstMeter -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed * thirstHungerScale;
            hungermeter -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed * thirstHungerScale;

            if (currentState == AgentState.Eating || currentState == AgentState.Drinking) return;
            if (hungermeter <= 90) {
                currentState = AgentState.Search;
            }
            else
            {
                currentState = AgentState.IdleWalk;
            }
        }

        private void HandleState()
        {
            SearchForTarget();

            switch (currentState)
            {
                case AgentState.IdleWalk:
                    IdleWalk();
                    break;
                case AgentState.Search:
                    //AgentSpecificSearch();
                    if (targetObjectInRad.Count == 0)
                    {
                        IdleWalk();
                    }
                    else
                    {
                        WalkTowardsTarget(ActionOnTargetFound);
                    }
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

        public abstract void ActionOnTargetFound();

        internal virtual void Eating()
        {
            // Decrease the timer by the elapsed time
            eatingTimer -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

            if (target == null || (target is Plant plant && plant.isEaten))
            {
                currentState = AgentState.Search;
            }
            // If the timer is less than or equal to zero, change state to IdleWalk
            else if (eatingTimer <= 0)
            {
                if (target is Plant plantObj)
                {
                    target = null;
                    plantObj.isEaten = true;
                    plantObj.DeletePlant();
                }
                hungermeter = hungerMaxmeter;
                currentState = AgentState.IdleWalk;
            }
        }

        internal virtual void Drinking()
        {

        }
        
        //internal virtual void AgentSpecificSearch() { }

        internal virtual void AgentSpecificAction() { }
        
        private void SearchForTarget()
        {
            if (this is Herbivore)
            {
                List<GameObject> list = SceneData.plants.Cast<GameObject>().ToList();
                SearchForType(list);
            } else if (this is Predator)
            {

            }
        }
        public void IdleWalk()
        {
            // Decrease the timer by the elapsed time
            changeDirectionTimer -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

            // If the timer is less than or equal to zero, change direction
            if (changeDirectionTimer <= 0)
            {
                ChangeDirection();
            }

            Vector2 tempPos = position;
            Vector2 nextPos = position + direction * speed * (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

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

        private void WalkTowardsTarget(Action actionOnDone)
        {
            if (targetObjectInRad.Count == 0) return;

            // Get the first target in the list
            target = targetObjectInRad[0];

            // Calculate the distance to the target
            float distanceToTarget = Vector2.Distance(position, target.position);

            // If the agent is close enough to the target, change its state and return
            if (distanceToTarget <= 10)
            {
                actionOnDone?.Invoke();
                return;
            }

            // Calculate the direction vector from the agent to the target
            direction = target.position - position;
            direction.Normalize();

            Vector2 tempPos = position;
            Vector2 nextPos = position + direction * speed * (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

            // Check if the next position is walkable
            if (CheckTileIsWalkable(nextPos))
            {
                position = nextPos;
            }
            else
            {
                position = tempPos;

                actionOnDone?.Invoke();
            }
        }


        internal void SearchForType(List<GameObject> targetObjects)
        {
            targetObjectInRad.Clear();
            foreach (GameObject obj in targetObjects)
            {
                if (!obj.isRemoved && Vector2.Distance(this.position, obj.position) <= this.searchRadPx)
                {
                    if (obj is Herbivore || obj is Predator || obj is Plant)
                        targetObjectInRad.Add(obj);
                    else
                        throw new Exception("The targetobjects must be a Herbivore, predator or plant");
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

        public override void Draw()
        {
            base.Draw();
            DrawSearchRad();
            DrawDebugCollisionBox(Color.AliceBlue);
        }



    }
}
