using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController
{
    private readonly Cube cube;
    private readonly Action onWaveComplete;
    private int aliveEnemyCount;
    private int currentWaveIndex;
    private List<WaveData> waveList;
    private Func<int, WaveData> generateWaveFunc;

    public WaveController(List<WaveData> waveList, Cube cube, Action onWaveComplete, Func<int, WaveData> generator = null)
    {
        this.waveList = waveList;
        this.cube = cube;
        this.onWaveComplete = onWaveComplete;
        this.generateWaveFunc = generator;
    }

    //public void StartWave(int waveIndex)
    //{
    //    WaveData wave = waveList != null && waveIndex < waveList.Count
    //        ? waveList[waveIndex]
    //        : generateWaveFunc?.Invoke(waveIndex);

    //    if (wave == null) return;

    //    GameManager.Instance.StartCoroutine(ExecuteWave(wave));
    //}

    //private IEnumerator ExecuteWave(WaveData wave)
    //{
    //    yield return new WaitForSeconds(wave.startDelay);

    //    foreach (var seq in wave.spawnSequences)
    //        GameManager.Instance.StartCoroutine(SpawnSequence(seq));

    //    currentWaveIndex++;
    //}

    //private IEnumerator SpawnSequence(EnemySpawnSequence seq)
    //{
    //    yield return new WaitForSeconds(seq.delayBeforeStart);

    //    for (int i = 0; i < seq.count; i++)
    //    {
    //        GameObject prefab = MonsterFactory.GetPrefab(seq.monsterId);
    //        Vector3 pos = cube.transform.position + seq.spawnOffset;

    //        var enemy = GameObject.Instantiate(prefab, pos, Quaternion.identity);
    //        var comp = enemy.GetComponent<Enemy>();
    //        comp.OnDeath += OnEnemyDeath;

    //        aliveEnemyCount++;
    //        yield return new WaitForSeconds(seq.spawnInterval);
    //    }
    //}

    private void OnEnemyDeath()
    {
        aliveEnemyCount--;
        if (aliveEnemyCount <= 0)
        {
            onWaveComplete?.Invoke();
        }
    }
}
