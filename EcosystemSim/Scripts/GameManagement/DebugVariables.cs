using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace EcosystemSim
{
    public static class DebugVariables
    {
        private static Vector2 pos;
        public static void DrawDebug()
        {
            pos = new Vector2(10, 10);
            DrawString($"Selected tile type: {InputManager.selectedTileType}");
            DrawString($"Selected grid: {GridManager.selectedGrid.gridName}");
            for (int i = 0; i < GridManager.grids.Count; i++)
            {
                int nonEmptyTilesCount = GridManager.grids[i].tiles.Cast<Tile>().Count(tile => tile.tileType != TileType.Empty);
                DrawString($"Grid{i}_{GridManager.grids[i].gridName} non-empty tiles count: {nonEmptyTilesCount}");
            }
            //DrawString($"Amount of Herbivores: {SceneData.herbivores.Count}");
            //DrawString($"Amount of Predators: {SceneData.predators.Count}");
            //DrawString($"Mouse in world: {InputManager.mousePositionInWorld}");
            //DrawString($"Mouse on UI: {InputManager.mousePositionOnScreen}");
            //DrawString($"Mouse out of bounds: {InputManager.mouseOutOfBounds}");


            Tile tile = InputManager.tileOnHover;
            //Tile tile = scene.bgGrid.GetTileAtPos(InputManager.mousePositionInWorld);
            if (tile != null)
            {
                DrawString($"Hover tile type: {tile.tileType}");
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
