using System.Collections.Generic;
using UnityEngine;

public class SpawnerFactory
{
    private static SpawnerFactory _instance;
    public static SpawnerFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SpawnerFactory();
                _instance.Initialize();
            }
            return _instance;
        }
    }

    private Dictionary<int, GameObject> spawnerPrefabs = new();
    private bool isInitialized = false;

    private SpawnerFactory() { }

    private void Initialize()
    {
        if (isInitialized) return;

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("SpawnerFactory");

        for (int i = 0; i < loadedPrefabs.Length; i++)
        {
            spawnerPrefabs[i] = loadedPrefabs[i];
            Debug.Log($"[SpawnerFactory] Registered Spawner ID {i} → {loadedPrefabs[i].name}");
        }

        isInitialized = true;
    }

    public GameObject GetPrefab(int id)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[SpawnerFactory] Not initialized. Initializing now...");
            Initialize();
        }

        if (!spawnerPrefabs.TryGetValue(id, out var prefab))
        {
            Debug.LogError($"[SpawnerFactory] ID {id} not found.");
            return null;
        }

        return prefab;
    }
}
