using System;
using UnityEngine;

public class CubeKeybordController: ICubeController
{
    Action<CubeAxisType, bool> cubeRotateAction;
    public CubeKeybordController(Action<CubeAxisType, bool> action)
    {
        cubeRotateAction = action;
    }
    public void RotateCube()
    {
        InputKeybord();
    }
    private void InputKeybord()
    {
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
    private void InputRotateCube(KeyCode keyCode)
    {
        bool clockwise = keyCode == KeyCode.W || keyCode == KeyCode.A;
        CubeAxisType axis = (keyCode == KeyCode.W || keyCode == KeyCode.S) ? CubeAxisType.X : CubeAxisType.Y;
        cubeRotateAction(axis, clockwise);  
    }
}