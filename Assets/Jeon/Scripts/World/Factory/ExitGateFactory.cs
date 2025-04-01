using System.Collections.Generic;
using UnityEngine;

public class ExitGateFactory
{
    private static ExitGateFactory _instance;
    public static ExitGateFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ExitGateFactory();
                _instance.Initialize();
            }
            return _instance;
        }
    }

    private Dictionary<int, GameObject> exitGatePrefabs = new();
    private bool isInitialized = false;

    private ExitGateFactory() { }

    private void Initialize()
    {
        if (isInitialized) return;

        // "ExitGateFactory" 폴더에서 ExitGate 관련 Prefab을 로드
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("ExitGateFactory");

        for (int i = 0; i < loadedPrefabs.Length; i++)
        {
            exitGatePrefabs[i] = loadedPrefabs[i];
            Debug.Log($"[ExitGateFactory] Registered ExitGate ID {i} → {loadedPrefabs[i].name}");
        }

        isInitialized = true;
    }

    public GameObject GetPrefab(int id)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[ExitGateFactory] Not initialized. Initializing now...");
            Initialize();
        }

        if (!exitGatePrefabs.TryGetValue(id, out var prefab))
        {
            Debug.LogError($"[ExitGateFactory] ID {id} not found.");
            return null;
        }

        return prefab;
    }
}