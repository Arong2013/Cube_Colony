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
    public static T GetUI<T>(string _name = null) where T : MonoBehaviour
    {
        T component = null;
        if (component == null)
        {
            component = FindInCanvasChildren<T>();
        }
        else if (component == null)
        {
            Debug.Log(component.name + " found in the current scene.");

        }
        return component;
    }
    private static T FindInCanvasChildren<T>() where T : MonoBehaviour
    {
        T component = null;
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas != null)
        {
            component = canvas.GetComponentInChildren<T>(true);
        }

        return component;
    }

}