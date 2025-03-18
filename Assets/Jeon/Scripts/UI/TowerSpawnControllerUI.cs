using UnityEngine;
using UnityEngine.EventSystems;
[System.Serializable]
public class TowerSpawnControllerUI 
{
    private CubieFace selectedFace;
    public void OnPointerDown(CubieFace cubieFace)
    {
        selectedFace = cubieFace;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        selectedFace.SpawnObject(); 
    }
}