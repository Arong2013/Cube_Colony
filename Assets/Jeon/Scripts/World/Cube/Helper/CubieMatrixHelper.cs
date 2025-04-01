using UnityEngine;
using System;

public static class CubieMatrixHelper
{
    public static Cubie[,] ExtractLayer(Cubie[,,] grid, int layer, CubeAxisType axis)
    {
        int size = grid.GetLength(0);
        var face = new Cubie[size, size];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                face[i, j] = axis switch
                {
                    CubeAxisType.X => grid[layer, i, j],
                    CubeAxisType.Y => grid[i, layer, j],
                    CubeAxisType.Z => grid[i, j, layer],
                    _ => null
                };
            }

        return face;
    }
    public static Cubie[,] RotateMatrix(Cubie[,] matrix, bool isClockwise, CubeAxisType axis)
    {
        int size = matrix.GetLength(0);
        var rotated = new Cubie[size, size];

        if (axis == CubeAxisType.Y) // Y�� ȸ�� (XZ ���)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    rotated[isClockwise ? j : size - 1 - j, isClockwise ? size - 1 - i : i] = matrix[i, j];
                }
        }
        else if (axis == CubeAxisType.X) // X�� ȸ�� (YZ ���)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    rotated[isClockwise ? size - 1 - j : j, isClockwise ? i : size - 1 - i] = matrix[i, j];
                }
        }
        else if (axis == CubeAxisType.Z) // Z�� ȸ�� (XY ���)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    rotated[isClockwise ? size - 1 - j : j, isClockwise ? i : size - 1 - i] = matrix[i, j];
                }
        }
        return rotated;
    }
}
