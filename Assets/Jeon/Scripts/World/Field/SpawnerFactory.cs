using UnityEngine;

public static class SpawnerFactory
{
    private static GameObject spawnerPrefab;

    public static void SetSpawnerPrefab(GameObject prefab)
    {
        spawnerPrefab = prefab;
    }

    public static MonsterSpawner Create(EnemySpawnSequence sequence, Transform parent)
    {
        Vector3 spawnPos = parent.position + GetOffsetFromFace(sequence.spawnOffset);
        GameObject go = Object.Instantiate(spawnerPrefab, spawnPos, Quaternion.identity, parent);

        return go.GetComponent<MonsterSpawner>();
    }
    private static Vector3 GetOffsetFromFace(CubeFaceType face)
    {
        return face switch
        {
            CubeFaceType.Front => Vector3.forward * 5,
            CubeFaceType.Back => Vector3.back * 5,
            CubeFaceType.Left => Vector3.left * 5,
            CubeFaceType.Right => Vector3.right * 5,
            CubeFaceType.Top => Vector3.up * 5,
            CubeFaceType.Bottom => Vector3.down * 5,
            _ => Vector3.zero,
        };
    }
}
