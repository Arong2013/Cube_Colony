using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class CubeRotater
{
    private bool isRotating;
    public bool IsRotating => isRotating;
    public IEnumerator RotateCubesSmooth(List<Cubie> cubes, CubeAxisType axis, int rotationAmount)
    {
        if (isRotating) yield break;
        isRotating = true;

        Vector3 rotationAxis = GetRotationAxis(axis);
        float duration = 0.5f;
        float elapsed = 0f;
        int angle = rotationAmount;

        while (elapsed < duration)
        {
            float step = (angle / duration) * Time.deltaTime;
            cubes.ForEach(cube => cube.transform.RotateAround(Vector3.zero, rotationAxis, step));
            elapsed += Time.deltaTime;
            yield return null;
        }

        float correction = angle - (elapsed * (angle / duration));
        cubes.ForEach(cube => cube.transform.RotateAround(Vector3.zero, rotationAxis, correction));

        isRotating = false;
    }
    private Vector3 GetRotationAxis(CubeAxisType axis)
    {
        return axis switch
        {
            CubeAxisType.X => Vector3.right,
            CubeAxisType.Y => Vector3.up,
            CubeAxisType.Z => Vector3.forward,
            _ => throw new ArgumentOutOfRangeException(nameof(axis), $"Unexpected axis value: {axis}")
        };
    }
}