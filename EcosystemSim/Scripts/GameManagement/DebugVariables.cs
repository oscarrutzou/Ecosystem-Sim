using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using SharpDX.Direct2D1.Effects;

namespace EcosystemSim
{
    public static class DebugVariables
    {
        private static Vector2 pos;
        public static double herbivoresAlive;

        public static Color selectedGridColor = Color.Green;
        public static Color debugNonWalkableTilesColor = Color.DeepPink;

        public static void DrawDebug()
        {
            pos = new Vector2(10, 10);

            if (SceneData.herbivores.Count > 0){
                herbivoresAlive += GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds * GameWorld.Instance.gameSpeed;
            }
            //DrawString($"Selected tile type: {InputManager.selectedTileType}");
            //DrawString($"Selected gridIndex: {GridManager.GridIndex}");



            //if (GridManager.selectedGrid == null) return;
            //DrawString($"Selected grid: {GridManager.selectedGrid.gridName}");
            //DrawString($"Selected grid layerDepth: {GridManager.selectedGrid.layerDepth}");
            //for (int i = 0; i < GridManager.grids.Count; i++)
            //{
            //    //if (GridManager.grids[i].tiles[0,0] == null) break;
            //    int nonEmptyTilesCount = GridManager.grids[i].tiles.Cast<Tile>().Count(tile => tile.tileType != TileType.Empty);
            //    int plantTiles = GridManager.grids[i].tiles.Cast<Tile>().Count(tile => tile.selectedPlant != null);
            //    DrawString($"Grid{i}_{GridManager.grids[i].gridName}, tiles count: {nonEmptyTilesCount} + plant tiles: {plantTiles} / {GridManager.grids[i].maxAmountOfPlants}");
            //}
            //DrawString($"Amount of plants: {SceneData.plants.Count}");
            ////DrawString($"Amount of Predators: {SceneData.predators.Count}");
            ////DrawString($"Mouse in world: {InputManager.mousePositionInWorld}");
            ////DrawString($"Mouse on UI: {InputManager.mousePositionOnScreen}");
            ////DrawString($"Mouse out of bounds: {InputManager.mouseOutOfBounds}");
            //if (InputManager.tileOnHover != null)
            //{
            //    DrawString($"Grid index hover: {InputManager.tileOnHover.gridPos[0]},{InputManager.tileOnHover.gridPos[1]}");

            //}

            //if (GameWorld.Instance.currentScene is TestScene scene && scene.astar.lastPath != null)
            //{
            //    DrawString($"Start pos: {GridManager.GetTileAtPos(InputManager.startPos).gridPos[0]},{GridManager.GetTileAtPos(InputManager.startPos).gridPos[1]}");
            //    DrawString($"Start astar pos: {scene.astar.lastPath.First().gridPos[0]},{scene.astar.lastPath.First().gridPos[1]}");
            //    DrawString($"End pos: {GridManager.GetTileAtPos(InputManager.endPos).gridPos[0]},{GridManager.GetTileAtPos(InputManager.endPos).gridPos[1]}");
            //    DrawString($"End astar pos: {scene.astar.lastPath.Last().gridPos[0]},{scene.astar.lastPath.Last().gridPos[1]}");
            //}

            DrawString($"Build Mode: {InputManager.buildMode}");
            DrawString($"GameSpeed: {GameWorld.Instance.gameSpeed}");
            DrawString($"herbivoresAlive timer: {herbivoresAlive}");
            DrawString($"Plants : {SceneData.plants.Count} / {GridManager.grids[0].maxAmountOfPlants}");
            DrawString($"Herbivore count: {SceneData.herbivores.Count}");
            DrawString($"DeathByThristCounter count: {Agent.deathByThristCounter}");
            DrawString($"DeathByHungerCounter count: {Agent.deathByHungerCounter}");

            if (InputManager.buildMode)
            {
                Tile tile = InputManager.tileOnHover;
                if (tile != null)
                {
                    DrawString($"Hover tile type: {tile.tileType}");
                    DrawString($"Hover tile hasBeenPlanted: {tile.hasBeenPlanted}");
                    DrawString($"Hover tile grid pos: ({tile.gridPos[0]}, {tile.gridPos[1]})");
                    DrawString($"Hover tile pos: {tile.position}");
                    DrawString($"Hover tile layerDepth: {tile.layerDepth}");
                    DrawString($"Hover tile layerDepth: {tile.canGrowPlants} + {tile.selectedPlant?.texture.Name}");
                }
            }

            else
            {
                GameObject obj = InputManager.objOnHover;
                if (obj != null)
                {
                    DrawString($"Hover obj pos: {obj.position}");
                    DrawString($"Hover obj layerDepth: {obj.layerDepth}");
                    if (obj is Agent agent)
                    {
                        DrawString($"Hover Agent hungermeter: {agent.hungermeter}");
                        DrawString($"Hover Agent thirstMeter: {agent.thirstMeter}");
                        //DrawString($"Hover Agent target: {agent.target}");
                        DrawString($"Hover Agent target list: {agent.targetObjectInRad.Count}");
                        DrawString($"Hover Agent target tileType: {agent.nextTargetTile?.tileType}");
                        DrawString($"Hover Agent state: {agent.currentState}");
                        int[] curPos = GridManager.grids[0].GetTile(agent.position).gridPos;
                        DrawString($"Hover Agent grid position: {curPos[0]}, {curPos[1]}");
                        DrawString($"Hover Agent target tile grid position: {agent.nextTargetTile?.gridPos[0]}, {agent.nextTargetTile?.gridPos[1]}");

                        if (agent.target != null)
                        {
                            int[] targetPos = GridManager.grids[0].GetTile(agent.target.position).gridPos;
                            DrawString($"Hover Agent target gridpos: {targetPos[0]}, {targetPos[1]}");
                        }

                    }
                }
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
