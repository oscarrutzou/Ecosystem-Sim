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
        internal bool isThirsty;
        internal bool isHungry;
        internal double drinkingTimer = 0;
        private Vector2 direction;
        
        internal AgentState currentState;
        public GameObject target;
        public Stack<Tile> path;
        public Stack<Tile> debugFullPath;
        public Tile nextTargetTile;
        public List<GameObject> targetObjectInRad = new List<GameObject>();

        public int amountDebug;
        public float distanceToTarget;
        public Tile pathEndTile;
        public bool canFindPath;
        private Random rnd = new Random();

        public Astar astar;

        public Agent() {
            layerDepth = 0.2f;
            health = maxHealth;
            isCentered = true;
            thirstMeter = thirstMaxMeter;
            hungermeter = hungerMaxmeter;
            currentState = AgentState.IdleWalk;
            astar = new Astar();
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


            isThirsty = thirstMeter <= amountBeforeSearch;
            isHungry = hungermeter <= amountBeforeSearch;


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
            List<GameObject> plantList = SceneData.plants.Cast<GameObject>().ToList();
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
            GetTargetTile(true);

            if (nextTargetTile == null) return;

            AfterGettingPathWalk();
        }

        public void WalkTowardsTarget(Action onTargetReached)
        {
            GetTargetTile(false);
            
            //If the agent is close enough to the target, or not found the path
            if (nextTargetTile == null)
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

            AfterGettingPathWalk();
        }

        private void AfterGettingPathWalk()
        {
            // Calculate the direction to the target tile
            direction = nextTargetTile.Center - position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            Vector2 nextPos = position + direction * speed * (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;

            //if (path == null) 

            // Check if the next position is close to the target tile
            if (Vector2.Distance(nextPos, nextTargetTile.Center) < speed * (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed)
            {
                // If it is, move to the target tile and get the next target tile from the path
                position = nextTargetTile.Center;
                nextTargetTile = (path.Count > 0) ? path.Pop() : null;
            }
            else
            {
                // If it's not, move towards the target tile
                position = nextPos;
            }
        }

 
        private void GetTargetTile(bool randomTarget)
        {
            GameObject tempTarget = null;
            if (targetObjectInRad.Count > 0)
            {
                if (randomTarget)
                {
                    targetObjectInRad = targetObjectInRad.OrderBy(x => rnd.Next()).ToList();
                    tempTarget = targetObjectInRad[0];

                    //foreach (GameObject gm in SceneData.tiles)
                    //{
                    //    gm.color = Color.White;
                    //}

                    //foreach (GameObject targetObject in targetObjectInRad)
                    //{
                    //    targetObject.color = Color.Green;
                    //}
                }
                else
                {
                    tempTarget = targetObjectInRad[0]; //Since we already have sorted the list for closest target.
                }
                
            }

            if (tempTarget == null) return;
            if (IsNewTargetSamePath(tempTarget)) return;

            path = null;
            target = tempTarget;

            path = astar.FindPath(position, target.position);
            debugFullPath = path;
            if (astar.startNTargetPosSame || path == null) //Since path is null if they are at the same tile, so 
            {
                nextTargetTile = null;
            }

            if (path != null && path.Count > 0)
            {
                pathEndTile = path.Last();
                nextTargetTile = path.Pop();
            }
        }


        /// <summary>
        /// False == new path
        /// </summary>
        /// <param name="target"></param>
        /// <param name="newTarget"></param>
        /// <returns></returns>
        private bool IsNewTargetSamePath(GameObject newTarget)
        {
            if (target == null) return false;
            if (nextTargetTile == null) return false; //Meaning it has walked to the target

            if (nextTargetTile.gridPos == pathEndTile.gridPos) return true; //Meaning it dosent have to generate a new path

            //Tile gridPosNewTarget = GridManager.GetTileAtPos(newTarget.position);

            List<Tile> newTargetTiles = GridManager.GetTilesAtPos(newTarget.position);
            newTargetTiles.Where(t => t != null && t.isWalkable);


            foreach (Tile tile in newTargetTiles)
            {
                if (isThirsty && Tile.IsTileTypeWater(tile.tileType))
                {
                    return Tile.IsTileTypeWater(pathEndTile.tileType);
                }
                else if (isHungry)
                {
                    //Check plants for herbivore
                }

            }

            foreach (Tile tile in newTargetTiles)
            {
                if (Tile.IsTileTypeGrowableGrass(tile.tileType))
                {
                    return Tile.IsTileTypeGrowableGrass(pathEndTile.tileType);
                }
            }

            return Tile.IsTileTypeGrowableGrass(pathEndTile.tileType);
        }

        internal void SearchForType(List<Tile> targetTiles)
        {
            targetObjectInRad.Clear();

            foreach (Tile tile in targetTiles)
            {
                if (!tile.isRemoved && Vector2.Distance(this.position, tile.centerPos) <= this.searchRadPx)
                {
                    targetObjectInRad.Add(tile);
                }
            }
            
            targetObjectInRad = targetObjectInRad.OrderBy(o => Vector2.Distance(this.position, o.centerPos)).ToList();
        }

        internal void SearchForType(List<GameObject> targetObjects)
        {
            targetObjectInRad.Clear();
            foreach (GameObject obj in targetObjects)
            {
                if (!obj.isRemoved && Vector2.Distance(this.position, obj.centerPos) <= this.searchRadPx)
                {
                    targetObjectInRad.Add(obj);
                }
            }
            targetObjectInRad = targetObjectInRad.OrderBy(o => Vector2.Distance(this.position, o.centerPos)).ToList();
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

            //Texture2D pixel = new Texture2D(GameWorld.Instance.gfxDevice, 1, 1);
            //pixel.SetData(new[] { Color.White });

            //if (pathEndTile != null && debugFullPath != null) {
            //    Vector2 pos = position;
            //    foreach (Tile tile in debugFullPath)
            //    {
            //        DrawLine(pixel, pos, tile.Center, Color.Red);
            //        pos = tile.Center;
            //    }
            //}


            //DrawLine(pixel, position, new Vector2(position.X, position.Y + searchRadPx), Color.Blue);
            //DrawLine(pixel, position, new Vector2(position.X, position.Y - searchRadPx), Color.Blue);
            //DrawLine(pixel, position, new Vector2(position.X + searchRadPx, position.Y), Color.Blue);
            //DrawLine(pixel, position, new Vector2(position.X - searchRadPx, position.Y), Color.Blue);


        }



    }
}
