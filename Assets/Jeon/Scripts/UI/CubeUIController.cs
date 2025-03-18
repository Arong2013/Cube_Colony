using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DualAxisType
{
    XY,      // X, Y축 회전 가능
    XZ,      // X, Z축 회전 가능
    YZ       // Y, Z축 회전 가능
}


[System.Serializable]
public class CubeUIController : IPointerUpHandler, IDragHandler
{
    public float dragThreshold = 30f;

    private Vector2 initialMousePosition;
    private bool isAxisConfirmed = false;
    private bool isAxisLocked = false;

    private CubieFace selectedCubie;
    private CubeAxisType singleAxis;
    private DualAxisType dualAxis;  
    private float accumulatedRotation = 0f;

    private Action<Cubie, CubeAxisType, int> rotateCube;

    public void SetRotateCubeUpdate(Action<Cubie, CubeAxisType, int> rotateCube)
    {
        this.rotateCube = rotateCube;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        selectedCubie = null;
        initialMousePosition = eventData.position;
        isAxisConfirmed = false;
        isAxisLocked = false;
        accumulatedRotation = 0f;
        DetectSelectedCubie(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectedCubie == null) return;

        bool isRotateConfirmed = Mathf.Abs(accumulatedRotation) >= dragThreshold;
        int finalRotation;

        if (isRotateConfirmed)
        {
            finalRotation = (int)(Mathf.Sign(accumulatedRotation) * 90);
            rotateCube?.Invoke(selectedCubie.cubie, singleAxis, finalRotation);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (selectedCubie == null) return;

        Vector2 dragVector = eventData.position - initialMousePosition;

        if (!isAxisLocked)
        {
            DetectAxis(dragVector);
            isAxisLocked = true;
        }

        DetectRotation(dragVector);

        
    }

    private void DetectAxis(Vector2 dragVector)
    {
        if (selectedCubie == null) return;

        // 선택된 면에서 가능한 회전 축을 가져옴
        DualAxisType allowedAxis = selectedCubie.GetAllowedRotationAxis();
        Debug.Log(allowedAxis + selectedCubie.face.ToString());

        // 회전 가능한 축에 맞게 축을 결정
        if (allowedAxis == DualAxisType.XY)
        {
            // XY축 회전만 가능
            if (Mathf.Abs(dragVector.x) < Mathf.Abs(dragVector.y))
            {
                singleAxis = CubeAxisType.X;  // X축 기준 회전
            }
            else
            {
                singleAxis = CubeAxisType.Y;  // Y축 기준 회전
            }
        }
        else if (allowedAxis == DualAxisType.XZ)
        {
            // XZ축 회전만 가능
            if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
            {
                singleAxis = CubeAxisType.X;  // X축 기준 회전
            }
            else
            {
                singleAxis = CubeAxisType.Z;  // Z축 기준 회전
            }
        }
        else if (allowedAxis == DualAxisType.YZ)
        {
            // YZ축 회전만 가능
            if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
            {
                singleAxis = CubeAxisType.Y;  // Y축 기준 회전
            }
            else
            {
                singleAxis = CubeAxisType.Z;  // Z축 기준 회전
            }
        }
    }


    private void DetectRotation(Vector2 dragVector)
    {
        if (dualAxis == DualAxisType.XY)
        {
            accumulatedRotation += dragVector.x * Time.deltaTime;  // X축
        }
        else if (dualAxis == DualAxisType.XZ)
        {
            accumulatedRotation += -dragVector.y * Time.deltaTime;  // Z축
        }
        else if (dualAxis == DualAxisType.YZ)
        {
            accumulatedRotation += -dragVector.x * Time.deltaTime;  // Y축
        }
    }



    public void DetectSelectedCubie(PointerEventData eventData)
    {
        if (Camera.main == null) return; // Camera.main이 없을 경우 예외 방지

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (var hit in hits)
        {
            CubieFace cubie = hit.collider.GetComponent<CubieFace>();
            if (cubie != null)
            {
                selectedCubie = cubie;
                break;
            }
        }
    }
}
