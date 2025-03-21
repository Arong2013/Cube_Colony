using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PlayerUIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IDragHandler
{
    [SerializeField] private float clickHoldTime = 1f;

    private CubieFace currentSelectedCubieFace;
    private float currentClickHold;
    private bool isClickHold;
    private bool isHoldClass;

    private Vector2 initialMousePosition;
    [SerializeField] CubeRotaterUI cubeRotaterUI = new CubeRotaterUI();
    [SerializeField] TowerSpawnControllerUI towerSpawnControllerUI;

    private Action<Cubie, CubeAxisType, int> rotateAction;
    

    public void Update()
    {
        if (isClickHold)
        {
            currentClickHold += Time.deltaTime;
            if (currentClickHold > clickHoldTime && currentSelectedCubieFace)
            {
                isHoldClass = true;
                cubeRotaterUI.SetUp(RotateCubeEvent, currentSelectedCubieFace, initialMousePosition);
            }
                
        }
        else
        {
            if(currentClickHold > 0.1f && currentSelectedCubieFace)
            {
                isHoldClass = false;
            }
        }
    }
    public void SetRotateAction(Action<Cubie, CubeAxisType, int> rotateCube)
    {
        rotateAction = rotateCube;
    }
    public void RotateCubeEvent(CubeAxisType cubeAxisType, int isClock)
    {
        rotateAction?.Invoke(currentSelectedCubieFace.cubie, cubeAxisType, isClock);
    }
    public void Init()
    {
        currentClickHold = 0;
        isClickHold = true; 
        currentSelectedCubieFace = null;
        isHoldClass = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Init();
        DetectSelectedCubieFace(eventData);
        initialMousePosition = eventData.position;  
    }
     public void OnDrag(PointerEventData eventData)
    {
        if(isHoldClass)
        {
            cubeRotaterUI.OndDrag(eventData);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isClickHold = false;
        if (isHoldClass)
        {
            cubeRotaterUI.OnPointerUP();
            isHoldClass = false;
        }
           
    }
    private void DetectSelectedCubieFace(PointerEventData eventData)
    {
        currentSelectedCubieFace = Utils.DetectSelectedObject<CubieFace>(eventData);
    }


}
