using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
[System.Serializable]
public class CubeRotaterUI 
{
    [SerializeField] float threshold = 2f;
    [SerializeField] RotaterSelectUI rotaterSelectUI = new RotaterSelectUI();

    private CubieFace cubieFace;
    private Action<CubeAxisType, bool> rotateAction;
    private Vector2 initialMousePosition;
    private CubeAxisType cantAxis,compomAxis;
    private bool isClockwise;
    private bool canAction;

    public void SetUp(Action<CubeAxisType, bool> _rotateAction, CubieFace _cubieFace, Vector2 _initialMousePosition)
    {
        rotateAction = _rotateAction;
        initialMousePosition = _initialMousePosition;
        cubieFace = _cubieFace;
        cantAxis = _cubieFace.GetNotRotationAxis();
    }
    public void OndDrag(PointerEventData eventData)
    {
        Vector2 dragVector = eventData.position - initialMousePosition;
        float dragMagnitude = Mathf.Abs(dragVector.x) + Mathf.Abs(dragVector.y);
        if(dragMagnitude > threshold)
        {
            TestAxis(dragVector);
            DetectRotation(dragVector);
            rotaterSelectUI.EnableUI(cubieFace,compomAxis,isClockwise);    
            canAction = true;   
        }
        else
        {
            canAction = false;
        }
    }
    public void OnPointerUP()
    {
        if(canAction)
        {
            rotateAction?.Invoke(compomAxis, isClockwise);
            canAction = false;
        }
    }

    private void TestAxis(Vector2 dragVector)
    {
        switch (cantAxis)
        {
            case CubeAxisType.Z:
                compomAxis = Mathf.Abs(dragVector.x) < Mathf.Abs(dragVector.y) ? CubeAxisType.X : CubeAxisType.Y;
                break;
            case CubeAxisType.Y:
                compomAxis = dragVector.x > 0 && dragVector.y > 0 || dragVector.x < 0 && dragVector.y < 0 == true ? CubeAxisType.Z : CubeAxisType.X;
                break;
            case CubeAxisType.X:
                compomAxis = Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y) ? CubeAxisType.Y : CubeAxisType.Z;
                break;
        }
    }

    private void DetectRotation(Vector2 dragVector)
    {
       switch(compomAxis)
        {
            case CubeAxisType.Z:
                isClockwise = dragVector.y < 0;
                break;
            case CubeAxisType.X:
                isClockwise = dragVector.y > 0;
                break;
            case CubeAxisType.Y:
                isClockwise = dragVector.x < 0;
                break;

        }

    }
}