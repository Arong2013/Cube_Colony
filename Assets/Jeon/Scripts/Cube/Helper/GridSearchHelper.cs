﻿using System.Collections.Generic;
using System.Linq;

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
}
