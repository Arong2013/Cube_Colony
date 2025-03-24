using System.Collections.Generic;
using UnityEngine;

public static class MonsterFactory
{
    private static Dictionary<int, GameObject> monsterPrefabs = new();

    public static void Register(int id, GameObject prefab)
    {
        if (!monsterPrefabs.ContainsKey(id))
            monsterPrefabs.Add(id, prefab);
    }

    public static GameObject GetPrefab(int id)
    {
        if (!monsterPrefabs.ContainsKey(id))
        {
            Debug.LogError($"[MonsterFactory] ID {id} not registered.");
            return null;
        }
        return monsterPrefabs[id];
    }
}
