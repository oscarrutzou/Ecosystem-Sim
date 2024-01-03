using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EcosystemSim
{
    public static class DebugVariables
    {
        private static Vector2 pos;
        public static void DrawDebug()
        {
            pos = new Vector2(10, 10);
            DrawString($"Amount of Herbivores: {SceneData.herbivores.Count}");
            DrawString($"Amount of Predators: {SceneData.predators.Count}");
            DrawString($"Mouse in world: {InputManager.mousePositionInWorld}");
            DrawString($"Mouse on UI: {InputManager.mousePositionOnScreen}");
            DrawString($"Mouse out of bounds: {InputManager.mouseOutOfBounds}");
            DrawString($"Grid: {GridManager.grids[0]?.startPosPx}");


            Tile tile = InputManager.tileOnHover;
            if (tile != null)
            {
                DrawString($"Hover tile type: {tile.type}");
                DrawString($"Hover tile grid pos: ({tile.gridPos[0]}, {tile.gridPos[1]})");
                DrawString($"Hover tile grid pos: {tile.position}");
            }

        }

        private static void DrawString(string text)
        {
            GameWorld.Instance.spriteBatch.DrawString(GlobalTextures.defaultFont, text, pos, Color.Black);
            Vector2 size = GlobalTextures.defaultFont.MeasureString(text);
            pos.Y += size.Y;
        }
    }
}
