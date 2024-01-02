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
            grid.InitGrid(new Vector2(0,0));
        }
    }
}
