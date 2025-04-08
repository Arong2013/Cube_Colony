using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public static class GridSearchHelper
{
    // 큐브의 레이어를 찾는 메서드
    public static int FindLayer(Cubie cubie, CubeAxisType axis, Cubie[,,] cubieGrid)
    {
        int size = cubieGrid.GetLength(0);  // 3D 배열의 크기

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

    // 모든 큐브를 반환하는 메서드
    public static List<Cubie> GetAllCubies(Cubie[,,] cubieGrid) => cubieGrid.Cast<Cubie>().ToList();

    // 특정 레이어에 있는 큐브들을 반환하는 메서드
    public static List<Cubie> GetCubiesInLayer(int layer, CubeAxisType axis, Cubie[,,] cubieGrid)
    {
        int size = cubieGrid.GetLength(0);  // 3D 배열의 크기
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

    public static CubieFace GetCenterFace(CubeFaceType faceType, int size, Cubie[,,] cubieGrid)
    {
        int center = size / 2;

        int x = center;
        int y = center;
        int z = center;

        return faceType switch
        {
            CubeFaceType.Front => cubieGrid[x, y, 0].GetFace(CubeFaceType.Front),
            CubeFaceType.Back => cubieGrid[x, y, size - 1].GetFace(CubeFaceType.Back),
            CubeFaceType.Left => cubieGrid[0, y, z].GetFace(CubeFaceType.Left),
            CubeFaceType.Right => cubieGrid[size - 1, y, z].GetFace(CubeFaceType.Right),
            CubeFaceType.Top => cubieGrid[x, size - 1, z].GetFace(CubeFaceType.Top),
            CubeFaceType.Bottom => cubieGrid[x, 0, z].GetFace(CubeFaceType.Bottom),
            _ => null
        };
    }

    public static CubieFace GetCubieFaceInPos(CubeFaceType cubeFaceType, Vector3 pos, Cubie[,,] cubieGrid)
    {
        var allCube = GetAllCubies(cubieGrid);
        var dic = GetCubieFaceMapByType(cubieGrid);
        var closestFace = dic[cubeFaceType]
            .OrderBy(face => Vector3.Distance(face.transform.position, pos)) // 거리순 정렬
            .FirstOrDefault();
        return closestFace;

    }
    // 큐비로부터 각 면별 CubieFace 추출 및 분류
    public static Dictionary<CubeFaceType, List<CubieFace>> GetCubieFaceMapByType(Cubie[,,] cubieGrid)
    {
        int size = cubieGrid.GetLength(0);
        List<Cubie> allCubies = GetAllCubies(cubieGrid);    

        var faceMap = new Dictionary<CubeFaceType, List<CubieFace>>()
        {
            { CubeFaceType.Front, new List<CubieFace>() },
            { CubeFaceType.Back, new List<CubieFace>() },
            { CubeFaceType.Left, new List<CubieFace>() },
            { CubeFaceType.Right, new List<CubieFace>() },
            { CubeFaceType.Top, new List<CubieFace>() },
            { CubeFaceType.Bottom, new List<CubieFace>() }
        };

        foreach (var cubie in allCubies)
        {
            int x = FindLayer(cubie, CubeAxisType.X, cubieGrid);
            int y = FindLayer(cubie, CubeAxisType.Y, cubieGrid);
            int z = FindLayer(cubie, CubeAxisType.Z, cubieGrid);

            foreach (var face in cubie.GetComponentsInChildren<CubieFace>())
            {
                if ((face.face == CubeFaceType.Front && z != 0) ||
                    (face.face == CubeFaceType.Back && z != size - 1) ||
                    (face.face == CubeFaceType.Top && y != size - 1) ||
                    (face.face == CubeFaceType.Bottom && y != 0) ||
                    (face.face == CubeFaceType.Left && x != 0) ||
                    (face.face == CubeFaceType.Right && x != size - 1))
                    continue;

                faceMap[face.face].Add(face);
            }
        }

        return faceMap;
    }

    public static List<CubieFace> GetCubieFaces(CubeFaceType CubeFaceType, Cubie[,,] cubieGrid)
    {
        var faceMap = GetCubieFaceMapByType( cubieGrid);
        return faceMap[CubeFaceType];   
    }
    public static CubieFaceSkillType GetRandomSkill(Dictionary<CubieFaceSkillType, float> probs, System.Random rand)
    {
        float total = 0;
        foreach (var prob in probs.Values) total += prob;

        float randomValue = (float)(rand.NextDouble() * total);
        float cumulative = 0;

        foreach (var kv in probs)
        {
            cumulative += kv.Value;
            if (randomValue < cumulative)
                return kv.Key;
        }

        return CubieFaceSkillType.RMonster; // fallback
    }

    public static Vector3Int GetCubieGridPosition(Cubie cubie, Cubie[,,] cubieGrid)
    {
        int sizeX = cubieGrid.GetLength(0);
        int sizeY = cubieGrid.GetLength(1);
        int sizeZ = cubieGrid.GetLength(2);

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                {
                    if (cubieGrid[x, y, z] == cubie)
                    {
                        return new Vector3Int(x, y, z);
                    }
                }

        return new Vector3Int(-1, -1, -1); // 못 찾은 경우
    }

}
