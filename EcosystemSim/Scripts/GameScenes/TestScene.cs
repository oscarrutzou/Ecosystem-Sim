using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EcosystemSim
{
    public class TestScene : Scene
    {

        public override void Initialize()
        {
            GridManager.InitStartGrids();
            SceneData.gameObjectsToAdd.Add(new Predator(Vector2.Zero));

        }
        public override void DrawOnScreen()
        {
            base.DrawOnScreen();

            if (InputManager.debugStats) DebugVariables.DrawDebug();

        }

    }
}
