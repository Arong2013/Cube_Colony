using UnityEngine.EventSystems;
using UnityEngine;

public static class Utils
{
    public static T DetectSelectedObject<T>(PointerEventData eventData)
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