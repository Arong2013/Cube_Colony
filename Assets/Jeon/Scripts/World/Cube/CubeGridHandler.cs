﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class CubeGridHandler 
{
    public static CubeGridHandler Instance { get; private set; }
    public readonly int Size;
    private Cubie[,,] cubieGrid;
    public Cubie[,,] GridSnapshot => GetGridCopy();
    public CubeGridHandler(Cubie cubiePrefab, Transform parent,CubeData cubeData)
    {
        if (Instance != null)
            Debug.LogWarning("CubeGridHandler instance already exists. Overwriting.");

        Instance = this; // ✅ 싱글톤 등록

        Size = cubeData.size;
        cubieGrid = new Cubie[Size, Size, Size];

        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                for (int z = 0; z < Size; z++)
                {
                    Vector3 position = new Vector3(x - 1, y - 1, z - 1);
                    Vector3 worldPos = (new Vector3(x - 1, y - 1, z - 1)) * 10f;
                    cubieGrid[x, y, z] = GameObject.Instantiate(cubiePrefab.gameObject, worldPos, Quaternion.identity, parent).GetComponent<Cubie>();
                    cubieGrid[x, y, z].name = $"Cubie_{x}_{y}_{z}";
                    cubieGrid[x, y, z].Init();
                }

        AssignCubieFaceSkills(cubeData.skillProbabilities);
    }
    public void RotateSingleLayer(Cubie referenceCubie, CubeAxisType axis, bool isClock)
    {
        int layerIndex = GridSearchHelper.FindLayer(referenceCubie, axis, GetGridCopy());
        ApplyLayerRotation(layerIndex, isClock, axis);
    }
    public void RotateWholeCube(CubeAxisType axis,bool clockwise)
    {
        for (int layer = 0; layer < Size; layer++)
            ApplyLayerRotation(layer, clockwise, axis,true);
    }
    public List<Cubie> GetAllCubies() => GridSearchHelper.GetAllCubies(GetGridCopy());

    public List<CubieFaceInfo> GetCubieFaces(CubeFaceType cubeFaceType)
    {
        var grid = GetGridCopy();

        return GridSearchHelper
            .GetCubieFaces(cubeFaceType, grid)
            .Where(f => f != null && f.FaceInfo != null)
            .Select(f => {
                f.FaceInfo.SetPosition(GridSearchHelper.GetCubieGridPosition(f.cubie, grid));
                return f.FaceInfo;
            })
            .ToList();
    }
    public List<Cubie> GetCubiesOnSameLayer(Cubie referenceCubie, CubeAxisType axis)
    {
        int layer = GridSearchHelper.FindLayer(referenceCubie, axis, GetGridCopy());
        return GridSearchHelper.GetCubiesInLayer(layer, axis, GetGridCopy());
    }

    public Vector3 GetCubieGridPosition(Cubie cubie) => GridSearchHelper.GetCubieGridPosition(cubie, GetGridCopy());    
    public void DestroyAllCubies()
    {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                for (int z = 0; z < Size; z++)
                {
                    if (cubieGrid[x, y, z] != null)
                    {
                        GameObject.Destroy(cubieGrid[x, y, z].gameObject);
                        cubieGrid[x, y, z] = null;
                    }
                }
    }
    public int GetSameTypeAdjacentCount(CubieFace cubieFace) => GridSearchHelper.GetConnectedSameSkillCount(cubieFace, GetGridCopy());    
    private void ApplyLayerRotation(int layer, bool isClockwise, CubeAxisType axis)
    {
        Cubie[,] layerSlice = CubieMatrixHelper.ExtractLayer(GetGridCopy(), layer, axis);
        RotateCubies(layerSlice, isClockwise, axis);
        Cubie[,] rotated = CubieMatrixHelper.RotateMatrix(layerSlice, isClockwise, axis);
        InsertRotatedLayer(layer, rotated, axis);
        RenameCubies();
        UpDateCubieVisual(false);
    }

        private void ApplyLayerRotation(int layer, bool isClockwise, CubeAxisType axis,bool isAllcube)
    {
        Cubie[,] layerSlice = CubieMatrixHelper.ExtractLayer(GetGridCopy(), layer, axis);
        RotateCubies(layerSlice, isClockwise, axis);
        Cubie[,] rotated = CubieMatrixHelper.RotateMatrix(layerSlice, isClockwise, axis);
        InsertRotatedLayer(layer, rotated, axis);
        RenameCubies();
        UpDateCubieVisual(isAllcube);
    }
    private void AssignCubieFaceSkills(Dictionary<CubieFaceSkillType, float> skillProbabilities)
    {
        var faceMap = GridSearchHelper.GetCubieFaceMapByType(GetGridCopy());
        System.Random rand = new();

        foreach (var faceList in faceMap.Values)
        {
            foreach (var face in faceList)
            {
                var skill = GridSearchHelper.GetRandomSkill(skillProbabilities, rand);
                face.SetSkillType(skill); // <- 아래에 SetSkillType 메서드 추가 필요
            }
        }
        foreach (var cubie in cubieGrid)
        {
            cubie.UpdateCubieVisual(false);
        }
    }
    public void RotateCubies(Cubie[,] cubies, bool isClockwise, CubeAxisType axis)
    {
        foreach (var cubie in cubies)
        {
            cubie.RotateCubie(axis, isClockwise);
        }
    }
    public void UpDateCubieVisual(bool isAllCubie)
    {
        foreach (var cubie in cubieGrid)
        {
            cubie.UpdateCubieVisual(isAllCubie);
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
