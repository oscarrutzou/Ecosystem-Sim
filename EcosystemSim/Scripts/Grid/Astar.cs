using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace EcosystemSim
{
    public class Astar
    {
        List<Grid> grids;
        public Stack<Tile> lastPath;

        int GridRows { get { return grids[0].rows; } }
        int GridCols { get { return grids[0].collumns; } }
        int NodeSize { get { return Tile.NodeSize; } }

        public Astar(List<Grid> grids)
        {
            this.grids = grids;
        }

        public Stack<Tile> FindPath(Vector2 Start, Vector2 End)
        {
            foreach (var grid in grids)
            {
                Tile start = grid.GetTile(Start);
                Tile end = grid.GetTile(End);

                if (start == null || end == null)
                    throw new ArgumentException("Error, can't find start or end tile");

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

                    foreach (Tile next in GetAdjacencies(current, grid))
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
                                float priority = newCost + Math.Abs(end.position.X - next.position.X) + Math.Abs(end.position.Y - next.position.Y);
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

                lastPath = path;
                return path;
            }

            return null;
        }

        private List<Tile> GetAdjacencies(Tile t, Grid grid)
        {
            List<Tile> temp = new List<Tile>();
            int x = t.gridPos[0];
            int y = t.gridPos[1];
            int GridRows = grid.rows;
            int GridCols = grid.collumns;

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

                // Check if the new position is within the grid
                if (nx >= 0 && nx < GridCols && ny >= 0 && ny < GridRows)
                {
                    // For diagonal directions, check if the tiles directly to each side are walkable
                    if (Math.Abs(direction.dx) != Math.Abs(direction.dy) || (grid.GetTile(x + direction.dx, y).isWalkable && grid.GetTile(x, y + direction.dy).isWalkable))
                    {
                        Tile tempTile = grid.GetTile(nx, ny);
                        if (tempTile != null && tempTile.isWalkable)
                        {
                            temp.Add(tempTile);
                        }
                    }
                }
            }

            return temp;
        }


        public void UpdateDebugColor()
        {
            if (lastPath == null) return;

            foreach (var grid in grids)
            {
                foreach (Tile t in grid.tiles)
                {
                    if (t.color != Color.Gray){
                        t.color = Color.White;
                    }
                        
                }

                foreach (Tile tile in lastPath)
                {
                    Tile gridTile = grid.GetTile(tile.gridPos[0], tile.gridPos[1]);

                    if (gridTile != null)
                    {
                        if (tile == lastPath.First())
                        {
                            gridTile.color = Color.Blue;
                        }
                        else if (tile == lastPath.Last())
                        {
                            gridTile.color = Color.Red;
                        }
                        else
                        {
                            gridTile.color = Color.Yellow;
                        }
                    }
                }
            }
        }

    }
}
