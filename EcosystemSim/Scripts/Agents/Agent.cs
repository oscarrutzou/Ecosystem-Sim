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
        private int thirstHungerScale = 3;
        private int minAmountBeforeDmg = 30;
        private int amountBeforeSearch = 50;
        public int searchRadPx = 200;

        public int speed = 40;
        //private double changeDirectionTimer = 0;
        internal double eatingTimer = 0;
        private bool isEating;
        internal double drinkingTimer = 0;
        private Vector2 direction;
        
        internal AgentState currentState;
        //private GameObject newTarget = null;
        public GameObject target;
        public Stack<Tile> path;
        private Tile targetTile;
        public List<GameObject> targetObjectInRad = new List<GameObject>();

        public int amountDebug;
        public float distanceToTarget;
        public Tile pathEndTile;
        public bool canFindPath;
        public bool hasReachedTarget;

        private Random rnd = new Random();
        public bool isThirsty;
        public bool isHungry;
        private List<Vector2> cornerGridTilePos = new List<Vector2>();
        private int amountBeforeReached = 35;
        public string targetName;
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

            SetCornerGridPos();
            
            CheckThirstHunger();

            HandleState();
            targetName = "";
            targetName = target?.GetType().Name;

            ChangeDirectionOfSprite();
        }

        private void SetCornerGridPos()
        {
            cornerGridTilePos.Clear();
            cornerGridTilePos.Add(GridManager.GetCenterPosInGridLeftTop());
            cornerGridTilePos.Add(GridManager.GetCenterPosInGridRightTop());
            cornerGridTilePos.Add(GridManager.GetCenterPosInGridLeftBottom());
            cornerGridTilePos.Add(GridManager.GetCenterPosInGridRightBottom());

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

            if (thirstMeter > amountBeforeSearch) isThirsty = false;
            if (hungermeter > amountBeforeSearch) isHungry = false;

            if (thirstMeter <= amountBeforeSearch)
            {
                currentState = AgentState.Search;
                isThirsty = true;
                SearchForWater();

                if (hungermeter <= amountBeforeSearch && targetObjectInRad.Count == 0)
                {
                    isHungry = true;
                    SearchForFoodObjects();
                }

                if (targetObjectInRad.Count == 0)
                {
                    SearchForIdleWalkTiles();
                }
            }
            else if (hungermeter <= amountBeforeSearch) {
                currentState = AgentState.Search;
                isHungry = true;
                SearchForFoodObjects();

                if (thirstMeter <= amountBeforeSearch && targetObjectInRad.Count == 0)
                {
                    isThirsty = true;
                    SearchForWater();
                }

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
                if (thirstMeter <= 0) deathByThristCounter++;
                else if (hungermeter <= -50) deathByHungerCounter++;
                GameWorld.Instance.gameSpeed = 1;
                isRemoved = true; //Agent died
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
            List<Tile> walkableTiles = SceneData.tiles.Where(tile => tile.isWalkable).ToList();
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
                    if (targetObjectInRad.Count == 0 || targetObjectInRad.First() is Tile tile && tile.tileType != TileType.Water) //Also check tiletype, since its still in search state when thirsty.
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



            if (path == null || targetTile == null) return;

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
                if (hasReachedTarget) hasReachedTarget = false;
            }
            else
            {
                // If it's not, move towards the target tile
                position = nextPos;
            }


        }

        public void WalkTowardsTarget(Action onTargetReached)
        {
            GetTargetTile(false);

            //If the agent is close enough to the target, or not found the path
            if (hasReachedTarget)
            {
                onTargetReached?.Invoke();
                hasReachedTarget = false;
                //return;
            }

            if (path == null || targetTile == null) return;

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
            GameObject newTarget = null;
            GameObject tempCurTarget = target;
            if (target != null)
            {
                if (Vector2.Distance(position, target.position) <= amountBeforeReached)
                {
                    hasReachedTarget = true;
                    //path = null; // Reset the path
                    //target = null; // Reset the target
                    //return; // Return from the function
                }
            }

            if (targetObjectInRad.Count > 0)
            {
                if (randomTarget)
                {
                    if (rnd.Next(0, 2) == 0)
                    {
                        newTarget = PickRandomCorner();
                    }
                    else
                    {
                        newTarget = targetObjectInRad[rnd.Next(targetObjectInRad.Count)];
                    }
                }
                else
                {
                    newTarget = targetObjectInRad[0]; //Since we already have sorted the list for closest target.
                }
            }

            if (newTarget == null || IsNewTargetAndTargetSame(tempCurTarget, newTarget)) return; //If there isn't any in the targetObjectInRad

            

            // If the agent has not found a path yet, or if the target type has changed, or if the target is a Tile and its tileType has changed, find a new path
            if (path == null || path.Count == 0 )
            {
                target = newTarget;

                path = Astar.FindPath(position, target.position);

                if (path != null)
                {
                    pathEndTile = path.Last();
                    targetTile = path.Pop();
                }
            }

        }

        private bool IsNewTargetAndTargetSame(GameObject target, GameObject newTarget)
        {
            if (target == null) return false;
            if (target is Tile targetTile && newTarget is Tile newTargetTile)
            {
                if (isThirsty)
                {
                    if (targetTile.tileType != TileType.Water && newTargetTile.tileType == TileType.Water) return false;
                    else if (targetTile.tileType == TileType.Water && newTargetTile.tileType != TileType.Water) return true;
                }

                return targetTile.tileType == newTargetTile.tileType;
             }

            return target.GetType()  == newTarget.GetType();
        }

        private GameObject PickRandomCorner()
        {
            //Closest corner.
            Vector2 closestCorner = cornerGridTilePos.Aggregate((x, y) => Vector2.Distance(x, position) < Vector2.Distance(y, position) ? x : y);
            cornerGridTilePos.Remove(closestCorner);

            int randomPick = rnd.Next(cornerGridTilePos.Count);
            return GridManager.GetTileAtPos(cornerGridTilePos[randomPick]);
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
                    CheckThirstHunger();
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
                CheckThirstHunger();
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
