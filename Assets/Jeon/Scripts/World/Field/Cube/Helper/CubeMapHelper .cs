using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public static class CubeMapHelper
{
    // 지정한 위치에 면을 채우는 그리드 생성
    private static List<Vector2Int> GetFaceGrid(int size, int offsetX, int offsetY)
    {
        var positions = new List<Vector2Int>();
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                positions.Add(new Vector2Int(x + offsetX, y + offsetY));
            }
        }
        return positions;
    }

    // 큐브의 6면을 위한 기본 위치 목록 생성
    private static List<Vector2Int> GetBaseFaceLayout(int size)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        positions.AddRange(GetFaceGrid(size, 0, 0));         // 예: 앞면
        positions.AddRange(GetFaceGrid(size, size * 2, 0));    // 예: 뒷 면
        positions.AddRange(GetFaceGrid(size, -size, 0));       // 예: 왼쪽 면
        positions.AddRange(GetFaceGrid(size, size, 0));        // 예:  오른쪽 면
        positions.AddRange(GetFaceGrid(size, 0, size));        // 예: 윗면
        positions.AddRange(GetFaceGrid(size, 0, -size));       // 예: 아랫면
        return positions;
    }
    // A* 탐색을 위한 확장된 큐브 레이아웃 생성
    private static List<Vector2Int> GetExtendedLayoutForAstar(int size)
    {
        var positions = GetBaseFaceLayout(size);
        positions.AddRange(GetFaceGrid(size, -size * 2, 0));
        positions.AddRange(GetFaceGrid(size, 0, size * 2).AsEnumerable().Reverse());
        positions.AddRange(GetFaceGrid(size, 0, -size * 2).AsEnumerable().Reverse());
        return positions;
    }

 

    // 두 축 기준으로 CubieFace 리스트 정렬
    private static void SetCubieFaceSort(List<CubieFace> cubieFaces, CubeAxisType primaryAxis, bool primaryAsc, CubeAxisType secondaryAxis, bool secondaryAsc, Cubie[,,] cubieGrid)
    {
        cubieFaces.Sort((face1, face2) =>
        {
            int p1 = GridSearchHelper.FindLayer(face1.cubie, primaryAxis, cubieGrid);
            int p2 = GridSearchHelper.FindLayer(face2.cubie, primaryAxis, cubieGrid);
            int result = primaryAsc ? p1.CompareTo(p2) : p2.CompareTo(p1);

            if (result == 0)
            {
                int s1 = GridSearchHelper.FindLayer(face1.cubie, secondaryAxis, cubieGrid);
                int s2 = GridSearchHelper.FindLayer(face2.cubie, secondaryAxis, cubieGrid);
                result = secondaryAsc ? s1.CompareTo(s2) : s2.CompareTo(s1);
            }

            return result;
        });
    }

    // 전체 면 타입별로 정렬 적용
    private static void SetCubieFaceMapSorting(Dictionary<CubeFaceType, List<CubieFace>> faceMap, Cubie[,,] cubieGrid)
    {
        foreach (var faceType in faceMap.Keys)
        {
            switch (faceType)
            {
                case CubeFaceType.Front:
                    SetCubieFaceSort(faceMap[faceType], CubeAxisType.X, true, CubeAxisType.Z, false, cubieGrid);
                    break;
                case CubeFaceType.Back:
                    SetCubieFaceSort(faceMap[faceType], CubeAxisType.X, false, CubeAxisType.Z, false, cubieGrid);
                    break;
                case CubeFaceType.Left:
                    SetCubieFaceSort(faceMap[faceType], CubeAxisType.Z, false, CubeAxisType.X, true, cubieGrid);
                    break;
                case CubeFaceType.Right:
                    SetCubieFaceSort(faceMap[faceType], CubeAxisType.Z, true, CubeAxisType.X, true, cubieGrid);
                    break;
                case CubeFaceType.Top:
                    SetCubieFaceSort(faceMap[faceType], CubeAxisType.X, true, CubeAxisType.Z, true, cubieGrid);
                    break;
                case CubeFaceType.Bottom:
                    SetCubieFaceSort(faceMap[faceType], CubeAxisType.X, true, CubeAxisType.Z, false, cubieGrid);
                    break;
            }
        }
    }

    // Face 순서를 반환하는 도우미
    private static List<CubeFaceType> GetFaceTypeOrder(bool isAstar)
    {
        return isAstar
            ? new List<CubeFaceType> { CubeFaceType.Front, CubeFaceType.Back, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Top, CubeFaceType.Bottom, CubeFaceType.Back, CubeFaceType.Back, CubeFaceType.Back }
            : new List<CubeFaceType> { CubeFaceType.Front, CubeFaceType.Back, CubeFaceType.Left, CubeFaceType.Right, CubeFaceType.Top, CubeFaceType.Bottom };
    }

    // 위치에 큐비페이스 연결
    private static Dictionary<Vector2Int, CubieFace> GetPositionFaceMap(Dictionary<CubeFaceType, List<CubieFace>> faceMap, List<Vector2Int> positions)
    {
        var positionFaceMap = new Dictionary<Vector2Int, CubieFace>();
        var faceSequence = GetFaceTypeOrder(false);
        int offset = 0;

        foreach (var faceType in faceSequence)
        {
            var faces = faceMap[faceType];
            for (int i = 0; i < faces.Count; i++)
            {
                positionFaceMap[positions[i + offset]] = faces[i];
            }
            offset += 9;
        }
        return positionFaceMap;
    }


    // 외부 호출용: 최종 매핑 반환
    public static Dictionary<Vector2Int, CubieFace> GetFinalFaceMap(Cubie[,,] cubieGrid, bool isAstar)
    {
        int size = cubieGrid.GetLength(0);
        var layoutPositions = isAstar ? GetExtendedLayoutForAstar(size) : GetBaseFaceLayout(size);
        var allCubies = GridSearchHelper.GetAllCubies(cubieGrid);
        var faceMap = GridSearchHelper.GetCubieFaceMapByType(allCubies, cubieGrid);
        SetCubieFaceMapSorting(faceMap, cubieGrid);
        return GetPositionFaceMap(faceMap, layoutPositions);
    }
}