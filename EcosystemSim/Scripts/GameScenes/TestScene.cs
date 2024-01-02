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
            Vector2 startPos = new Vector2(0 - grid.gridSizeDem * (grid.gridSize[0] / 2), 0 - grid.gridSizeDem * (grid.gridSize[1] / 2));
            grid.InitGrid(startPos);
        }
        public override void DrawOnScreen()
        {
            base.DrawOnScreen();

            if (InputManager.debugStats) DebugVariables.DrawDebug(this);

        }
    }
}
