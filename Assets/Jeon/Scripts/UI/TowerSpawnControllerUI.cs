using UnityEngine;
using UnityEngine.EventSystems;
[System.Serializable]
public class TowerSpawnControllerUI : IPointerDownHandler, IPointerUpHandler
{
    private CubieFace selectedFace;
    public void OnPointerDown(PointerEventData eventData)
    {
        DetectSelectedCubie(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        selectedFace.SpawnObject(); 
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
                selectedFace = cubie;
                break;
            }
        }
    }
}