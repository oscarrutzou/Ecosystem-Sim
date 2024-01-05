using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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

        public static bool mouseOutOfBounds;
        public static bool debugStats = true;

        public static Tile tileOnHover;
        public static TileType selectedTileType = TileType.TestTile;

        private static Dictionary<Keys, TileType> keyTileTypeMap = new Dictionary<Keys, TileType>
        {
            { Keys.D1, TileType.TestTile },
            { Keys.D2, TileType.TestTileNonWalk }
        };
        #endregion

        /// <summary>
        /// Gets called in GameWorld, at the start of the update
        /// </summary>
        public static void HandleInput()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

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

            MoveCam();
            ChangeSelectedTile();

            if (GameWorld.Instance.currentScene is TestScene scene && scene.bgGrid != null)
            {
                if (keyboardState.IsKeyDown(Keys.E) && !previousKeyboardState.IsKeyDown(Keys.E))
                {
                    SaveLoad.SaveGrid(scene.bgGrid);
                }

                if (keyboardState.IsKeyDown(Keys.R) && !previousKeyboardState.IsKeyDown(Keys.R))
                {
                    foreach (Tile tile1 in scene.bgGrid.tiles)
                    {
                        tile1.ChangeTile(TileType.Empty);
                    }
                }

                if (keyboardState.IsKeyDown(Keys.T) && !previousKeyboardState.IsKeyDown(Keys.T))
                {
                    scene.bgGrid = SaveLoad.LoadGrid();
                }
            }
        }

        private static void HandleMouseInput()
        {
            mouseClicked = (Mouse.GetState().LeftButton == ButtonState.Pressed) && (previousMouseState.LeftButton == ButtonState.Released);
            mouseRightClicked = (Mouse.GetState().RightButton == ButtonState.Pressed) && (previousMouseState.RightButton == ButtonState.Released);

            if (GameWorld.Instance.currentScene is TestScene scene && scene.bgGrid != null)
            {
                tileOnHover = scene.bgGrid.GetTile(mousePositionInWorld);
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
