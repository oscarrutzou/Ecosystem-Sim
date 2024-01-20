using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EcosystemSim
{
    public static class InputManager
    {
        #region Variables
        public static KeyboardState keyboardState;
        public static KeyboardState previousKeyboardState;
        public static MouseState mouseState;
        // Prevents multiple click when clicking a button
        public static MouseState previousMouseState;

        public static Vector2 mousePositionInWorld;
        public static Vector2 mousePositionOnScreen;
        public static bool mouseClicked;
        public static bool mouseRightClicked;

        public static bool buildMode;
        public static bool mouseOutOfBounds;
        public static bool debugStats = true;
        public static bool debugWalkableTiles = true;

        public static GameObject objOnHover;
        public static Tile tileOnHover;
        public static Grid selectedGrid;
        public static TileType selectedTileType = TileType.Grass;
        public static HerbivoreType selectedHervicoreType = HerbivoreType.Rabbit;
        private static int gameSpeedIndex = 1;
        private static Dictionary<Keys, TileType> keyTileTypeMap = new Dictionary<Keys, TileType>
        {
            { Keys.D1, TileType.Grass },
            { Keys.D2, TileType.TestTileNonWalk },
            { Keys.D3, TileType.Water },
            { Keys.D4, TileType.TestTileNonFullNonWalk },
        };

        private static Dictionary<Keys, HerbivoreType> keyHerbivoreTypeMap = new Dictionary<Keys, HerbivoreType>
        {
            { Keys.D1, HerbivoreType.Rabbit },
        };

        private static List<float> gameSpeed = new List<float>()
        {
            { 0.1f},
            { 1f},
            { 2f},
            { 5f},
            { 10f},
            { 50f},
        };
        #endregion

        /// <summary>
        /// Gets called in GameWorld, at the start of the update
        /// </summary>
        public static void HandleInput()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            selectedGrid = GridManager.selectedGrid;

            //Sets the mouse position
            mousePositionOnScreen = GetMousePositionOnUI();
            mousePositionInWorld = GetMousePositionInWorld();

            HandleKeyboardInput();
            HandleMouseInput();

            previousMouseState = mouseState;
            previousKeyboardState = keyboardState;
        }

        private static void HandleKeyboardInput()
        {
            // Check if the player presses the escape key
            if (keyboardState.IsKeyDown(Keys.Escape) && !previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                GameWorld.Instance.Exit();
            }

            if (keyboardState.IsKeyDown(Keys.Q) && !previousKeyboardState.IsKeyDown(Keys.Q))
            {
                debugStats = !debugStats;
            }

            if (keyboardState.IsKeyDown(Keys.Tab) && !previousKeyboardState.IsKeyDown(Keys.Tab))
            {
                buildMode = !buildMode;
            }
            if (keyboardState.IsKeyDown(Keys.F) && !previousKeyboardState.IsKeyDown(Keys.F))
            {
                debugWalkableTiles = !debugWalkableTiles;
            }

            if (debugWalkableTiles)
            {
                foreach (Tile tile in SceneData.tiles)
                {
                    //Get all tiles at that pos
                    //find if one is not walkable
                    //Color all tiles in that temp to that color
                    if (GridManager.grids.All(g =>
                    {
                        Tile temp = g.GetTile(tile.gridPos[0], tile.gridPos[1]);
                        return temp != null && temp.isWalkable || temp != null && temp.tileType == TileType.Empty;
                    }))
                    {
                        if (tile.color != DebugVariables.selectedGridColor)
                        {
                            tile.color = Color.White;
                        }
                    }
                    else
                    {
                        if (tile.color != DebugVariables.selectedGridColor)
                        {
                            tile.color = DebugVariables.debugNonWalkableTilesColor;
                        }
                    }
                }
            }

            MoveCam();

            if (keyboardState.IsKeyDown(Keys.Left) && !previousKeyboardState.IsKeyDown(Keys.Left))
            {
                if (gameSpeedIndex >= 1)
                {
                    gameSpeedIndex -= 1;
                    GameWorld.Instance.gameSpeed = gameSpeed[gameSpeedIndex];
                }
            }

            if (keyboardState.IsKeyDown(Keys.Right) && !previousKeyboardState.IsKeyDown(Keys.Right))
            {
                if (gameSpeedIndex < gameSpeed.Count - 1)
                {
                    gameSpeedIndex += 1;
                    GameWorld.Instance.gameSpeed = gameSpeed[gameSpeedIndex];
                }
            }

            if (keyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                for (int i = 0; i < 50; i++)
                {
                    SceneData.gameObjectsToAdd.Add(new Herbivore(Vector2.Zero, HerbivoreType.Rabbit));
                }
            }

            if (!buildMode) return;

            ChangeSelectedTile();

            if (keyboardState.IsKeyDown(Keys.I) && !previousKeyboardState.IsKeyDown(Keys.I))
            {
                GridManager.grids.Add(new Grid(TileType.Empty, "NewGrid"));
            }

            if (GridManager.grids.Count != 0)
            {
                if (keyboardState.IsKeyDown(Keys.E) && !previousKeyboardState.IsKeyDown(Keys.E))
                {
                    for (int i = 0; i < GridManager.grids.Count; i++)
                    {
                        SaveLoad.SaveGrid(GridManager.grids[i], i, GridManager.grids[i].gridName);
                    }
                }

                if (keyboardState.IsKeyDown(Keys.R) && !previousKeyboardState.IsKeyDown(Keys.R) && keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    foreach (Grid grid in GridManager.grids)
                    {
                        foreach(Tile tile in grid.tiles)
                        {
                            tile.ChangeTile(TileType.Empty);
                        }
                    }
                    foreach (Agent agent in SceneData.herbivores)
                    {
                        agent.isRemoved = true;
                    }
                    foreach (Agent agent in SceneData.predators)
                    {
                        agent.isRemoved = true;
                    }
                    DebugVariables.herbivoresAlive = 0;
                }

                if (keyboardState.IsKeyDown(Keys.T) && !previousKeyboardState.IsKeyDown(Keys.T))
                {
                    SaveLoad.LoadGrids();
                }

                if (keyboardState.IsKeyDown(Keys.Z) && !previousKeyboardState.IsKeyDown(Keys.Z))
                {
                    if (GridManager.GridIndex > 0) GridManager.GridIndex--;
                }

                if (keyboardState.IsKeyDown(Keys.X) && !previousKeyboardState.IsKeyDown(Keys.X))
                {
                    if (GridManager.GridIndex < GridManager.grids.Count - 1) GridManager.GridIndex++;
                }

            }
        }

        private static void HandleMouseInput()
        {
            mouseClicked = (Mouse.GetState().LeftButton == ButtonState.Pressed) && (previousMouseState.LeftButton == ButtonState.Released);
            mouseRightClicked = (Mouse.GetState().RightButton == ButtonState.Pressed) && (previousMouseState.RightButton == ButtonState.Released);

            if (!buildMode)
            {
                //objOnHover = null;

                foreach (GameObject obj in SceneData.herbivores)
                {
                    if (IsMouseOver(obj))
                    {
                        objOnHover = obj;
                        break;
                    }
                }

                if (mouseRightClicked && objOnHover != null && objOnHover is Agent agent)
                {
                    agent.thirstMeter = 40;
                    agent.hungermeter = 40;
                }

                tileOnHover = GridManager.GetTileAtPos(mousePositionInWorld);
                if (tileOnHover != null)
                {

                    if (mouseClicked)
                    {
                        //startPos = tileOnHover.position;
                        Herbivore herbivore = new Herbivore(tileOnHover.Center, selectedHervicoreType);
                        SceneData.gameObjectsToAdd.Add(herbivore);
                    }
                }
            }
            else
            {
                if (GridManager.grids.Count != 0)
                {
                    tileOnHover = GridManager.GetTileAtPos(mousePositionInWorld);

                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && tileOnHover != null)
                    {
                        tileOnHover.ChangeTile(selectedTileType);
                    }
                    if (Mouse.GetState().RightButton == ButtonState.Pressed && tileOnHover != null)
                    {
                        tileOnHover.ChangeTile(TileType.Empty);
                    }
                }
            }


            
        }
        private static bool IsMouseOver(GameObject gameObject)
        {
            if (gameObject is Tile)
            {
                return false;
            }
            else
            {
                return gameObject.collisionBox.Contains(InputManager.mousePositionInWorld.ToPoint());
            }
        }

        private static void ChangeSelectedTile()
        {
            foreach (var mapping in keyTileTypeMap)
            {
                if (keyboardState.IsKeyDown(mapping.Key) && !previousKeyboardState.IsKeyDown(mapping.Key))
                {
                    selectedTileType = mapping.Value;
                    break;
                }
            }
        }

        private static Vector2 GetMousePositionInWorld()
        {
            Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
            Matrix invMatrix = Matrix.Invert(GameWorld.Instance.worldCam.GetMatrix());
            return Vector2.Transform(pos, invMatrix);
        }

        private static Vector2 GetMousePositionOnUI()
        {
            Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
            Matrix invMatrix = Matrix.Invert(GameWorld.Instance.uiCam.GetMatrix());
            Vector2 returnValue = Vector2.Transform(pos, invMatrix);
            mouseOutOfBounds = (returnValue.X < 0 || returnValue.Y < 0 || returnValue.X > GameWorld.Instance.gfxManager.PreferredBackBufferWidth || returnValue.Y > GameWorld.Instance.gfxManager.PreferredBackBufferHeight);
            return returnValue;
        }


        private static void MoveCam()
        {
            // Handle camera movement based on keyboard input //-- look at
            Vector2 moveDirection = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.W))
                moveDirection.Y = -1;
            if (keyboardState.IsKeyDown(Keys.S))
                moveDirection.Y = 1;
            if (keyboardState.IsKeyDown(Keys.A))
                moveDirection.X = -1;
            if (keyboardState.IsKeyDown(Keys.D))
                moveDirection.X = 1;

            GameWorld.Instance.worldCam.Move(moveDirection * 5); // Control camera speed //-- look at
        }

    }
}
