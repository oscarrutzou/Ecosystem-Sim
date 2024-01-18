using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EcosystemSim
{
    public class TestScene : Scene
    {
        //public Astar astar;
        public override void Initialize()
        {
            GridManager.InitStartGrids();

            SaveLoad.LoadGrids();
            for (int i = 0; i < 1; i++)
            {
                SceneData.gameObjectsToAdd.Add(new Herbivore(Vector2.Zero, HerbivoreType.Rabbit));
            }
            //SceneData.gameObjectsToAdd.Add(new Predator(Vector2.Zero));
            //astar = new Astar(GridManager.grids);
        }
        public override void DrawOnScreen()
        {
            base.DrawOnScreen();

            if (InputManager.debugStats) DebugVariables.DrawDebug();

        }

        public override void Update()
        {
            base.Update();
            GridManager.Update();
            //Astar.UpdateDebugColor();
        }
    }
}
