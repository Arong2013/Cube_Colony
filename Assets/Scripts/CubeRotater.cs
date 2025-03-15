using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using UnityEngine;

public class CubeRotater
{
    readonly int size;
    bool isRotating;
    GameObject[,,] cubes;
    GameObject Testpice;
    public CubeRotater(Cube cube)
    {
        this.size = cube.Size;
        cubes = new GameObject[size, size, size]; // 큐브 오브젝트 배열
        Testpice = cube.Pice;
    }
    public void GenerateCube()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    GameObject cube = MonoBehaviour.Instantiate(Testpice, new Vector3(x - 1, y - 1, z - 1), Quaternion.identity);
                    cubes[x, y, z] = cube;
                }
            }
        }
    }


    IEnumerator UpdateCubeRotation(int layer, string axis)
    {
        isRotating = true;
        Vector3 rotationAxis = (axis == "x") ? Vector3.right : (axis == "y") ? Vector3.up : Vector3.forward;
        float duration = 0.5f;
        float elapsed = 0f;
        float angle = 90f;

        List<GameObject> rotatingCubes = new List<GameObject>();
        foreach (GameObject cube in cubes)
        {
            if ((axis == "x" && Mathf.RoundToInt(cube.transform.position.x) == layer) ||
                (axis == "y" && Mathf.RoundToInt(cube.transform.position.y) == layer) ||
                (axis == "z" && Mathf.RoundToInt(cube.transform.position.z) == layer))
            {
                rotatingCubes.Add(cube);
            }
        }

        while (elapsed < duration)
        {
            float step = (angle / duration) * Time.deltaTime;
            foreach (GameObject cube in rotatingCubes)
            {
                cube.transform.RotateAround(Vector3.zero, rotationAxis, step);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (GameObject cube in rotatingCubes)
        {
            cube.transform.RotateAround(Vector3.zero, rotationAxis, angle - (elapsed * (angle / duration))); // 보정
        }

        isRotating = false;
    }
}