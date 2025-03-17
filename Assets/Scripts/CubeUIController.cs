using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CubeUIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float dragThreshold = 30f;

    private const float maxRotation = 90f;
    private Vector2 initialMousePosition;
    private bool isAxisConfirmed = false;
    private bool isAxisLocked = false;

    private Cubie selectedCubie;
    private CubeAxisType selectedAxis;
    private float accumulatedRotation = 0f;

    private Action<Cubie, CubeAxisType, float, RotateType> rotateCube;

    public void SetRotateCubeUpdate(Action<Cubie, CubeAxisType, float, RotateType> rotateCube)
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

        bool isRotateConfirmed = Mathf.Abs(accumulatedRotation) >= 45f;
        int finalRotation;
        RotateType rotateType;

        if (isRotateConfirmed)
        {
            finalRotation = (int)(Mathf.Sign(accumulatedRotation) * (90 - Mathf.Abs(accumulatedRotation)));
            rotateType = RotateType.Confirmed;
        }
        else
        {
            finalRotation = (int)(-accumulatedRotation);
            rotateType = RotateType.Zero;
        }

        rotateCube?.Invoke(selectedCubie, selectedAxis, finalRotation, rotateType);
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
        float absX = Mathf.Abs(dragVector.x);
        float absY = Mathf.Abs(dragVector.y);

        if (absX > absY)
        {
            selectedAxis = CubeAxisType.Y; // 가로 움직임이 크면 Y축 회전
        }
        else
        {
            selectedAxis = CubeAxisType.X; // 세로 움직임이 크면 X축 회전
        }
    }

    private void DetectRotation(Vector2 dragVector)
    {
        float rotationAmount = (selectedAxis == CubeAxisType.Y) ? -dragVector.x : dragVector.y;
        accumulatedRotation += Mathf.Clamp(rotationAmount * Time.deltaTime, -maxRotation, maxRotation);
        rotateCube?.Invoke(selectedCubie, selectedAxis, rotationAmount * Time.deltaTime, RotateType.Normal);
    }

    private void DetectSelectedCubie(PointerEventData eventData)
    {
        if (Camera.main == null) return; // Camera.main이 없을 경우 예외 방지

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (var hit in hits)
        {
            Cubie cubie = hit.collider.GetComponent<Cubie>();
            if (cubie != null)
            {
                selectedCubie = cubie;
                break;
            }
        }
    }
}
