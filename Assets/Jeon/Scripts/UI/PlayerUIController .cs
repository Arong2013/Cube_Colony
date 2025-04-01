using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PlayerUIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Settings")]
    [SerializeField] private float clickHoldTime = 1f;

    [Header("Dependencies")]
    [SerializeField] private CubeRotaterUI cubeRotaterUI = new CubeRotaterUI();

    private Action<Cubie, CubeAxisType, bool> rotateAction;

    private CubieFace currentSelectedCubieFace;
    private Vector2 initialMousePosition;

    private float clickHoldTimer;
    private bool isClickHold;
    private bool isHoldMode;

    private void Update()
    {
        if (isClickHold)
        {
            clickHoldTimer += Time.deltaTime;

            if (clickHoldTimer > clickHoldTime && currentSelectedCubieFace)
            {
                isHoldMode = true;
                cubeRotaterUI.SetUp(RotateCubeEvent, currentSelectedCubieFace, initialMousePosition);
            }
        }
        else if (clickHoldTimer > 0.1f && currentSelectedCubieFace)
        {
            isHoldMode = false;
        }
    }

    public void SetRotateAction(Action<Cubie, CubeAxisType, bool> rotateCube)
    {
        rotateAction = rotateCube;
    }

    public void RotateCubeEvent(CubeAxisType axis, bool isClock)
    {
        rotateAction?.Invoke(currentSelectedCubieFace.cubie, axis, isClock);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ResetClickState();
        DetectSelectedCubieFace(eventData);
        initialMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isHoldMode)
        {
            cubeRotaterUI.OndDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClickHold = false;

        if (isHoldMode)
        {
            cubeRotaterUI.OnPointerUP();
            isHoldMode = false;
        }
    }

    private void ResetClickState()
    {
        clickHoldTimer = 0;
        isClickHold = true;
        isHoldMode = false;
        currentSelectedCubieFace = null;
    }

    private void DetectSelectedCubieFace(PointerEventData eventData)
    {
        currentSelectedCubieFace = Utils.DetectSelectedObject<CubieFace>(eventData);
    }
}
