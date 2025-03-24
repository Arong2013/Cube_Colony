using System;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private EnemySpawnSequence sequence;
    private Action onEnemyDeath;

    private float timer;
    private int spawned;
    private bool isSpawning;

    public void Init(EnemySpawnSequence sequence, Action onEnemyDeath)
    {
        this.sequence = sequence;
        this.onEnemyDeath = onEnemyDeath;
        this.timer = sequence.delayBeforeStart;
        this.spawned = 0;
        this.isSpawning = false;

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
        GameObject prefab = MonsterFactory.GetPrefab(sequence.monsterId);
        GameObject enemyGO = Instantiate(prefab, transform.position, Quaternion.identity);

        var enemy = enemyGO.GetComponent<FaceObject>();
        enemy.AddOnDeathAction(onEnemyDeath);

        if (sequence.spawnSound != null)
            AudioSource.PlayClipAtPoint(sequence.spawnSound, transform.position);
    }
    private void DestroySelfIfFinished()
    {
        Debug.Log("[Spawner] All monsters spawned. Destroying spawner.");
        Destroy(gameObject);
    }
}
