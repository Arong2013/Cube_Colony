using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class CubeRotater
{
    private bool isRotating;
    public bool IsRotating => isRotating;

    Transform cubeParent; // 부모 Transform을 참조로 갖고 있어야 합니다.

    public CubeRotater(Transform parent)
    {
        cubeParent = parent;
    }

    public IEnumerator RotateCubesSmooth(List<Cubie> cubes, CubeAxisType axis, int rotationAmount)
    {
        if (isRotating) yield break;
        isRotating = true;

        Vector3 rotationAxis = GetRotationAxis(axis);

        Vector3 center = GetCubesCenter(cubes);

        float duration = 0.5f;
        float elapsed = 0f;
        float angle = rotationAmount;

        while (elapsed < duration)
        {
            float step = (angle / duration) * Time.deltaTime;
            cubes.ForEach(cube => cube.transform.RotateAround(center, rotationAxis, step));
            elapsed += Time.deltaTime;
            yield return null;
        }

        float correction = angle - (elapsed * (angle / duration));
        cubes.ForEach(cube => cube.transform.RotateAround(center, rotationAxis, correction));

        isRotating = false;
    }
    private Vector3 GetRotationAxis(CubeAxisType axis)
    {
        return axis switch
        {
            CubeAxisType.X => cubeParent.right,
            CubeAxisType.Y => cubeParent.up,
            CubeAxisType.Z => cubeParent.forward,
            _ => throw new ArgumentOutOfRangeException(nameof(axis), $"Unexpected axis value: {axis}")
        };
    }
    private Vector3 GetCubesCenter(List<Cubie> cubes)
    {
        Vector3 sum = Vector3.zero;
        foreach (var cube in cubes)
        {
            sum += cube.transform.position;
        }
        return sum / cubes.Count; // 평균 위치가 중심이 됨
    }
}
