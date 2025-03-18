using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class PlayerUIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] float clickHoldTime = 1f;
    [SerializeField] private float currrentClickHold;
    private bool isClickHold;

    [SerializeField] CubeUIController cubeUIController;
    [SerializeField] TowerSpawnControllerUI towerSpawnControllerUI;



    Cubie selectedCubie;
    CubieFace selectedCubieFace;

    public void Awake()
    {
        cubeUIController = new CubeUIController();
        towerSpawnControllerUI = new TowerSpawnControllerUI();  
    }

    public void Update()
    {
        if (isClickHold)
        {
            currrentClickHold += Time.deltaTime;
        }
    }

    public void SetRotateCubeUpdate(Action<Cubie, CubeAxisType, int> rotateCube) => cubeUIController.SetRotateCubeUpdate(rotateCube);
    public void OnPointerDown(PointerEventData eventData)
    {
        currrentClickHold = 0;
        isClickHold = true;
        cubeUIController.OnPointerDown(eventData);
        towerSpawnControllerUI.OnPointerDown(eventData);  
    }
    public void OnDrag(PointerEventData eventData)
    {
        cubeUIController.OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isClickHold = false;
        if(currrentClickHold < clickHoldTime)
        {
            towerSpawnControllerUI.OnPointerUp(eventData);
        }
        else
        {
            cubeUIController.OnPointerUp(eventData);
        }
    }
}
