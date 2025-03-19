using System;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class CubeUIController 
{
    public float dragThreshold;
    private Vector2 initialMousePosition;
    private bool isAxisLocked = false;
    private CubieFace selectedCubie;
    private CubeAxisType singleAxis;
    private float accumulatedRotation = 0f;
    private Action<Cubie, CubeAxisType, int> rotateCube;

    [SerializeField] Transform parent;

    public void SetRotateCubeUpdate(Action<Cubie, CubeAxisType, int> rotateCube)
    {
        this.rotateCube = rotateCube;
    }

    public void OnPointerDown(PointerEventData eventData,CubieFace cubieFace)
    {
        selectedCubie = null;
        initialMousePosition = eventData.position;
        isAxisLocked = false;
        accumulatedRotation = 0f;
        selectedCubie = cubieFace;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectedCubie == null) return;

        if (Mathf.Abs(accumulatedRotation) >= dragThreshold)
        {
            int finalRotation = (int)(Mathf.Sign(accumulatedRotation) * 90);
            rotateCube?.Invoke(selectedCubie.cubie, singleAxis, finalRotation);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (selectedCubie == null) return;

        Vector2 dragVector = eventData.position - initialMousePosition;

        DetectAxis(dragVector);
        DetectRotation(dragVector);
    }

    private void DetectAxis(Vector2 dragVector)
    {
        if (selectedCubie == null) return;

        CubeAxisType cantAxis = selectedCubie.GetNotRotationAxis();

        switch (cantAxis)
        {
            case CubeAxisType.Z:
                singleAxis = Mathf.Abs(dragVector.x) < Mathf.Abs(dragVector.y) ? CubeAxisType.X : CubeAxisType.Y;
                break;
            case CubeAxisType.Y:
                float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
                singleAxis = (angle >= 0 && angle < 90) || (angle >= 180 && angle < 270) ? CubeAxisType.Z : CubeAxisType.X;
                Debug.Log(angle);
                break;
            case CubeAxisType.X:
                singleAxis = Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y) ? CubeAxisType.Y : CubeAxisType.Z;
                break;
        }

        Debug.Log($"{singleAxis} {selectedCubie.face} {selectedCubie.cubie.name}");
    }

    private void DetectRotation(Vector2 dragVector)
    {
        Quaternion parentRotation = parent.transform.parent.rotation;

        float rotationDirectionX = Mathf.Sign(parentRotation.eulerAngles.x);
        float rotationDirectionY = Mathf.Sign(parentRotation.eulerAngles.y);
        float rotationDirectionZ = Mathf.Sign(parentRotation.eulerAngles.z);

        accumulatedRotation += singleAxis switch
        {
            CubeAxisType.X => dragVector.y * rotationDirectionX * Time.deltaTime,
            CubeAxisType.Y => -dragVector.x * rotationDirectionY * Time.deltaTime,
            CubeAxisType.Z => -dragVector.y * rotationDirectionZ * Time.deltaTime,
            _ => 0
        };
    }
}