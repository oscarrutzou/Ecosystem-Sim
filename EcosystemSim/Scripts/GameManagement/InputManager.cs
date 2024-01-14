using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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

        public static GameObject objOnHover;
        public static Tile tileOnHover;
        public static Grid selectedGrid;
        public static TileType selectedTileType = TileType.Grass;


        public static Vector2 startPos;
        public static Vector2 endPos;
        public static Stack<Tile> path = new Stack<Tile>();

        private static Dictionary<Keys, TileType> keyTileTypeMap = new Dictionary<Keys, TileType>
        {
            { Keys.D1, TileType.Grass },
            { Keys.D2, TileType.TestTileNonWalk },
            { Keys.D3, TileType.Water },
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

            if (keyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                if (GameWorld.Instance.currentScene is TestScene scene)
                {
                    path = scene.astar.FindPath(startPos, endPos);
                }
            }

            MoveCam();

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
                }

                if (keyboardState.IsKeyDown(Keys.T) && !previousKeyboardState.IsKeyDown(Keys.T))
                {
                    foreach (Grid grid in GridManager.grids)
                    {
                        foreach (Tile tile in grid.tiles)
                        {
                            tile.ChangeTile(TileType.Empty);
                        }
                    }
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
                //foreach (GameObject obj in SceneData.gameObjects)
                //{
                //    if (IsMouseOver(obj))
                //    {
                //        objOnHover = obj;
                //        return;
                //    }
                //}
                tileOnHover = GridManager.GetTileAtPos(mousePositionInWorld);
                if (tileOnHover != null)
                {

                    if (mouseClicked)
                    {
                        startPos = tileOnHover.position;
                        //tileOnHover.color = Color.Pink;
                    }
                    if (mouseRightClicked)
                    {
                        endPos = tileOnHover.position;

                        //tileOnHover.color = Color.Green;

                    }
                }
                //There isnt any object where the mouse is, therefore set objOnHover to null.
                objOnHover = null;
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
