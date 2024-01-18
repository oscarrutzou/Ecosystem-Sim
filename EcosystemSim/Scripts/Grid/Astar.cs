using Microsoft.Xna.Framework;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace EcosystemSim
{
    public class Astar
    {
        private List<Grid> grids = GridManager.grids;
        public bool startNTargetPosSame;
        public Stack<Tile> FindPath(Vector2 Start, Vector2 End)
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
                if (start.gridPos == end.gridPos)
                {
                    startNTargetPosSame = true;
                    return null;
                }
            }

            startNTargetPosSame = false;

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
                return path;
            }

            return null;
        }

        private List<Tile> GetAdjacencies(Tile t)
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
