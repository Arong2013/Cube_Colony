using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CubeUIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float dragThreshold = 30f;

    private Vector2 initialMousePosition;
    private bool isAxisConfirmed = false;
    private bool isAxisLocked = false;

    private Cubie selectedCubie;
    private CubeAxisType selectedAxis;
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
            rotateCube?.Invoke(selectedCubie, selectedAxis, finalRotation);
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
        float absX = Mathf.Abs(dragVector.x);
        float absY = Mathf.Abs(dragVector.y);

        if (absX > absY)
        {
            selectedAxis = CubeAxisType.Y; 
        }
        else
        {
            selectedAxis = CubeAxisType.X; 
        }
    }

    private void DetectRotation(Vector2 dragVector)
    {
        float rotationAmount = (selectedAxis == CubeAxisType.Y) ? -dragVector.x : dragVector.y;
        accumulatedRotation += rotationAmount * Time.deltaTime;
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
