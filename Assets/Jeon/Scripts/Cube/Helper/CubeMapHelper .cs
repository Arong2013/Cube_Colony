using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public static class CubeMapHelper
{
    private static List<Vector2Int> GetFaceVector2List(int size, int offsetX, int offsetY)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int posX =  x + offsetX;
                int posY = y + offsetY;
                positions.Add(new Vector2Int(posX, posY));
            }
        }
        return positions;   
    }
    //앞 뒤 좌 우 위 아래
    private static List<Vector2Int> GetEmptyMap(int size)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        positions.AddRange(GetFaceVector2List(size, 0, 0));         // 예: 앞면
        positions.AddRange(GetFaceVector2List(size, size * 2, 0));    // 예: 뒷 면
        positions.AddRange(GetFaceVector2List(size, -size, 0));       // 예: 왼쪽 면
        positions.AddRange(GetFaceVector2List(size, size, 0));        // 예:  오른쪽 면
        positions.AddRange(GetFaceVector2List(size, 0, size));        // 예: 윗면
        positions.AddRange(GetFaceVector2List(size, 0, -size));       // 예: 아랫면
        return positions;
    }
    private static List<Vector2Int> BulidEmptyMapforAstar(int size)
    {
        List<Vector2Int> positions = GetEmptyMap(size);
        positions.AddRange(GetFaceVector2List(size, -size * 2, 0));
        positions.AddRange(GetFaceVector2List(size, 0, size * 2).AsEnumerable().Reverse().ToList());
        positions.AddRange(GetFaceVector2List(size, 0, -size * 2).AsEnumerable().Reverse().ToList());
        return positions;
    }

    private static Dictionary<CubeFaceType, List<CubieFace>> SetCubieFaceMap(List<Cubie> allCubies,Cubie[,,] cubieGrid)
    {
        int size = cubieGrid.GetLength(0);  

        Dictionary<CubeFaceType, List<CubieFace>> cubieFaceMap = new Dictionary<CubeFaceType, List<CubieFace>>()
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
            int x = GridSearchHelper.FindLayer(cubie, CubeAxisType.X,cubieGrid);
            int y = GridSearchHelper.FindLayer(cubie, CubeAxisType.Y,cubieGrid);
            int z = GridSearchHelper.FindLayer(cubie, CubeAxisType.Z, cubieGrid);
            foreach (var face in cubie.GetComponentsInChildren<CubieFace>())
            {
                CubeFaceType faceType = face.face;

                switch (faceType)
                {
                    case CubeFaceType.Front:
                        if (z != 0) continue;  // Front 면은 z = 0일 때만 유효
                        cubieFaceMap[CubeFaceType.Front].Add(face);
                        break;
                    case CubeFaceType.Back:
                        if (z != size - 1) continue;  // Back 면은 z = size - 1일 때만 유효
                        cubieFaceMap[CubeFaceType.Back].Add(face);
                        break;
                    case CubeFaceType.Top:
                        if (y != size - 1) continue;  // Top 면은 y = size - 1일 때만 유효
                        cubieFaceMap[CubeFaceType.Top].Add(face);
                        break;
                    case CubeFaceType.Bottom:
                        if (y != 0) continue;  // Bottom 면은 y = 0일 때만 유효
                        cubieFaceMap[CubeFaceType.Bottom].Add(face);
                        break;
                    case CubeFaceType.Left:
                        if (x != 0) continue;  // Left 면은 x = 0일 때만 유효
                        cubieFaceMap[CubeFaceType.Left].Add(face);
                        break;
                    case CubeFaceType.Right:
                        if (x != size - 1) continue;  // Right 면은 x = size - 1일 때만 유효
                        cubieFaceMap[CubeFaceType.Right].Add(face);
                        break;
                }
            }
        }

        return cubieFaceMap;
    }

    private static void SortFaces(List<CubieFace> cubieFaces, CubeAxisType primaryAxis, bool primaryAscending, CubeAxisType secondaryAxis, bool secondaryAscending, Cubie[,,] cubieGrid)
    {
        cubieFaces.Sort((face1, face2) =>
        {
            // 첫 번째 축 기준 정렬 (FindLayer 사용)
            int primaryValue1 = GridSearchHelper.FindLayer(face1.cubie, primaryAxis, cubieGrid);
            int primaryValue2 = GridSearchHelper.FindLayer(face2.cubie, primaryAxis, cubieGrid);

            // 첫 번째 축 기준 비교
            int result = primaryAscending ? primaryValue1.CompareTo(primaryValue2) : primaryValue2.CompareTo(primaryValue1);

            // 첫 번째 축 값이 동일한 경우 두 번째 축 기준으로 정렬
            if (result == 0)
            {
                int secondaryValue1 = GridSearchHelper.FindLayer(face1.cubie, secondaryAxis, cubieGrid);
                int secondaryValue2 = GridSearchHelper.FindLayer(face2.cubie, secondaryAxis, cubieGrid);

                // 두 번째 축 기준 비교
                result = secondaryAscending ? secondaryValue1.CompareTo(secondaryValue2) : secondaryValue2.CompareTo(secondaryValue1);
            }

            return result;
        });
    }
    private static void SortCubieFaceMap(Dictionary<CubeFaceType, List<CubieFace>> cubieFaceMap, Cubie[,,] cubieGrid)
    {
        foreach (var faceType in cubieFaceMap.Keys)
        {
            switch (faceType)
            {
                case CubeFaceType.Front:
                    // Front 면은 X 값 기준으로 정렬 (작은 순서대로), 그 다음 Z 값 기준으로 (큰 순서대로)
                    SortFaces(cubieFaceMap[faceType], CubeAxisType.X, true, CubeAxisType.Z, false, cubieGrid);
                    break;

                case CubeFaceType.Back:
                    // Back 면은 X 값 기준으로 정렬 (큰 순서대로), 그 다음 Z 값 기준으로 (큰 순서대로)
                    SortFaces(cubieFaceMap[faceType], CubeAxisType.X, false, CubeAxisType.Z, false, cubieGrid);
                    break;

                case CubeFaceType.Left:
                    // Left 면은 Z 값 기준으로 정렬 (작은 순서대로), 그 다음 X 값 기준으로 (작은 순서대로)
                    SortFaces(cubieFaceMap[faceType], CubeAxisType.Z, false, CubeAxisType.X, true, cubieGrid);
                    break;

                case CubeFaceType.Right:
                    // Right 면은 Z 값 기준으로 정렬 (큰 순서대로), 그 다음 X 값 기준으로 (작은 순서대로)
                    SortFaces(cubieFaceMap[faceType], CubeAxisType.Z, true, CubeAxisType.X, true, cubieGrid);
                    break;

                case CubeFaceType.Top:
                    // Top 면은 X 값 기준으로 정렬 (작은 순서대로), 그 다음 Z 값 기준으로 (큰 순서대로)
                    SortFaces(cubieFaceMap[faceType], CubeAxisType.X, true, CubeAxisType.Z, true, cubieGrid);
                    break;

                case CubeFaceType.Bottom:
                    // Bottom 면은 X 값 기준으로 정렬 (큰 순서대로), 그 다음 Z 값 기준으로 (큰 순서대로)
                    SortFaces(cubieFaceMap[faceType], CubeAxisType.X, true, CubeAxisType.Z, false, cubieGrid);
                    break;
            }
        }

    }

    private static List<CubeFaceType> GetFaceOderList(bool IsAstar)
    {
        List<CubeFaceType> faceOrderlist = new List<CubeFaceType>();

        if(IsAstar)
        {
            faceOrderlist.AddRange(new[] { CubeFaceType.Front, CubeFaceType.Back, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Top, CubeFaceType.Bottom, CubeFaceType.Back, CubeFaceType.Back, CubeFaceType.Back });            
        }
        else
        {
            faceOrderlist.AddRange(new[] { CubeFaceType.Front, CubeFaceType.Back, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Top, CubeFaceType.Bottom });
        }

        return faceOrderlist;   
    }
    private static Dictionary<Vector2Int, CubieFace> AddCubieFaceInEmptyMap(Dictionary<CubeFaceType, List<CubieFace>> cubieFaceMap, List<Vector2Int> positions)
    {
        Dictionary<Vector2Int, CubieFace> v2CubieFaceMap = new Dictionary<Vector2Int, CubieFace>();
        List<CubeFaceType> faceOrder = GetFaceOderList(false);
        int positionIndex = 0; 

        foreach (var faceType in faceOrder)
        {
            var faces = cubieFaceMap[faceType];
            for (int i = 0; i < faces.Count; i++)
            {
                var face = faces[i];
                Vector2Int pos = positions[i + positionIndex];
                v2CubieFaceMap[pos] = face;
            }
            positionIndex += 9;
        }
        return v2CubieFaceMap;
    }
    public static Dictionary<Vector2Int, CubieFace> GetFaceMap(Cubie[,,] cubieGrid)
    {
        int size = cubieGrid.GetLength(0);
        List<Vector2Int> positions = GetEmptyMap(size);
        var allCubies = GridSearchHelper.GetAllCubies(cubieGrid);
        Dictionary<CubeFaceType, List<CubieFace>> cubieFaceMap = SetCubieFaceMap(allCubies, cubieGrid);
        SortCubieFaceMap(cubieFaceMap, cubieGrid);
        return AddCubieFaceInEmptyMap(cubieFaceMap,positions);
    }
}