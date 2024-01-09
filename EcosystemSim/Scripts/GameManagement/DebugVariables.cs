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
            DrawString($"Selected gridIndex: {GridManager.GridIndex}");

            if (GridManager.selectedGrid == null) return;
            DrawString($"Selected grid: {GridManager.selectedGrid.gridName}");
            DrawString($"Selected grid layerDepth: {GridManager.selectedGrid.layerDepth}");
            for (int i = 0; i < GridManager.grids.Count; i++)
            {
                //if (GridManager.grids[i].tiles[0,0] == null) break;
                int nonEmptyTilesCount = GridManager.grids[i].tiles.Cast<Tile>().Count(tile => tile.tileType != TileType.Empty);
                int plantTiles = GridManager.grids[i].tiles.Cast<Tile>().Count(tile => tile.selectedPlant != null);
                DrawString($"Grid{i}_{GridManager.grids[i].gridName}, tiles count: {nonEmptyTilesCount} + plant tiles: {plantTiles} / {GridManager.grids[i].maxAmountOfPlants}");
            }
            //DrawString($"Amount of Herbivores: {SceneData.herbivores.Count}");
            //DrawString($"Amount of Predators: {SceneData.predators.Count}");
            //DrawString($"Mouse in world: {InputManager.mousePositionInWorld}");
            //DrawString($"Mouse on UI: {InputManager.mousePositionOnScreen}");
            //DrawString($"Mouse out of bounds: {InputManager.mouseOutOfBounds}");


            Tile tile = InputManager.tileOnHover;
            if (tile != null)
            {
                DrawString($"Hover tile type: {tile.tileType}");
                DrawString($"Hover tile grid pos: ({tile.gridPos[0]}, {tile.gridPos[1]})");
                DrawString($"Hover tile pos: {tile.position}");
                DrawString($"Hover tile layerDepth: {tile.layerDepth}");
                DrawString($"Hover tile layerDepth: {tile.canGrowPlants} + {tile.selectedPlant?.texture.Name}");
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
