using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Cube : MonoBehaviour
{
    [SerializeField] PlayerUIController cubeUIController;
    [SerializeField] Cubie cubie;

    CubeRotater cubeRotater;
    CubeGridHandler cubeGridHandler;

    ICubeController cubeController;
    Func<bool> IsSurvivalCompleteState = null;  
    public void Init(int size)
    {
        cubeRotater = new CubeRotater(this.transform);
        cubeGridHandler = new CubeGridHandler(size, cubie,this.transform);
        cubeUIController.SetRotateAction(RotateCube);
        cubeController = new CubeKeybordController(RotateAllCube);
    }
    private void Update()
    {
        if (cubeRotater.IsRotating) return;
        cubeController.RotateCube();
    }
    private void  RotateAllCube(CubeAxisType cubeAxisType,bool isClock)
    {
        StartCoroutine(cubeRotater.RotateCubesSmooth(cubeGridHandler.GetAllCubies(), cubeAxisType, isClock));
        cubeGridHandler.RotateWholeCube(cubeAxisType, isClock);
    }
    private void RotateCube(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        List<Cubie> targetLayer = cubeGridHandler.GetCubiesOnSameLayer(selectedCubie, axis);
        StartCoroutine(cubeRotater.RotateCubesSmooth(targetLayer, axis, isClock));
        cubeGridHandler.RotateSingleLayer(selectedCubie, axis, isClock);
    }
 }
