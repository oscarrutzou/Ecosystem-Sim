using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Net.NetworkInformation;
using SharpDX.Direct2D1;

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
        public static int deathByThristCounter;
        public static int deathByHungerCounter;
        private int maxHealth = 100;
        public double health;
        public int damage = 10;
        public double thirstMeter;
        public int thirstMaxMeter = 100;
        public double hungermeter;
        public int hungerMaxmeter = 100;
        private int thirstHungerScale = 1;
        private int minAmountBeforeDmg = 30;
        internal int amountBeforeSearch = 50;
        public int searchRadPx = 200;

        public int speed = 60;
        //private double changeDirectionTimer = 0;
        internal double eatingTimer = 0;
        private bool isEating;
        internal double drinkingTimer = 0;
        private Vector2 direction;
        
        internal AgentState currentState;
        public GameObject target;
        public Stack<Tile> path;
        private Tile targetTile;
        public List<GameObject> targetObjectInRad = new List<GameObject>();

        public int amountDebug;
        public float distanceToTarget;
        public Tile pathEndTile;
        public bool canFindPath;
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

            ChangeDirectionOfSprite();
        }
        private void ChangeDirectionOfSprite()
        {
            if (direction.X < 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            if (direction.X >= 0)
            {
                spriteEffects = SpriteEffects.None;
            }
        }

        #region Search and set states
        private void CheckThirstHunger()
        {
            thirstMeter -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed * thirstHungerScale;
            hungermeter -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed * thirstHungerScale;

            if (currentState == AgentState.Eating || currentState == AgentState.Drinking) return;

            if (thirstMeter <= amountBeforeSearch)
            {
                currentState = AgentState.Search;
                SearchForWater();

                if (hungermeter <= amountBeforeSearch && targetObjectInRad.Count == 0)
                {
                    SearchForFoodObjects();
                }

                if (targetObjectInRad.Count == 0)
                {
                    SearchForIdleWalkTiles();
                }
            }
            else if (hungermeter <= amountBeforeSearch) {
                currentState = AgentState.Search;

                SearchForFoodObjects();

                if (targetObjectInRad.Count == 0)
                {
                    currentState = AgentState.Search;
                    SearchForIdleWalkTiles();
                }
            }
            else
            {
                currentState = AgentState.IdleWalk;
                SearchForIdleWalkTiles();
            }


            if (hungermeter <= minAmountBeforeDmg || thirstMeter <= minAmountBeforeDmg)
            {
                health -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed * thirstHungerScale;
            }
            else
            {
                if (health < maxHealth && health > 0)
                {
                    health += GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed * thirstHungerScale;
                }
            }

            if (health <= 0 || thirstMeter <= 0 || hungermeter <= -50)
            {
                isRemoved = true; //Agent died
                if (hungermeter <= -50)
                {
                    deathByHungerCounter++;
                    
                }
                else if (thirstMeter <= 0)
                {
                    deathByThristCounter++;
                }
            }
        }

        private void SearchForWater()
        {
            List<Tile> waterTiles = SceneData.tiles.Where(tile => tile.tileType == TileType.Water).ToList();
            SearchForType(waterTiles);
        }
        private void SearchForFoodObjects()
        {
            if (this is Herbivore)
            {
                List<GameObject> plantList = SceneData.plants.Cast<GameObject>().ToList();
                SearchForType(plantList);
            }
            else if (this is Predator)
            {

            }
        }
        private void SearchForIdleWalkTiles()
        {
            List<Tile> walkableTiles;

            // If the herbivore is not thirsty, exclude water tiles from the list of walkable tiles
            if (thirstMeter > amountBeforeSearch)
            {
                walkableTiles = SceneData.tiles.Where(tile => tile.isWalkable && tile.tileType != TileType.Water).ToList();
            }
            else
            {
                walkableTiles = SceneData.tiles.Where(tile => tile.isWalkable).ToList();
            }

            SearchForType(walkableTiles);
        }


        #endregion

        private void HandleState()
        {
            switch (currentState)
            {
                case AgentState.IdleWalk:
                    WalkTowardsTargetIdle();
                    break;
                case AgentState.Search:
                    if (targetObjectInRad.Count == 0 || targetObjectInRad.First() is Tile tile && tile.tileType != TileType.Water)
                    {
                        WalkTowardsTargetIdle();
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


        public void WalkTowardsTargetIdle()
        {
            
            if (targetTile == null)
            {
                // If there's no target tile, change direction
                GetTargetTile(true);
            }

            if (targetTile == null) return;

            // Calculate the direction to the target tile
            direction = targetTile.Center - position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            Vector2 nextPos = position + direction * speed * (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

            // Check if the next position is close to the target tile
            if (Vector2.Distance(nextPos, targetTile.Center) < speed * (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed)
            {
                // If it is, move to the target tile and get the next target tile from the path
                position = targetTile.Center;
                targetTile = (path.Count > 0) ? path.Pop() : null;
            }
            else
            {
                // If it's not, move towards the target tile
                position = nextPos;
            }

        }

        public void WalkTowardsTarget(Action onTargetReached)
        {
            if (targetTile == null)
            {
                // If there's no target tile, change direction
                GetTargetTile(false);
            }
            
            //If the agent is close enough to the target, or not found the path
            if (targetTile == null)
            {
                float distance = Vector2.Distance(position, target.position);
                if (distance <= 35) //If it cant find the path, since its already here
                {
                    onTargetReached?.Invoke();
                    pathEndTile = null;
                }
                else //Not found the path, walk idle
                {
                    SearchForIdleWalkTiles();
                    WalkTowardsTargetIdle();
                }
                return;
            }

            // Calculate the direction to the target tile
            direction = targetTile.Center - position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            Vector2 nextPos = position + direction * speed * (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

            // Check if the next position is close to the target tile
            if (Vector2.Distance(nextPos, targetTile.Center) < speed * (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed)
            {
                // If it is, move to the target tile and get the next target tile from the path
                position = targetTile.Center;
                targetTile = (path.Count > 0) ? path.Pop() : null;
                
            }
            else
            {
                // If it's not, move towards the target tile
                position = nextPos;
            }
        }
 
        private void GetTargetTile(bool randomTarget)
        {
           
            if (targetObjectInRad.Count > 0)
            {
                if (randomTarget)
                {
                    // Select a random tile from targetObjectInRad
                    targetObjectInRad = targetObjectInRad.OrderBy(x => rnd.Next()).ToList();
                    target = targetObjectInRad[0];
                }
                else
                {
                    target = targetObjectInRad[0]; //Since we already have sorted the list for closest target.
                }
                
            }

            if (target == null) return; //If there isnt any in the targetObjectInRad

            if (path == null || path.Count == 0)
            {
                path = Astar.FindPath(position, target.position);

                if (GridManager.grids[0].GetTile(position).gridPos == GridManager.grids[0].GetTile(target.position).gridPos) //Since path is null if they are at the same tile, so 
                {
                    targetTile = null;
                }
            }

            if (path != null && path.Count > 0)
            {
                pathEndTile = path.Last();
                targetTile = path.Pop();
            }
        }

        internal void SearchForType(List<Tile> targetTiles)
        {
            targetObjectInRad.Clear();
            foreach (Tile tile in targetTiles)
            {
                if (!tile.isRemoved && Vector2.Distance(this.position, tile.position) <= this.searchRadPx)
                {
                    targetObjectInRad.Add(tile);
                }
            }
            targetObjectInRad = targetObjectInRad.OrderBy(o => Vector2.Distance(this.position, o.position)).ToList();
        }

        internal void SearchForType(List<GameObject> targetObjects)
        {
            targetObjectInRad.Clear();
            foreach (GameObject obj in targetObjects)
            {
                if (!obj.isRemoved && Vector2.Distance(this.position, obj.position) <= this.searchRadPx)
                {
                    targetObjectInRad.Add(obj);
                }
            }
            targetObjectInRad = targetObjectInRad.OrderBy(o => Vector2.Distance(this.position, o.position)).ToList();
        }

        public abstract void ActionOnTargetFound();

        internal virtual void Eating()
        {
            // Decrease the timer by the elapsed time
            eatingTimer -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

            if (target == null || target is Plant plant && plant.isBeingEaten && !isEating)
            {
                currentState = AgentState.Search;
            }
            // If the timer is less than or equal to zero, change state to IdleWalk
            else
            {
                if (target is Plant plant1)
                {
                    plant1.isBeingEaten = true;
                    isEating = true;
                }

                if (eatingTimer <= 0)
                {
                    if (target is Plant plantObj)
                    {
                        target = null;
                        plantObj.DeletePlant();
                    }

                    if (target is Herbivore herbivore)
                    {
                        herbivore.isRemoved = true;
                    }
                    isEating = false;
                    hungermeter = hungerMaxmeter;
                    currentState = AgentState.IdleWalk;
                }
            }
        }

        internal virtual void Drinking()
        {
            // Decrease the timer by the elapsed time
            drinkingTimer -= GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

            // If the timer is less than or equal to zero, change state to IdleWalk
            if (drinkingTimer <= 0)
            {
                thirstMeter = thirstMaxMeter;
                currentState = AgentState.IdleWalk;
            }
        }

        internal virtual void AgentSpecificAction() { }


        private void DrawSearchRad()
        {
            Vector2 pos = new Vector2(position.X - (GlobalTextures.textures[TextureNames.UISearchRad100].Width / 2), position.Y - (GlobalTextures.textures[TextureNames.UISearchRad100].Height / 2));
            GameWorld.Instance.spriteBatch.Draw(GlobalTextures.textures[TextureNames.UISearchRad100], pos, null, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }

        public override void Draw()
        {
            base.Draw();
            //DrawSearchRad();
            //if (InputManager.debugStats) DrawDebugCollisionBox(Color.AliceBlue);
        }



    }
}
