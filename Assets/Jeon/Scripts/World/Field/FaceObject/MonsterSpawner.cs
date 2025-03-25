using System;
using UnityEngine;

public class MonsterSpawner : FaceUnit
{
    private EnemySpawnSequence sequence;
    private Action onEnemyDeath;
    private CubieFace cunCubieFace;

    private float timer;
    private int spawned;
    private bool isSpawning;
    public void Init(EnemySpawnSequence sequence, Action onEnemyDeath,CubieFace cubieFace)
    {
        this.sequence = sequence;
        this.onEnemyDeath = onEnemyDeath;
        this.timer = sequence.delayBeforeStart;
        this.spawned = 0;
        this.isSpawning = false;
        cunCubieFace = cubieFace;   

        if (sequence.warningTime > 0f)
        {
            Debug.Log($"[Spawner] Warning: {sequence.warningTime}초 후 소환!");
        }
    }
    private void Update()
    {
        if (!isSpawning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isSpawning = true;
                timer = 0f;
            }
            else return;
        }

        if (spawned >= sequence.count)
        {
            DestroySelfIfFinished();
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnMonster();
            spawned++;
            timer = sequence.spawnInterval;
        }
    }
    private void SpawnMonster()
    {
        GameObject prefab = MonsterFactory.Instance.GetPrefab(sequence.monsterId);
        var faceObj =  cunCubieFace.SpawnObject(prefab);
        faceObj.AddOnDeathAction(onEnemyDeath);

        if (sequence.spawnSound != null)
            AudioSource.PlayClipAtPoint(sequence.spawnSound, transform.position);

    }
    private void DestroySelfIfFinished()
    {
        Debug.Log("[Spawner] All monsters spawned. Destroying spawner.");
        Destroy(gameObject);
    }
}
