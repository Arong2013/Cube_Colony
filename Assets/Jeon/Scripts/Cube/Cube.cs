using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;

public enum CubeAxisType
{
    X,       // X축 회전만 가능
    Y,       // Y축 회전만 가능
    Z        // Z축 회전만 가능
}

public class Cube : MonoBehaviour
{
    [SerializeField] PlayerUIController cubeUIController;
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

    private void RotateCube(Cubie selectedCubie, CubeAxisType axis, int rotationAmount)
    {
        if (cubeRotater.IsRotating) return;
        int layer = cubeGridHandler.FindLayer(selectedCubie, axis);
        List<Cubie> cubies = cubeGridHandler.GetCubiesInLayer(layer, axis);
        StartCoroutine(cubeRotater.RotateCubesSmooth(cubies, axis, rotationAmount));
        cubeGridHandler.RotateLayer(layer, rotationAmount > 0, axis);
    }
}
