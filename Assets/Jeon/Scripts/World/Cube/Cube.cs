using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Cube : MonoBehaviour
{
    [SerializeField] Cubie cubie;

    CubeRotater cubeRotater;
    CubeGridHandler cubeGridHandler;
    ICubeController cubeController;

    public void Init(CubeData cubeData)
    {
        cubeRotater = new CubeRotater(this.transform);
        cubeGridHandler = new CubeGridHandler(cubie,this.transform, cubeData);

        cubeController = new CubeKeybordController(RotateAllCube);
    }
    public List<CubieFaceInfo> GetTopCubieFace() => cubeGridHandler.GetCubieFaces(CubeFaceType.Top);        
    private void Update()
    {
        if (cubeRotater.IsRotating) return;
        cubeController?.RotateCube();
    }

    public void RotateCube(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        StartCoroutine(RotateCubeCoroutine(selectedCubie, axis, isClock));
    }
    private void  RotateAllCube(CubeAxisType cubeAxisType,bool isClock)
    {
        StartCoroutine(RotateAllCubeCoroutine(cubeAxisType, isClock));

    }


    IEnumerator RotateAllCubeCoroutine(CubeAxisType cubeAxisType, bool isClock)
    {
        cubeGridHandler.RotateWholeCube(cubeAxisType, isClock);
        yield return  StartCoroutine(cubeRotater.RotateCubesSmooth(cubeGridHandler.GetAllCubies(), cubeAxisType, isClock));
    }

    IEnumerator RotateCubeCoroutine(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        List<Cubie> targetLayer = cubeGridHandler.GetCubiesOnSameLayer(selectedCubie, axis);
        cubeGridHandler.RotateSingleLayer(selectedCubie, axis, isClock);
        yield return StartCoroutine(cubeRotater.RotateCubesSmooth(targetLayer, axis, isClock));
    }
    public void RemoveCube()
    {
        cubeGridHandler.DestroyAllCubies();

        cubeGridHandler = null;
        cubeController = null;
    }
}
