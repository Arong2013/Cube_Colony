using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나의 웨이브 진행을 담당 (해당 웨이브의 시퀀스 실행, 몬스터 생성, 웨이브 종료 감지 등)
/// </summary>
public class WaveController
{
    private readonly Cube cube;
    private readonly WaveData waveData;

    private Queue<EnemySpawnSequence> pendingSequences = new();
    private EnemySpawnSequence currentSequence;

    private float waveTimer;
    private int aliveEnemyCount;
    private float sequenceTimer = 0f;

    public bool IsWaveRunning { get; private set; }
    public bool IsComplete => !IsWaveRunning;

    public WaveController(WaveData waveData, Cube cube)
    {
        this.waveData = waveData;
        this.cube = cube;
        InitializeWave();
    }

    private void InitializeWave()
    {
        waveTimer = waveData.startDelay;
        IsWaveRunning = true;

        PrepareSequenceQueue(waveData.spawnSequences);
        currentSequence = null;
    }

    private void PrepareSequenceQueue(List<EnemySpawnSequence> sequences)
    {
        pendingSequences.Clear();
        foreach (var seq in sequences)
        {
            pendingSequences.Enqueue(seq);
        }
    }
    public void Tick(float deltaTime)
    {
        if (!IsWaveRunning) return;

        if (waveTimer > 0f)
        {
            waveTimer -= deltaTime;
            return;
        }

        if (currentSequence != null)
        {
            sequenceTimer -= deltaTime;
            if (sequenceTimer <= 0f)
            {
                SpawnSpawner(currentSequence);
                currentSequence = null;
            }
        }

        if (currentSequence == null && pendingSequences.Count > 0)
        {
            BeginNextSequence();
        }

    }
    private void BeginNextSequence()
    {
        currentSequence = pendingSequences.Dequeue();
        sequenceTimer = currentSequence.delayBeforeStart;
    }
    private void SpawnSpawner(EnemySpawnSequence seq)
    {
        cube.SpawnSpawner(seq, OnEnemyDeath,seq.spawnOffset);
        aliveEnemyCount += seq.count;
    }
    private void OnEnemyDeath()
    {
        aliveEnemyCount--;

        if (aliveEnemyCount <= 0 && pendingSequences.Count == 0 && currentSequence == null)
        {
            IsWaveRunning = false;
        }
    }
}
