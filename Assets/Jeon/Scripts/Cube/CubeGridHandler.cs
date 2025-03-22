using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
[System.Serializable]

public class CubeGridHandler
{
    public readonly int size;
    private Cubie[,,] cubieGrid;
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
    }

    public List<CubieFace> GetAstarFaceList(CubieFace start,CubieFace target)
    {
        var faceType = start.face;  
        HandleCubeRotation(faceType, false);
        AStarPathfinding astar = new AStarPathfinding(BulidEmptyMap(CubeFaceType.Front), size);
        var list = astar.FindPath(start, target);
        HandleCubeRotation(faceType, true);
        return list;
    }
    private void HandleCubeRotation(CubeFaceType cubeFaceType, bool isRestore)
    {
        // 회전 처리: 큐브의 각 면에 따라 회전 수행
        switch (cubeFaceType)
        {
            case CubeFaceType.Top:
                // Top 면은 X축 기준으로 회전 (반대 방향)
                RotateEntireCube(isRestore ? false : true, CubeAxisType.X);
                break;
            case CubeFaceType.Bottom:
                // Bottom 면은 X축 기준으로 회전 (반대 방향)
                RotateEntireCube(isRestore ? true : false, CubeAxisType.X);
                break;
            case CubeFaceType.Front:
                // Front 면은 Y축 기준으로 회전 (반대 방향)
                RotateEntireCube(isRestore ? false : true, CubeAxisType.Y);
                break;
            case CubeFaceType.Back:
                // Back 면은 Y축 기준으로 회전 (반대 방향)
                RotateEntireCube(isRestore ? true : false, CubeAxisType.Y);
                break;
            case CubeFaceType.Left:
                // Left 면은 Z축 기준으로 회전 (반대 방향)
                RotateEntireCube(isRestore ? false : true, CubeAxisType.Z);
                break;
            case CubeFaceType.Right:
                // Right 면은 Z축 기준으로 회전 (반대 방향)
                RotateEntireCube(isRestore ? true : false, CubeAxisType.Z);
                break;
            default:
                break;
        }
    }
    void RotateLayer(int layer, bool isClockwise, CubeAxisType axis)
    {
        Cubie[,] face = CubieMatrixHelper.ExtractLayer(cubieGrid, layer, axis);
        CubieManipulator.RotateCubies(face, isClockwise, axis);
        var rotated = CubieMatrixHelper.RotateMatrix(face, isClockwise, axis);
        CubieMatrixHelper.InsertLayer(cubieGrid, layer, rotated, axis);
        UpdateCubieNames();
    }

    public void RotateLayer(Cubie selectedCubie, CubeAxisType axis, int rotationAmount)
    {
        int layer = GridSearchHelper.FindLayer(selectedCubie, axis, cubieGrid);
        bool isClockwise = rotationAmount > 0;
        RotateLayer(layer, isClockwise, axis);
        UpdateCubieNames();
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
    private Dictionary<Vector2Int, CubieFace> BulidEmptyMap(CubeFaceType cubeFaceType)
    {
        List<Vector2Int> positions = CubeMapHelper.BuildEmptyMap(size); 
        var allCubies = GridSearchHelper.GetAllCubies(cubieGrid); 
        Dictionary<CubeFaceType, List<CubieFace>> cubieFaceMap = CubeMapHelper.FillCubieFaceMap(allCubies, cubieGrid);
        CubeMapHelper.SortCubieFaceMap(cubieFaceMap, cubieGrid);
        var cubiedict = CubeMapHelper.BuildCubieFaceMap(cubieFaceMap, positions, cubeFaceType);
        foreach (var entry in cubiedict)
        {

            Debug.Log($"Position: {entry.Key} at position: {entry.Value.transform.position}", entry.Value.gameObject);
        }
        return cubiedict;
    }
    public List<Cubie> GetAllCubies()
    {
        return GridSearchHelper.GetAllCubies(cubieGrid);
    }
    public List<Cubie> GetCubiesInLayer(Cubie selectedCubie, CubeAxisType axis)
    {
        int layer = GridSearchHelper.FindLayer(selectedCubie, axis, cubieGrid);
        return GridSearchHelper.GetCubiesInLayer(layer, axis, cubieGrid);
    }
}