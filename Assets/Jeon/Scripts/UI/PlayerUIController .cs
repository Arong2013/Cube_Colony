using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class PlayerUIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float clickHoldTime = 1f;

    private float currrentClickHold;
    private bool isClickHold;

    [SerializeField] TowerSpawnControllerUI towerSpawnControllerUI = new TowerSpawnControllerUI();
    [SerializeField] RotaterSelectUI selectUI;

    private CubieFace curselectedCubieFace;
    public bool IsEnableSelectUI => curselectedCubieFace && currrentClickHold > clickHoldTime && isClickHold;
    public bool IsEnableTowerSpawnUI => curselectedCubieFace && currrentClickHold < clickHoldTime && !isClickHold;

    public void Update()
    {
        ClickEvent();
        if(IsEnableSelectUI)
            EnableSelectUI();
    }

    //public void SetRotateCubeUpdate(Action<Cubie, CubeAxisType, int> rotateCube) => cubeUIController.SetRotateCubeUpdate(rotateCube);

    public void Init()
    {
        curselectedCubieFace = null;
        currrentClickHold = 0;
        isClickHold = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Init();
        RayCubieFace(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsEnableTowerSpawnUI)
            EnableTowerSpawnUI();
    }

    private void RayCubieFace(PointerEventData eventData)
    {
        var selectedCubieFace = Utils.DetectSelectedObject<CubieFace>(eventData);
        curselectedCubieFace = selectedCubieFace;
    }
    private void EnableSelectUI()
    {
        isClickHold = false;
        selectUI.EnableUI(curselectedCubieFace);
    }

    private void EnableTowerSpawnUI()
    {
        isClickHold = false;
        towerSpawnControllerUI.EnableUI(curselectedCubieFace);
    }

    public void ClickEvent() { if (isClickHold) currrentClickHold += Time.deltaTime; }
}
