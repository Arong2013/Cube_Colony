using System.Collections.Generic;
using UnityEngine;

public class MonsterFactory
{
    private static MonsterFactory _instance;
    public static MonsterFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MonsterFactory();
                _instance.Initialize(); 
            }
            return _instance;
        }
    }

    private Dictionary<int, GameObject> monsterPrefabs = new();
    private bool isInitialized = false;

    private MonsterFactory() { }

    public void Initialize()
    {
        if (isInitialized) return;

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("MonsterFactory");

        for (int i = 0; i < loadedPrefabs.Length; i++)
        {
            monsterPrefabs[i] = loadedPrefabs[i];
            Debug.Log($"[MonsterFactory] Registered ID {i} → {loadedPrefabs[i].name}");
        }

        isInitialized = true;
    }

    public GameObject GetPrefab(int id)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[MonsterFactory] Not initialized. Initializing now...");
            Initialize();
        }

        if (!monsterPrefabs.TryGetValue(id, out var prefab))
        {
            Debug.LogError($"[MonsterFactory] ID {id} not found.");
            return null;
        }

        return prefab;
    }
}
