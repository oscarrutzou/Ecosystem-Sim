﻿using Microsoft.Xna.Framework;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace EcosystemSim
{
    public static class Astar
    {
        private static List<Grid> grids = GridManager.grids;

        public static Stack<Tile> FindPath(Vector2 Start, Vector2 End)
        {
            if (grids.Count == 0) return null;

            foreach (var grid in grids)
            {
                Tile start = grid.GetTile(Start);
                Tile end = grid.GetTile(End);
                if (start == null || !start.isWalkable && start.tileType != TileType.Empty)
                    return null;
                if (end == null || !end.isWalkable && end.tileType != TileType.Empty)
                    return null;
            }

            foreach (var grid in grids)
            {
                Tile start = grid.GetTile(Start);
                Tile end = grid.GetTile(End);

                Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
                Dictionary<Tile, float> costSoFar = new Dictionary<Tile, float>();
                PriorityQueue<Tile, float> frontier = new PriorityQueue<Tile, float>();

                frontier.Enqueue(start, 0);
                cameFrom[start] = null;
                costSoFar[start] = 0;

                while (frontier.Count > 0)
                {
                    var current = frontier.Dequeue();

                    if (current.Equals(end))
                        break;

                    foreach (Tile next in GetAdjacencies(current))
                    {
                        // Check if the tile is walkable in all grids
                        if (grids.All(g =>
                        {
                            Tile tile = g.GetTile(next.gridPos[0], next.gridPos[1]);
                            return tile != null && tile.isWalkable || tile != null && tile.tileType == TileType.Empty;
                        }))
                        {
                            float newCost = costSoFar[current] + next.cost;

                            if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                            {
                                costSoFar[next] = newCost;
                                float first = Math.Abs(end.position.X - next.position.X);
                                float second = Math.Abs(end.position.Y - next.position.Y);
                                float priority = newCost + (float)Math.Sqrt(Math.Pow(first, 2) + Math.Pow(second, 2)); // Euclidean distance
                                frontier.Enqueue(next, priority);
                                cameFrom[next] = current;
                            }
                        }
                    }
                }

                if (!cameFrom.ContainsKey(end))
                    return null;

                var path = new Stack<Tile>();
                var temp = end;

                while (temp != null)
                {
                    path.Push(temp);
                    temp = cameFrom[temp];
                }
                
                //optimize with saving last path in agent, then use that and check if the new path.count is larger that oldpath,
                //then check if you start at the end of last path, and search to the new pos, and use that count - 2 (since we shouldnt count start and end tiles again) and add the lastpath. Compare to newPath.count.

                //lastPath = path;
                return path;
            }

            return null;
        }
        //Does nada:/
        public static Stack<Tile> OptimizePath(Stack<Tile> path, Grid grid)
        {
            Stack<Tile> optimizedPath = new Stack<Tile>();
            Tile current = path.Pop();
            optimizedPath.Push(current);

            while (path.Count > 0)
            {
                Tile next = path.Peek();

                // Check if there's a direct line of sight between the current tile and the next tile
                if (HasLineOfSight(current, next, grid))
                {
                    // If there's a line of sight, remove the next tile from the original path
                    // but don't add it to the optimized path yet
                    path.Pop();
                }
                else
                {
                    // If there's no line of sight, move to the next tile
                    current = path.Pop();
                    optimizedPath.Push(current);
                }
            }

            return optimizedPath;
        }

        public static bool HasLineOfSight(Tile start, Tile end, Grid grid)
        {
            List<Vector2> points = GetPointsOnLine(start.gridPos[0], start.gridPos[1], end.gridPos[0], end.gridPos[1]);

            foreach (Vector2 point in points)
            {
                // Get all tiles at this point in all grids
                List<Tile> tilesAtPoint = grids.Select(g => g.GetTile(point)).Where(tile => tile != null).ToList();

                // If any tile at this point is not walkable, return false
                if (tilesAtPoint.Any(tile => !tile.isWalkable && tile.tileType != TileType.Empty))
                {
                    return false;
                }
            }

            return true;
        }

        //Bresenham’s line algorithm
        public static List<Vector2> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            List<Vector2> result = new List<Vector2>();

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);

            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                result.Add(new Vector2((steep ? y : x), (steep ? x : y)));
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }

            return result;
        }

        private static List<Tile> GetAdjacencies(Tile t)
        {
            List<Tile> temp = new List<Tile>();
            int x = t.gridPos[0];
            int y = t.gridPos[1];

            // Define the possible directions
            List<(int dx, int dy)> directions = new List<(int, int)>
            {
                (-1, 0), // Left
                (1, 0),  // Right
                (0, 1),  // Down
                (0, -1), // Up
                (-1, 1), // Left + Down
                (-1, -1), // Left + Up
                (1, 1), // Right + Down
                (1, -1) // Right + Up
            };

            foreach (var direction in directions)
            {
                int nx = x + direction.dx;
                int ny = y + direction.dy;

                // Check all grids for a valid tile at the new position
                foreach (var grid in grids)
                {
                    if (nx >= 0 && nx < Grid.collumns && ny >= 0 && ny < Grid.rows)
                    {
                        Tile tempTile = grid.GetTile(nx, ny);
                        if (tempTile != null && tempTile.tileType != TileType.Empty)
                        {
                            temp.Add(tempTile);
                        }
                    }
                }
            }
            // Filter out diagonally adjacent tiles with non-walkable tiles on their straight sides
            temp = temp.Where(tile =>
            {
                // Check if the tile is diagonally adjacent
                if (Math.Abs(tile.gridPos[0] - t.gridPos[0]) == 1 && Math.Abs(tile.gridPos[1] - t.gridPos[1]) == 1)
                {
                    // Check the tiles directly to each side
                    bool isWalkableSide1 = grids.Select(grid => grid.GetTile(t.gridPos[0] + (tile.gridPos[0] - t.gridPos[0]), t.gridPos[1])).All(adjTile => adjTile != null && adjTile.isWalkable);
                    bool isWalkableSide2 = grids.Select(grid => grid.GetTile(t.gridPos[0], t.gridPos[1] + (tile.gridPos[1] - t.gridPos[1]))).All(adjTile => adjTile != null && adjTile.isWalkable);

                    // If both sides are walkable, the tile is valid
                    // If either side is not walkable, the tile is not valid
                    return isWalkableSide1 && isWalkableSide2;
                }

                // If the tile is not diagonally adjacent, it is valid
                return true;
            }).ToList();

            // Filter out tiles that are not walkable
            temp = temp.Where(tile => tile.isWalkable).ToList();

            return temp;
        }

        //public static void UpdateDebugColor()
        //{
        //    if (lastPath == null) return;

        //    foreach (var grid in grids)
        //    {
        //        foreach (Tile t in grid.tiles)
        //        {
        //            if (t.color != Color.Gray){
        //                t.color = Color.White;
        //            }
                        
        //        }

        //        foreach (Tile tile in lastPath)
        //        {
        //            Tile gridTile = grid.GetTile(tile.gridPos[0], tile.gridPos[1]);

        //            if (gridTile != null)
        //            {
        //                if (tile == lastPath.First())
        //                {
        //                    gridTile.color = Color.Blue;
        //                }
        //                else if (tile == lastPath.Last())
        //                {
        //                    gridTile.color = Color.Red;
        //                }
        //                else
        //                {
        //                    gridTile.color = Color.Yellow;
        //                }
        //            }
        //        }
        //    }
        //}

    }
}
