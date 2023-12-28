using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace EcosystemSim
{

    public class GameWorld : Game
    {
        #region Variables
        public Dictionary<Scenes, Scene> scenes { get; private set; }
        public Scene currentScene;
        public Camera worldCam { get; private set; }
        public Camera uiCam { get; private set; } //Static on the ui
        public GameTime gameTime { get; private set; }

        public SpriteBatch spriteBatch;
        public GraphicsDeviceManager gfxManager;
        public GraphicsDevice gfxDevice => GraphicsDevice;


        public static GameWorld Instance;
        #endregion

        public GameWorld()
        {
            if (Instance == null) Instance = this;

            gfxManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "Ecosystem Simulation";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ResolutionSize(1280, 720);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(gfxDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            gfxDevice.Clear(Color.Beige);

            base.Draw(gameTime);
        }


        public void ResolutionSize(int width, int height)
        {
            gfxManager.HardwareModeSwitch = true;
            gfxManager.PreferredBackBufferWidth = width;
            gfxManager.PreferredBackBufferHeight = height;
            gfxManager.IsFullScreen = false;
            gfxManager.ApplyChanges();
        }

        public void Fullscreen()
        {
            gfxManager.HardwareModeSwitch = false;
            gfxManager.PreferredBackBufferWidth = gfxDevice.DisplayMode.Width;
            gfxManager.PreferredBackBufferHeight = gfxDevice.DisplayMode.Height;
            gfxManager.IsFullScreen = true;
            gfxManager.ApplyChanges();
        }

    }
}