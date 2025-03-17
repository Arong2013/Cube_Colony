using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeGridHandler
{
    public readonly int size;
    private Cubie[,,] cubieGrid;

    public CubeGridHandler(int _size,Cubie cubie)
    {
        size = _size;
        cubieGrid = new Cubie[size, size, size];

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                for (int z = 0; z < size; z++)
                {
                    Vector3 position = new Vector3(x - 1, y - 1, z - 1);
                    cubieGrid[x, y, z] =  UnityEngine.Object.Instantiate(cubie, position, Quaternion.identity);
                    cubieGrid[x, y, z].name = $"Cubie_{x}_{y}_{z}";
                }
    }

    public void RotateLayer(int layer, bool isClockwise, CubeAxisType axis)
    {
        Cubie[,] face = ExtractLayer(layer, axis);
        RotateCubies(face, isClockwise, axis);
        Cubie[,] rotatedFace = RotateMatrix(face, isClockwise,axis);
        InsertLayer(layer, rotatedFace, axis);
        UpdateCubieNames();
    }
    public int FindLayer(Cubie cubie, CubeAxisType axis)
    {
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                for (int z = 0; z < size; z++)
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
        return -1;
    }

    public List<Cubie> GetCubiesInLayer(int layer, CubeAxisType axis)
    {
        List<Cubie> cubies = new List<Cubie>();

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (axis == CubeAxisType.X)
                {
                    cubies.Add(cubieGrid[layer, i, j]);
                }
                else if (axis == CubeAxisType.Y)
                {
                    cubies.Add(cubieGrid[i, layer, j]);
                }
                else if (axis == CubeAxisType.Z)
                {
                    cubies.Add(cubieGrid[i, j, layer]);
                }
            }
        }
        return cubies;
    }

    private Cubie[,] ExtractLayer(int layer, CubeAxisType axis)
    {
        var face = new Cubie[size, size];

        Enumerable.Range(0, size).ToList().ForEach(i =>
            Enumerable.Range(0, size).ToList().ForEach(j =>
            {
                if (axis == CubeAxisType.X)
                {
                    face[i, j] = cubieGrid[layer, i, j];
                }
                else if (axis == CubeAxisType.Y)
                {
                    face[i, j] = cubieGrid[i, layer, j];
                }
                else
                {
                    face[i, j] = cubieGrid[i, j, layer];
                }
            })
        );

        return face;
    }

    private void InsertLayer(int layer, Cubie[,] face, CubeAxisType axis)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (axis == CubeAxisType.X)
                {
                    cubieGrid[layer, i, j] = face[i, j];
                }
                else if (axis == CubeAxisType.Y)
                {
                    cubieGrid[i, layer, j] = face[i, j];
                }
                else if (axis == CubeAxisType.Z)
                {
                    cubieGrid[i, j, layer] = face[i, j];

                }
            }
        }
    }

    private Cubie[,] RotateMatrix(Cubie[,] matrix, bool isClockwise, CubeAxisType axis)
    {
        var rotated = new Cubie[size, size];

        if (axis == CubeAxisType.Y) // Y축 회전 (XZ 평면)
        {
            Enumerable.Range(0, size).ToList().ForEach(i =>
                Enumerable.Range(0, size).ToList().ForEach(j =>
                {
                    if (isClockwise)
                    {
                        rotated[j, size - 1 - i] = matrix[i, j];
                    }
                    else
                    {
                        rotated[size - 1 - j, i] = matrix[i, j];
                    }
                })
            );
        }
        else if (axis == CubeAxisType.X) // X축 회전 (YZ 평면)
        {
            Enumerable.Range(0, size).ToList().ForEach(i =>
                Enumerable.Range(0, size).ToList().ForEach(j =>
                {
                    if (isClockwise)
                    {
                        rotated[size - 1 - j, i] = matrix[i, j];
                    }
                    else
                    {
                        rotated[j, size - 1 - i] = matrix[i, j];
                    }
                })
            );
        }
        else if (axis == CubeAxisType.Z) // Z축 회전 (XY 평면, 기존 방식)
        {
            Enumerable.Range(0, size).ToList().ForEach(i =>
                Enumerable.Range(0, size).ToList().ForEach(j =>
                {
                    if (isClockwise)
                    {
                        rotated[size - 1 - j, i] = matrix[i, j];
                    }
                    else
                    {
                        rotated[j, size - 1 - i] = matrix[i, j];
                    }
                })
            );
        }

        return rotated;
    }


    private void UpdateCubieNames()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    if (cubieGrid[x, y, z] != null)
                    {
                        cubieGrid[x, y, z].name = $"Cubie_{x}_{y}_{z}";
                    }
                }
            }
        }
    }

    private void RotateCubies(Cubie[,] cubies, bool isClockwise, CubeAxisType axis)
    {
        cubies.Cast<Cubie>().ToList().ForEach(cubie => cubie.RotateCubie(axis, isClockwise));
    }
}