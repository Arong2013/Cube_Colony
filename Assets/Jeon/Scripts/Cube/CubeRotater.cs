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
        Vector3 rotationCenter = cubeParent.position; // 부모의 위치를 중심으로 사용

        float duration = 0.5f;
        float elapsed = 0f;
        float angle = rotationAmount;

        while (elapsed < duration)
        {
            float step = (angle / duration) * Time.deltaTime;
            cubes.ForEach(cube => cube.transform.RotateAround(rotationCenter, rotationAxis, step));
            elapsed += Time.deltaTime;
            yield return null;
        }

        float correction = angle - (elapsed * (angle / duration));
        cubes.ForEach(cube => cube.transform.RotateAround(rotationCenter, rotationAxis, correction));

        isRotating = false;
    }

    private Vector3 GetRotationAxis(CubeAxisType axis)
    {
        // 부모 오브젝트의 로컬 축을 기준으로 축 결정
        return axis switch
        {
            CubeAxisType.X => cubeParent.right,
            CubeAxisType.Y => cubeParent.up,
            CubeAxisType.Z => cubeParent.forward,
            _ => throw new ArgumentOutOfRangeException(nameof(axis), $"Unexpected axis value: {axis}")
        };
    }
}
