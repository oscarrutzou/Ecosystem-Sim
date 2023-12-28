using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

            // Check if the player presses the escape key
            if (keyboardState.IsKeyDown(Keys.Escape) && !previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                GameWorld.Instance.Exit();
                //Global.currentScene.isPaused = !Global.currentScene.isPaused;

                //if (Global.currentScene.isPaused)
                //{
                //    Global.world.pauseScreen.ShowPauseMenu();
                //}
                //else
                //{
                //    Global.world.pauseScreen.HidePauseMenu();
                //}
            }

            //if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            //{
            //    CheckButtons();
            //}

            //PlayerInput();

            mouseClicked = (Mouse.GetState().LeftButton == ButtonState.Pressed) && (previousMouseState.LeftButton == ButtonState.Released);
            mouseRightClicked = (Mouse.GetState().RightButton == ButtonState.Pressed) && (previousMouseState.RightButton == ButtonState.Released);

            previousMouseState = mouseState;
            previousKeyboardState = keyboardState;
        }

        //public static void PlayerInput()
        //{
        //    if (Global.currentScene.isPaused) return;

        //    if (Global.player != null)
        //    {
        //        Vector2 dir = mousePositionInWorld - Global.player.position;
        //        dir.Normalize();

        //        // Calculate the offset vector perpendicular to the direction vector
        //        Vector2 offset = new Vector2(-dir.Y, dir.X) * -Global.player.textureOffset; // 50 is the offset distance in px
        //        Vector2 tempPosition = Global.player.position; // Store the current position

        //        Global.player.RotateTowardsTargetWithOffset(mousePositionInWorld, offset);

        //        if (keyboardState.IsKeyDown(Keys.A))
        //        {
        //            Global.player.position.X -= Global.player.playerSpeed;
        //        }
        //        if (keyboardState.IsKeyDown(Keys.D))
        //        {
        //            Global.player.position.X += Global.player.playerSpeed;
        //        }
        //        if (keyboardState.IsKeyDown(Keys.W))
        //        {
        //            Global.player.position.Y -= Global.player.playerSpeed;
        //        }
        //        if (keyboardState.IsKeyDown(Keys.S))
        //        {
        //            Global.player.position.Y += Global.player.playerSpeed;
        //        }

        //        // Toggle no clip
        //        if (keyboardState.IsKeyDown(Keys.N) && previousKeyboardState.IsKeyDown(Keys.N))
        //        {
        //            noClip = !noClip;
        //        }

        //        CheckPlayerMoveColRoom(tempPosition);

        //        anyMoveKeyPressed = keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.D);
        //    }
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
            return Vector2.Transform(pos, invMatrix);
        }

    }
}
