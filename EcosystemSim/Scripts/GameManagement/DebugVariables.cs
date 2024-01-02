using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EcosystemSim
{
    public static class DebugVariables
    {
        private static Vector2 pos;
        public static void DrawDebug(TestScene scene)
        {
            pos = new Vector2(10, 10);
            DrawString($"Mouse in world: {InputManager.mousePositionInWorld}");
            DrawString($"Mouse on UI: {InputManager.mousePositionOnScreen}");
            DrawString($"Mouse out of bounds: {InputManager.mouseOutOfBounds}");

            Tile tile = scene.grid.GetTileAtPos(InputManager.mousePositionInWorld);
            if (tile != null)
            {
                DrawString($"Hover tile type: {tile.type}");
                DrawString($"Hover tile grid pos: ({scene.grid.hoverGridPos[0]}, {scene.grid.hoverGridPos[1]})");
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
