using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// 하나의 웨이브 진행을 관리하는 컨트롤러. Tick으로 시간 기반 실행.
/// </summary>
public class WaveController
{
    private readonly Cube cube;
    private readonly Action onWaveComplete;
    private readonly List<WaveData> waveList;
    private readonly Func<int, WaveData> generateWaveFunc;

    private Queue<EnemySpawnSequence> pendingSequences = new();
    private EnemySpawnSequence currentSequence;
    private int spawnedCount;
    private int aliveEnemyCount;

    private float waveTimer;
    private float sequenceTimer;

    private int currentWaveIndex;
    public bool IsWaveRunning { get; private set; }

    public WaveController(List<WaveData> waveList, Cube cube, Action onWaveComplete, Func<int, WaveData> generator = null)
    {
        this.waveList = waveList;
        this.cube = cube;
        this.onWaveComplete = onWaveComplete;
        this.generateWaveFunc = generator;
    }

    /// <summary>
    /// 외부에서 웨이브를 시작할 때 호출
    /// </summary>
    /// 
    public void StartNextWave()
    {
        var wave = GetWave(currentWaveIndex);
        if (wave == null) return;

        InitializeWave(wave);
        currentWaveIndex++; // ✅ 이젠 내부에서 증가!
    }

    public void StartWave(int waveIndex)
    {
        var wave = GetWave(waveIndex);
        if (wave == null) return;

        InitializeWave(wave);
    }
    /// <summary>
    /// 매 프레임마다 호출되어 웨이브의 타이머 및 시퀀스를 진행
    /// </summary>
    public void Tick(float deltaTime)
    {
        if (!IsWaveRunning) return;

        if (waveTimer > 0f)
        {
            waveTimer -= deltaTime;
            return;
        }

        if (currentSequence == null && pendingSequences.Count > 0)
        {
            BeginNextSequence();
        }

        if (currentSequence != null)
        {
            UpdateSequence(deltaTime);
        }
    }

    private WaveData GetWave(int index)
    {
        if (waveList != null && index < waveList.Count)
            return waveList[index];

        return generateWaveFunc?.Invoke(index);
    }

    private void InitializeWave(WaveData wave)
    {
        IsWaveRunning = true;
        waveTimer = wave.startDelay;
        currentWaveIndex++;

        PrepareSequenceQueue(wave.spawnSequences);

        currentSequence = null;
        spawnedCount = 0;
        sequenceTimer = 0f;
    }

    private void PrepareSequenceQueue(List<EnemySpawnSequence> sequences)
    {
        pendingSequences.Clear();
        foreach (var seq in sequences)
        {
            pendingSequences.Enqueue(seq);
        }
    }

    private void BeginNextSequence()
    {
        currentSequence = pendingSequences.Dequeue();
        sequenceTimer = currentSequence.delayBeforeStart;
        spawnedCount = 0;
    }

    private void UpdateSequence(float deltaTime)
    {
        if (sequenceTimer > 0f)
        {
            sequenceTimer -= deltaTime;
            return;
        }

        SpawnEnemy(currentSequence);
        spawnedCount++;
        sequenceTimer = currentSequence.spawnInterval;

        if (spawnedCount >= currentSequence.count)
        {
            currentSequence = null;
        }
    }

    private void SpawnEnemy(EnemySpawnSequence seq)
    {
        //GameObject prefab = MonsterFactory.GetPrefab(seq.monsterId);
        //Vector3 spawnPosition = cube.GetFaceWorldPosition(seq.spawnOffset);

        //GameObject enemyGO = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);
        //Enemy enemy = enemyGO.GetComponent<Enemy>();
        //enemy.OnDeath += OnEnemyDeath;

        aliveEnemyCount++;
    }

    private void OnEnemyDeath()
    {
        aliveEnemyCount--;

        if (aliveEnemyCount <= 0 && pendingSequences.Count == 0 && currentSequence == null)
        {
            IsWaveRunning = false;
            onWaveComplete?.Invoke();
        }
    }
}
