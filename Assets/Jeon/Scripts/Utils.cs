using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class Utils
{
    public static T DetectSelectedObject<T>(PointerEventData eventData)
        where T : MonoBehaviour
    {
        T selectedObject = null;

        if (Camera.main == null) return selectedObject;

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        Debug.DrawRay(ray.origin, ray.direction * 300f, Color.red, 2f);

        RaycastHit[] hits = Physics.RaycastAll(ray, 300f);
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
        if (_name != null)
        {
            GameObject go = GameObject.Find(_name);
            if (go != null)
            {
                component = go.GetComponent<T>();
            }
        }

        if (component == null)
        {
            component = FindInCanvasChildren<T>();
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
    public static Vector3 GetCentroid(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;

        foreach (var point in points)
        {
            sum += point;
        }

        return sum / points.Count;
    }

    public static List<IPlayerUesableUI> SetPlayerMarcineOnUI()
    {
        var list = new List<IPlayerUesableUI>();
        GameObject canvas = GameObject.Find("Canvas");
        foreach (Transform child in canvas.GetComponentsInChildren<Transform>(true))
        {
            IPlayerUesableUI usableUI = child.GetComponent<IPlayerUesableUI>();
            if (usableUI != null)
            {
                Debug.Log(child.name);
                list.Add(usableUI);
            }
        }
        return list;
    }

    public static PlayerEntity GetPlayer()
    {
        // GameObject.FindWithTag를 사용하여 플레이어 찾기
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            return playerObj.GetComponent<PlayerEntity>();
        }

        // 태그로 못 찾으면 컴포넌트로 찾기
        PlayerEntity players = GameObject.FindAnyObjectByType<PlayerEntity>();
        if (players != null)
        {
            return players;
        }

        return null;
    }
}