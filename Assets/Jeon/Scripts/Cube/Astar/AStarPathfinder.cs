using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStarPathfinder
{
    private class Node
    {
        public Vector2Int pos;
        public Node parent;
        public float gCost, hCost;
        public float fCost => gCost + hCost;

        public Node(Vector2Int pos, Node parent, float g, float h)
        {
            this.pos = pos;
            this.parent = parent;
            this.gCost = g;
            this.hCost = h;
        }
    }

    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    public static List<Vector2Int> FindPath(Cubie[,] map, Vector2Int start, Vector2Int goal)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        var open = new List<Node>();
        var closed = new HashSet<Vector2Int>();

        Node startNode = new Node(start, null, 0, Vector2Int.Distance(start, goal));
        open.Add(startNode);

        while (open.Count > 0)
        {
            Node current = open.OrderBy(n => n.fCost).First();
            open.Remove(current);
            closed.Add(current.pos);

            if (current.pos == goal)
            {
                List<Vector2Int> path = new();
                while (current != null)
                {
                    path.Add(current.pos);
                    current = current.parent;
                }
                path.Reverse();
                return path;
            }

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = current.pos + dir;
                if (neighborPos.x < 0 || neighborPos.x >= width || neighborPos.y < 0 || neighborPos.y >= height)
                    continue;

                Cubie cubie = map[neighborPos.x, neighborPos.y];
                if (cubie == null || !cubie.isWalkable)
                    continue;

                if (closed.Contains(neighborPos)) continue;

                float tentativeG = current.gCost + 1;
                Node existing = open.FirstOrDefault(n => n.pos == neighborPos);

                if (existing == null)
                {
                    open.Add(new Node(neighborPos, current, tentativeG, Vector2Int.Distance(neighborPos, goal)));
                }
                else if (tentativeG < existing.gCost)
                {
                    existing.parent = current;
                    existing.gCost = tentativeG;
                }
            }
        }

        return null; // 경로 없음
    }
}
