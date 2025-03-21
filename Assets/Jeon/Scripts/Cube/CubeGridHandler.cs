using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]

public class CubeGridHandler
{
    public readonly int size;
    private Cubie[,,] cubieGrid;
    private Dictionary<CubeFaceType, List<CubieFace>> faceMap;
    private Dictionary<Vector2Int, CubieFace> flatFaceMap;
    public List<CubieFace> cubieFaces = new List<CubieFace>();
    public CubeGridHandler(int _size, Cubie cubie, Transform parent)
    {
        size = _size;
        cubieGrid = new Cubie[size, size, size];

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                for (int z = 0; z < size; z++)
                {
                    Vector3 position = new Vector3(x - 1, y - 1, z - 1);
                    cubieGrid[x, y, z] = UnityEngine.Object.Instantiate(cubie, position, Quaternion.identity, parent);
                    cubieGrid[x, y, z].name = $"Cubie_{x}_{y}_{z}";
                }
        BuildFaceMap();
        BuildFlatNet(CubeFaceType.Right);
        LogValidFlatPositions();
    }
    public void LogValidFlatPositions()
    {
        foreach (var pos in flatFaceMap.Keys)
        {
            var face = flatFaceMap[pos];
            Debug.Log($"🧩 {pos} → {face.face}, Walkable: {face.cubie.isWalkable}");
        }
    }
    public void RotateLayer(int layer, bool isClockwise, CubeAxisType axis)
    {
        Cubie[,] face = CubieMatrixHelper.ExtractLayer(cubieGrid, layer, axis);     
        CubieManipulator.RotateCubies(face, isClockwise, axis);
        var rotated = CubieMatrixHelper.RotateMatrix(face, isClockwise, axis);
        CubieMatrixHelper.InsertLayer(cubieGrid, layer, rotated, axis);
        UpdateCubieNames();
    }

    public int FindLayer(Cubie cubie, CubeAxisType axis)
    {
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                for (int z = 0; z < size; z++)
                {
                    if (cubieGrid[x, y, z] == cubie)
                    {
                        return axis switch
                        {
                            CubeAxisType.X => x,
                            CubeAxisType.Y => y,
                            CubeAxisType.Z => z,
                            _ => -1
                        };
                    }
                }

        return -1;
    }

    public List<Cubie> GetAllCubies() => cubieGrid.Cast<Cubie>().ToList();
    public List<Cubie> GetCubiesInLayer(int layer, CubeAxisType axis)
    {
        List<Cubie> cubies = new List<Cubie>();

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                cubies.Add(axis switch
                {
                    CubeAxisType.X => cubieGrid[layer, i, j],
                    CubeAxisType.Y => cubieGrid[i, layer, j],
                    CubeAxisType.Z => cubieGrid[i, j, layer],
                    _ => null
                });
            }

        return cubies;
    }
    public void RotateEntireCube(bool isClockwise, CubeAxisType axis)
    {
        for (int layer = 0; layer < size; layer++)
            RotateLayer(layer, isClockwise, axis);
    }
    private void UpdateCubieNames()
    {
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                for (int z = 0; z < size; z++)
                {
                    if (cubieGrid[x, y, z] != null)
                    {
                        cubieGrid[x, y, z].name = $"Cubie_{x}_{y}_{z}";
                    }
                }
    }
    void BuildFaceMap()
    {
        faceMap = new();
        var allCubies = GetAllCubies();

        foreach (var cubie in allCubies)
        {
            foreach (var face in cubie.GetComponentsInChildren<CubieFace>())
            {
                if (!faceMap.ContainsKey(face.face))
                    faceMap[face.face] = new List<CubieFace>();

                faceMap[face.face].Add(face);
            }
        }
    }

    void BuildFlatNet(CubeFaceType center)
    {
        flatFaceMap = new();

        int s = size;

        var faceOffsets = GetFaceOffsets(center, s);

        foreach (var kvp in faceOffsets)
        {
            CubeFaceType faceType = kvp.Key;
            Vector2Int offset = kvp.Value;

            if (!faceMap.ContainsKey(faceType)) continue;
            List<CubieFace> faces = faceMap[faceType];

            foreach (var face in faces)
            {
                Vector3 localPos = face.transform.localPosition;
                int x = Mathf.RoundToInt(localPos.x + 1);
                int y = Mathf.RoundToInt(localPos.y + 1);
                int z = Mathf.RoundToInt(localPos.z + 1);

                Vector2Int pos2D = faceType switch
                {
                    CubeFaceType.Front => new Vector2Int(offset.x + x, offset.y + y),
                    CubeFaceType.Back => new Vector2Int(offset.x + (size - 1 - x), offset.y + y),
                    CubeFaceType.Left => new Vector2Int(offset.x + z, offset.y + y),
                    CubeFaceType.Right => new Vector2Int(offset.x + (size - 1 - z), offset.y + y),
                    CubeFaceType.Top => new Vector2Int(offset.x + x, offset.y + (size - 1 - z)),
                    CubeFaceType.Bottom => new Vector2Int(offset.x + x, offset.y + z),
                    _ => Vector2Int.zero
                };

                flatFaceMap[pos2D] = face;
            }
        }
    }

    Dictionary<CubeFaceType, Vector2Int> GetFaceOffsets(CubeFaceType center, int s)
    {
        return center switch
        {
            CubeFaceType.Front => new Dictionary<CubeFaceType, Vector2Int>
            {
                { CubeFaceType.Top,    new Vector2Int(1 * s, 0 * s) },
                { CubeFaceType.Left,   new Vector2Int(0 * s, 1 * s) },
                { CubeFaceType.Front,  new Vector2Int(1 * s, 1 * s) },
                { CubeFaceType.Right,  new Vector2Int(2 * s, 1 * s) },
                { CubeFaceType.Back,   new Vector2Int(3 * s, 1 * s) },
                { CubeFaceType.Bottom, new Vector2Int(1 * s, 2 * s) },
            },
            CubeFaceType.Left => new Dictionary<CubeFaceType, Vector2Int>
            {
                { CubeFaceType.Top,    new Vector2Int(1 * s, 0 * s) },
                { CubeFaceType.Back,   new Vector2Int(0 * s, 1 * s) },
                { CubeFaceType.Left,   new Vector2Int(1 * s, 1 * s) },
                { CubeFaceType.Front,  new Vector2Int(2 * s, 1 * s) },
                { CubeFaceType.Right,  new Vector2Int(3 * s, 1 * s) },
                { CubeFaceType.Bottom, new Vector2Int(1 * s, 2 * s) },
            },
            CubeFaceType.Right => new Dictionary<CubeFaceType, Vector2Int>
            {
                { CubeFaceType.Top,    new Vector2Int(1 * s, 0 * s) },
                { CubeFaceType.Front,  new Vector2Int(0 * s, 1 * s) },
                { CubeFaceType.Right,  new Vector2Int(1 * s, 1 * s) },
                { CubeFaceType.Back,   new Vector2Int(2 * s, 1 * s) },
                { CubeFaceType.Left,   new Vector2Int(3 * s, 1 * s) },
                { CubeFaceType.Bottom, new Vector2Int(1 * s, 2 * s) },
            },
            CubeFaceType.Back => new Dictionary<CubeFaceType, Vector2Int>
            {
                { CubeFaceType.Top,    new Vector2Int(1 * s, 0 * s) },
                { CubeFaceType.Right,  new Vector2Int(0 * s, 1 * s) },
                { CubeFaceType.Back,   new Vector2Int(1 * s, 1 * s) },
                { CubeFaceType.Left,   new Vector2Int(2 * s, 1 * s) },
                { CubeFaceType.Front,  new Vector2Int(3 * s, 1 * s) },
                { CubeFaceType.Bottom, new Vector2Int(1 * s, 2 * s) },
            },
            CubeFaceType.Top => new Dictionary<CubeFaceType, Vector2Int>
            {
                { CubeFaceType.Back,   new Vector2Int(1 * s, 0 * s) },
                { CubeFaceType.Left,   new Vector2Int(0 * s, 1 * s) },
                { CubeFaceType.Top,    new Vector2Int(1 * s, 1 * s) },
                { CubeFaceType.Right,  new Vector2Int(2 * s, 1 * s) },
                { CubeFaceType.Bottom,  new Vector2Int(3 * s, 1 * s) },
                { CubeFaceType.Front, new Vector2Int(1 * s, 2 * s) },
            },
            CubeFaceType.Bottom => new Dictionary<CubeFaceType, Vector2Int>
            {
                { CubeFaceType.Front,  new Vector2Int(1 * s, 0 * s) },
                { CubeFaceType.Left,   new Vector2Int(0 * s, 1 * s) },
                { CubeFaceType.Bottom, new Vector2Int(1 * s, 1 * s) },
                { CubeFaceType.Right,  new Vector2Int(2 * s, 1 * s) },
                { CubeFaceType.Top,   new Vector2Int(3 * s, 1 * s) },
                { CubeFaceType.Back,    new Vector2Int(1 * s, 2 * s) },
            },
            _ => throw new ArgumentOutOfRangeException(nameof(center), $"Unsupported center face: {center}")
        };
    }

    void LogFlatNet()
    {
        int width = 4 * size;
        int height = 3 * size;
        string log = "\n";

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pos = new Vector2Int(x, y);
                if (flatFaceMap.TryGetValue(pos, out var face))
                {
                    log += face.face switch
                    {
                        CubeFaceType.Front => "F ",
                        CubeFaceType.Back => "B ",
                        CubeFaceType.Left => "L ",
                        CubeFaceType.Right => "R ",
                        CubeFaceType.Top => "T ",
                        CubeFaceType.Bottom => "D ",
                        _ => "? "
                    };
                }
                else
                {
                    log += "  ";
                }
            }
            log += "\n";
        }

        Debug.Log(log);
    }
    public List<CubieFace> FindPath(Vector2Int start, Vector2Int goal)
    {
        var openSet = new PriorityQueue<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, int> { [start] = 0 };
        var fScore = new Dictionary<Vector2Int, int> { [start] = Heuristic(start, goal) };

        openSet.Enqueue(start, fScore[start]);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            foreach (var direction in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                var neighbor = current + direction;
                if (!flatFaceMap.ContainsKey(neighbor) || !flatFaceMap[neighbor].cubie.isWalkable)
                    continue;

                int tentativeG = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return null;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<CubieFace> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var totalPath = new List<CubieFace> { flatFaceMap[current] };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(flatFaceMap[current]);
        }
        totalPath.Reverse();
        return totalPath;
    }
}
public class PriorityQueue<T>
{
    private readonly List<(T item, int priority)> elements = new();
    public int Count => elements.Count;

    public void Enqueue(T item, int priority)
    {
        elements.Add((item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].priority < elements[bestIndex].priority)
                bestIndex = i;
        }
        var bestItem = elements[bestIndex].item;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public bool Contains(T item)
    {
        return elements.Any(e => EqualityComparer<T>.Default.Equals(e.item, item));
    }
}
