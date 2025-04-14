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

    public Action onRotateAction;
    public void Init(CubeData cubeData, Action action)
    {
        cubeRotater = new CubeRotater(this.transform);
        cubeGridHandler = new CubeGridHandler(cubie,this.transform, cubeData);
        cubeUIController.gameObject.SetActive(true);    
        cubeUIController.SetRotateAction(RotateCube);
        cubeController = new CubeKeybordController(RotateAllCube);
        onRotateAction = action;    
    }
    public List<CubieFaceInfo> GetTopCubieFace() => cubeGridHandler.GetCubieFaces(CubeFaceType.Top);        
    private void Update()
    {
        if (cubeRotater.IsRotating) return;
        cubeController?.RotateCube();
    }
    private void  RotateAllCube(CubeAxisType cubeAxisType,bool isClock)
    {
        StartCoroutine(RotateAllCubeCoroutine(cubeAxisType, isClock));

    }
    private void RotateCube(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        StartCoroutine(RotateCubeCoroutine(selectedCubie, axis, isClock));  
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
        onRotateAction?.Invoke();
    }
    public void RemoveCube()
    {
        cubeUIController.gameObject.SetActive(false);
        cubeGridHandler.DestroyAllCubies();

        cubeGridHandler = null;
        cubeController = null;
    }
}
