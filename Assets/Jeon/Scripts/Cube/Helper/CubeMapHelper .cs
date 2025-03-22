using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public static class CubeMapHelper
{
    private static void AddFaceTiles(List<Vector2Int> positions, int size, int offsetX, int offsetY, bool IsPlusX , bool IsPlusY)
    {
        int xFactor = IsPlusX ? 1 : -1;
        int yFactor = IsPlusY ? 1 : -1;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // 타일 위치 계산
                int posX = xFactor * x + offsetX;
                int posY = yFactor * y + offsetY;

                positions.Add(new Vector2Int(posX, posY));
            }
        }
    }
    //앞 뒤 좌 우 위 아래
    public static List<Vector2Int> BuildEmptyMap(int size)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        AddFaceTiles(positions, size, 0, 0, true, true);
        AddFaceTiles(positions, size, size * 2, 0, true, true);
        AddFaceTiles(positions, size, -size, 0, true, true);
        AddFaceTiles(positions, size, size, 0, true, true);
        AddFaceTiles(positions, size, 0, size, true, true);
        AddFaceTiles(positions, size, 0, -size, true, true);
        return positions;   
    }

    //앞 뒤 좌 우 위 아래 좌 위 아래
    public static List<Vector2Int> BulidEmptyMapforAstar(int size)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        AddFaceTiles(positions, size, 0, 0, true, true);
        AddFaceTiles(positions, size, size * size - 1, 0, false, true);
        AddFaceTiles(positions, size, -1, 0, false, true);
        AddFaceTiles(positions, size, size, 0, true, true);
        AddFaceTiles(positions, size, 0, size, true, true);
        AddFaceTiles(positions, size, 0, -1, true, false);
        AddFaceTiles(positions, size, -size - 1, 0, false, true);
        AddFaceTiles(positions, size, 0, size*2, true, true);
        AddFaceTiles(positions, size, 0, -size - 1, true, false);

        return positions;
    }

    public static Dictionary<CubeFaceType, List<CubieFace>> FillCubieFaceMap(List<Cubie> allCubies,Cubie[,,] cubieGrid)
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
            // 큐브 위치 얻기
            int x = GridSearchHelper.FindLayer(cubie, CubeAxisType.X,cubieGrid);
            int y = GridSearchHelper.FindLayer(cubie, CubeAxisType.Y,cubieGrid);
            int z = GridSearchHelper.FindLayer(cubie, CubeAxisType.Z, cubieGrid);

            // 큐브의 각 면에 맞는 위치에 배치
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
    public static void SortCubieFaceMap(Dictionary<CubeFaceType, List<CubieFace>> cubieFaceMap, Cubie[,,] cubieGrid)
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
    public static List<CubeFaceType> GetFaceOrder(CubeFaceType centerFace)
    {
        List<CubeFaceType> faceOrder = new List<CubeFaceType>();

        switch (centerFace)
        {
            case CubeFaceType.Top:
                faceOrder.AddRange(new[] { CubeFaceType.Top, CubeFaceType.Bottom, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Back, CubeFaceType.Front });
                break;
            case CubeFaceType.Bottom:
                faceOrder.AddRange(new[] { CubeFaceType.Bottom, CubeFaceType.Top, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Front, CubeFaceType.Back });
                break;
            case CubeFaceType.Front:
                faceOrder.AddRange(new[] { CubeFaceType.Front, CubeFaceType.Back, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Top, CubeFaceType.Bottom });
                break;
            case CubeFaceType.Back:
                faceOrder.AddRange(new[] { CubeFaceType.Back, CubeFaceType.Front, CubeFaceType.Right, CubeFaceType.Left, CubeFaceType.Top, CubeFaceType.Bottom });
                break;
            case CubeFaceType.Left:
                faceOrder.AddRange(new[] { CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Back, CubeFaceType.Front, CubeFaceType.Top, CubeFaceType.Bottom });
                break;
            case CubeFaceType.Right:
                faceOrder.AddRange(new[] { CubeFaceType.Right, CubeFaceType.Left, CubeFaceType.Front, CubeFaceType.Back, CubeFaceType.Top, CubeFaceType.Bottom });
                break;
        }

        return faceOrder;
    }

    public static List<CubeFaceType> GetFaceOrderforAstar(CubeFaceType centerFace)
    {
        List<CubeFaceType> faceOrder = new List<CubeFaceType>();

        switch (centerFace)
        {
            case CubeFaceType.Top:
                faceOrder.AddRange(new[] { CubeFaceType.Top, CubeFaceType.Bottom, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Back, CubeFaceType.Front , CubeFaceType.Bottom , CubeFaceType.Bottom , CubeFaceType.Bottom });
                break;
            case CubeFaceType.Bottom:
                faceOrder.AddRange(new[] { CubeFaceType.Bottom, CubeFaceType.Top, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Front, CubeFaceType.Back, CubeFaceType.Top, CubeFaceType.Top, CubeFaceType.Top });
                break;
            case CubeFaceType.Front:
                faceOrder.AddRange(new[] { CubeFaceType.Front, CubeFaceType.Back, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Top, CubeFaceType.Bottom , CubeFaceType.Back , CubeFaceType.Back , CubeFaceType.Back });
                break;
            case CubeFaceType.Back:
                faceOrder.AddRange(new[] { CubeFaceType.Back, CubeFaceType.Front, CubeFaceType.Right, CubeFaceType.Left, CubeFaceType.Top, CubeFaceType.Bottom , CubeFaceType.Front , CubeFaceType.Front , CubeFaceType.Front });
                break;
            case CubeFaceType.Left:
                faceOrder.AddRange(new[] { CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Back, CubeFaceType.Front, CubeFaceType.Top, CubeFaceType.Bottom, CubeFaceType.Right, CubeFaceType.Right, CubeFaceType.Right });
                break;
            case CubeFaceType.Right:
                faceOrder.AddRange(new[] { CubeFaceType.Right, CubeFaceType.Left, CubeFaceType.Front, CubeFaceType.Back, CubeFaceType.Top, CubeFaceType.Bottom, CubeFaceType.Left, CubeFaceType.Left, CubeFaceType.Left });
                break;
        }

        return faceOrder;
    }


    public static Dictionary<Vector2Int, CubieFace> BuildCubieFaceMap(Dictionary<CubeFaceType, List<CubieFace>> cubieFaceMap, List<Vector2Int> positions, CubeFaceType centerFace)
    {
        Dictionary<Vector2Int, CubieFace> v2CubieFaceMap = new Dictionary<Vector2Int, CubieFace>();
        List<CubeFaceType> faceOrder = GetFaceOrder(centerFace);
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

}