using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EcosystemSim
{
    public class TestScene : Scene
    {

        public Grid grid;


        public override void Initialize()
        {
            grid = new Grid();
            //Vector2 startPos = Vector2.Zero;
            grid.InitGrid(Vector2.Zero, true);
        }
        public override void DrawOnScreen()
        {
            base.DrawOnScreen();

            if (InputManager.debugStats) DebugVariables.DrawDebug(this);

        }
    }
}
