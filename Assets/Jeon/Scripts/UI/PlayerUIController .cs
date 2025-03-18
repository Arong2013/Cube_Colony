using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class PlayerUIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] float clickHoldTime = 1f;
    
    private float currrentClickHold;
    private bool isClickHold;

    [SerializeField] CubeUIController cubeUIController = new CubeUIController();
    [SerializeField] TowerSpawnControllerUI towerSpawnControllerUI = new TowerSpawnControllerUI();

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
        RayCubieFace(eventData);
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

    private void RayCubieFace(PointerEventData eventData)
    {
        var  selectedCubieFace = DetectSelectedCubieFace<CubieFace>(eventData);
        if (selectedCubieFace)
        {
            cubeUIController.OnPointerDown(eventData);
            towerSpawnControllerUI.OnPointerDown(selectedCubieFace);
        }
    }

    private T DetectSelectedCubieFace<T>(PointerEventData eventData)
        where T : MonoBehaviour
    {
        T selectedObject = null;    

        if (Camera.main == null) return selectedObject;

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        if (hits.Length == 0) return selectedObject;

        T closestCubieFace = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out T cubieFace))
            {
                float hitDistance = Vector3.Distance(ray.origin, hit.point);
                if (hitDistance < minDistance)
                {
                    closestCubieFace = cubieFace;
                    minDistance = hitDistance;
                }
            }
        }
        return closestCubieFace;    
    }
}
