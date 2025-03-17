using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;

public enum CubeAxisType
{
    X,
    Y,
    Z
}
public enum RotateType
{
    Normal,
    Confirmed,
    Zero
}
public class Cube : MonoBehaviour
{
    [SerializeField] CubeUIController cubeUIController;
    [SerializeField] int size;
    [SerializeField] Cubie cubie;

    CubeRotater cubeRotater;
    CubeGridHandler cubeGridHandler;

    private void Start()
    {
        cubeRotater = new CubeRotater();
        cubeGridHandler = new CubeGridHandler(size, cubie);

        cubeUIController.SetRotateCubeUpdate(RotateCube);
    }

    private void RotateCube(Cubie selectedCubie, CubeAxisType axis, float rotationAmount, RotateType rotateType)
    {
        if (cubeRotater.IsRotating) return;
        int layer = cubeGridHandler.FindLayer(selectedCubie, axis);
        List<Cubie> cubies = cubeGridHandler.GetCubiesInLayer(layer, axis);
        switch(rotateType)
        {
            case RotateType.Normal:
                cubeRotater.RotateCubesUpdate(cubies, axis, rotationAmount);
                break;
            case RotateType.Confirmed:
                StartCoroutine(cubeRotater.RotateCubesSmooth(cubies, axis,rotationAmount));
                cubeGridHandler.RotateLayer(layer, rotationAmount > 0, axis);
                break;
           case RotateType.Zero:
                StartCoroutine(cubeRotater.RotateCubesSmooth(cubies, axis, rotationAmount));
                break;
        }
    }
}
