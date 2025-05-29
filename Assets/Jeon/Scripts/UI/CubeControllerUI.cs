using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CubeControllerUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Settings")]
    [SerializeField] private float clickHoldTime = 1f;

    [Header("Dependencies")]
    [SerializeField] private CubeRotaterUI cubeRotaterUI = new CubeRotaterUI();
    [Header("사운드 지연 시간")]
    [SerializeField] float soundDelay = 0.3f; // 소리 지연 시간 추가



    private Action<Cubie, CubeAxisType, bool> rotateAction;

    private CubieFace currentSelectedCubieFace;
    private Vector2 initialMousePosition;

    private float clickHoldTimer;
    private bool isClickHold;
    private bool isHoldMode;
    private bool hasSoundPlayed = false; // 소리 재생 여부를 추적하는 플래그 추가

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
            // 첫 드래그 시에만 소리 재생
            if (!hasSoundPlayed)
            {
                PlaySound();
                hasSoundPlayed = true;
            }
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

        // 포인터 업 시 소리 재생 플래그 초기화
        hasSoundPlayed = false;
    }

    private void ResetClickState()
    {
        clickHoldTimer = 0;
        isClickHold = true;
        isHoldMode = false;
        currentSelectedCubieFace = null;
        hasSoundPlayed = false; // 클릭 상태 초기화 시 소리 재생 플래그도 초기화

    }

    private void DetectSelectedCubieFace(PointerEventData eventData)
    {
        currentSelectedCubieFace = Utils.DetectSelectedObject<CubieFace>(eventData);
    }


    public void PlaySound()
    {
        SoundManager.Instance.PlaySFX("CubeTurn");
    }
}
