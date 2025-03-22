using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;
using static UnityEditor.Experimental.GraphView.GraphView;

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
   public CubeGridHandler cubeGridHandler;

    private void Start()
    {
        cubeRotater = new CubeRotater(this.transform);
        cubeGridHandler = new CubeGridHandler(size, cubie,this.transform);

       cubeUIController.SetRotateAction(RotateCube);
    }
    private void Update()
    {
        if (cubeRotater.IsRotating) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            InputRotateCube(KeyCode.W);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            InputRotateCube(KeyCode.S);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            InputRotateCube(KeyCode.A);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            InputRotateCube(KeyCode.D);
        }
    }

    public void InputRotateCube(KeyCode keyCode)
    {
        switch(keyCode)
        {
            case KeyCode.W:
                StartCoroutine(cubeRotater.RotateCubesSmooth(cubeGridHandler.GetAllCubies(), CubeAxisType.X, 90));
                cubeGridHandler.RotateEntireCube(true, CubeAxisType.X);
                break;
            case KeyCode.S:
                StartCoroutine(cubeRotater.RotateCubesSmooth(cubeGridHandler.GetAllCubies(), CubeAxisType.X, -90));
                cubeGridHandler.RotateEntireCube(false, CubeAxisType.X);
                break;
            case KeyCode.A:
                StartCoroutine(cubeRotater.RotateCubesSmooth(cubeGridHandler.GetAllCubies(), CubeAxisType.Y, 90));
                cubeGridHandler.RotateEntireCube(true, CubeAxisType.Y);
                break;
            case KeyCode.D:
                StartCoroutine(cubeRotater.RotateCubesSmooth(cubeGridHandler.GetAllCubies(), CubeAxisType.Y, -90));
                cubeGridHandler.RotateEntireCube(false, CubeAxisType.Y);
                break;
        }
    }

    private void RotateCube(Cubie selectedCubie, CubeAxisType axis, int rotationAmount)
    {
        if (cubeRotater.IsRotating) return;
        List<Cubie> cubies = cubeGridHandler.GetCubiesInLayer(selectedCubie, axis);
        StartCoroutine(cubeRotater.RotateCubesSmooth(cubies, axis, rotationAmount));
        cubeGridHandler.RotateLayer(selectedCubie, axis, rotationAmount);
    }


    public List<CubieFace> TestAstar(CubieFace start, CubieFace end) => cubeGridHandler.GetAstarFaceList(start, end);   
}
