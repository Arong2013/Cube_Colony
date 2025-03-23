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
        bool clockwise = keyCode == KeyCode.W || keyCode == KeyCode.A;
        CubeAxisType axis = (keyCode == KeyCode.W || keyCode == KeyCode.S) ? CubeAxisType.X : CubeAxisType.Y;

        int angle = clockwise ? 90 : -90;

        StartCoroutine(cubeRotater.RotateCubesSmooth(cubeGridHandler.GetAllCubies(), axis, angle));
        cubeGridHandler.RotateWholeCube(clockwise, axis);
    }
    private void RotateCube(Cubie selectedCubie, CubeAxisType axis, int rotationAmount)
    {
        if (cubeRotater.IsRotating) return;
        List<Cubie> targetLayer = cubeGridHandler.GetCubiesOnSameLayer(selectedCubie, axis);
        StartCoroutine(cubeRotater.RotateCubesSmooth(targetLayer, axis, rotationAmount));
        cubeGridHandler.RotateSingleLayer(selectedCubie, axis, rotationAmount);
    }}
