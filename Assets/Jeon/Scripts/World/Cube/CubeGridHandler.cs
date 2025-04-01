using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;

[System.Serializable]
public class CubeGridHandler 
{
    public readonly int Size;
    private Cubie[,,] cubieGrid;
    public Cubie[,,] GridSnapshot => GetGridCopy();
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
                    cubieGrid[x, y, z].Init();
                }
    }
    public void RotateSingleLayer(Cubie referenceCubie, CubeAxisType axis, bool isClock)
    {
        int layerIndex = GridSearchHelper.FindLayer(referenceCubie, axis, GetGridCopy());
        ApplyLayerRotation(layerIndex, isClock, axis);
    }
    public void RotateWholeCube(CubeAxisType axis,bool clockwise)
    {
        for (int layer = 0; layer < Size; layer++)
            ApplyLayerRotation(layer, clockwise, axis);
    }
    public List<Cubie> GetAllCubies() => GridSearchHelper.GetAllCubies(GetGridCopy());
    public List<Cubie> GetCubiesOnSameLayer(Cubie referenceCubie, CubeAxisType axis)
    {
        int layer = GridSearchHelper.FindLayer(referenceCubie, axis, GetGridCopy());
        return GridSearchHelper.GetCubiesInLayer(layer, axis, GetGridCopy());
    }
    public CubieFace GetCenterFace(CubeFaceType faceType) => GridSearchHelper.GetCenterFace(faceType, Size, GetGridCopy());
    public CubieFace GetCubieFaceInPos(CubeFaceType cubeFaceType, Vector3 pos) => GridSearchHelper.GetCubieFaceInPos(cubeFaceType,pos,GetGridCopy());
    private void ApplyLayerRotation(int layer, bool isClockwise, CubeAxisType axis)
    {
        Cubie[,] layerSlice = CubieMatrixHelper.ExtractLayer(GetGridCopy(), layer, axis);
        RotateCubies(layerSlice, isClockwise, axis);
        Cubie[,] rotated = CubieMatrixHelper.RotateMatrix(layerSlice, isClockwise, axis);
        InsertRotatedLayer(layer, rotated, axis);
        RenameCubies();
    }
    public void RotateCubies(Cubie[,] cubies, bool isClockwise, CubeAxisType axis)
    {
        foreach (var cubie in cubies)
        {
            cubie.RotateCubie(axis, isClockwise);
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
