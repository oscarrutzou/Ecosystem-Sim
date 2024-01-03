using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EcosystemSim
{
    public class TestScene : Scene
    {

        public Grid bgGrid;


        public override void Initialize()
        {
            bgGrid = new Grid();
            GridManager.grids.Add(bgGrid);
            bgGrid.InitGrid(Vector2.Zero, false);
            SceneData.gameObjectsToAdd.Add(new Predator(Vector2.Zero));

        }
        public override void DrawOnScreen()
        {
            base.DrawOnScreen();

            if (InputManager.debugStats) DebugVariables.DrawDebug();

        }

        public override void Update()
        {
            base.Update();
            if (SceneData.gameObjects.Count > 0)
            {

            }
            if (SceneData.tiles.Count > 0)
            {

            }
        }
    }
}
