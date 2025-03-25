using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[System.Serializable]
public class CubeGridHandler
{
    public readonly int Size;
    private Cubie[,,] cubieGrid;

    public Cubie[,,] GridSnapshot => GetGridCopy();

    // ✅ 생성자
    public CubeGridHandler(int size, Cubie cubiePrefab, Transform parent)
    {
        Size = size;
        cubieGrid = new Cubie[Size, Size, Size];

        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                for (int z = 0; z < Size; z++)
                {
                    Vector3 position = new Vector3(x - 1, y - 1, z - 1);
                    cubieGrid[x, y, z] = UnityEngine.Object.Instantiate(cubiePrefab, position, Quaternion.identity, parent);
                    cubieGrid[x, y, z].name = $"Cubie_{x}_{y}_{z}";
                }
    }

    // ✅ 외부 호출 메서드 - 회전
    public void RotateSingleLayer(Cubie referenceCubie, CubeAxisType axis, int rotationAmount)
    {
        int layerIndex = GridSearchHelper.FindLayer(referenceCubie, axis, GetGridCopy());
        bool clockwise = rotationAmount > 0;
        ApplyLayerRotation(layerIndex, clockwise, axis);
        RenameCubies();
    }

    public void RotateWholeCube(bool clockwise, CubeAxisType axis)
    {
        for (int layer = 0; layer < Size; layer++)
            ApplyLayerRotation(layer, clockwise, axis);
    }

    // ✅ 외부 호출 메서드 - 경로 탐색
    public List<CubieFace> GetAstarPathFaces(CubieFace startFace, CubieFace targetFace)
    {
        var referenceFace = startFace.face;
        try
        {
            ApplyViewAlignment(referenceFace, false);
            var pathfinder = new AStarPathfinding(CubeMapHelper.GetFinalFaceMap(GetGridCopy()), Size);
            return pathfinder.FindPath(startFace, targetFace);
        }
        finally
        {
            ApplyViewAlignment(referenceFace, true);
            RenameCubies();
        }
    }

    // ✅ 외부 호출 메서드 - 조회
    public List<Cubie> GetAllCubies() => GridSearchHelper.GetAllCubies(GetGridCopy());

    public List<Cubie> GetCubiesOnSameLayer(Cubie referenceCubie, CubeAxisType axis)
    {
        int layer = GridSearchHelper.FindLayer(referenceCubie, axis, GetGridCopy());
        return GridSearchHelper.GetCubiesInLayer(layer, axis, GetGridCopy());
    }
    public CubieFace GetCenterFace(CubeFaceType faceType) => GridSearchHelper.GetCenterFace(faceType, Size, GetGridCopy());

    // ✅ 내부 구현 - 레이어 회전 흐름
    private void ApplyLayerRotation(int layer, bool isClockwise, CubeAxisType axis)
    {
        Cubie[,] layerSlice = CubieMatrixHelper.ExtractLayer(GetGridCopy(), layer, axis);
        RotateCubies(layerSlice, isClockwise, axis);
        Cubie[,] rotated = CubieMatrixHelper.RotateMatrix(layerSlice, isClockwise, axis);
        InsertRotatedLayer(layer, rotated, axis);
        RenameCubies();
    }

    // ✅ 내부 구현 - 큐비 단위 회전
    public void RotateCubies(Cubie[,] cubies, bool isClockwise, CubeAxisType axis)
    {
        foreach (var cubie in cubies)
        {
            cubie.RotateCubie(axis, isClockwise);
        }
    }

    // ✅ 내부 구현 - A* 뷰 회전 처리
    private void ApplyViewAlignment(CubeFaceType face, bool restore)
    {
        switch (face)
        {
            case CubeFaceType.Top:
                RotateWholeCube(restore ? false : true, CubeAxisType.X);
                break;
            case CubeFaceType.Bottom:
                RotateWholeCube(restore ? true : false, CubeAxisType.X);
                break;
            case CubeFaceType.Back:
                RotateWholeCube(restore ? true : false, CubeAxisType.Y);
                RotateWholeCube(restore ? true : false, CubeAxisType.Y);
                break;
            case CubeFaceType.Left:
                RotateWholeCube(restore ? false : true, CubeAxisType.Z);
                break;
            case CubeFaceType.Right:
                RotateWholeCube(restore ? true : false, CubeAxisType.Z);
                break;
        }
    }
    // ✅ 내부 구현 - 레이어 데이터 적용
    private void InsertRotatedLayer(int layer, Cubie[,] slice, CubeAxisType axis)
    {
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
            {
                switch (axis)
                {
                    case CubeAxisType.X:
                        cubieGrid[layer, i, j] = slice[i, j];
                        break;
                    case CubeAxisType.Y:
                        cubieGrid[i, layer, j] = slice[i, j];
                        break;
                    case CubeAxisType.Z:
                        cubieGrid[i, j, layer] = slice[i, j];
                        break;
                }
            }
    }
    // ✅ 내부 구현 - 이름 정리
    private void RenameCubies()
    {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                for (int z = 0; z < Size; z++)
                    if (cubieGrid[x, y, z] != null)
                        cubieGrid[x, y, z].name = $"Cubie_{x}_{y}_{z}";
    }

    // ✅ 내부 구현 - 복사본 생성
    private Cubie[,,] GetGridCopy() => (Cubie[,,])cubieGrid.Clone();
}
