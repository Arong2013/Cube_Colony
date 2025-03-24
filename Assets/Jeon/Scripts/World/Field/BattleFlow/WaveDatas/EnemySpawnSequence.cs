using System;
using UnityEngine;

[Serializable]
public class EnemySpawnSequence
{
    public string label = "Sequence A";
    public int monsterId;                 
    public int count = 5;
    public float spawnInterval = 1f;
    public float delayBeforeStart = 0f; 
    public CubeFaceType spawnOffset = CubeFaceType.Front;

    [Header("Advanced Settings")]
    public AudioClip spawnSound;
    public float warningTime = 0f;
    public SequenceType sequenceType = SequenceType.Parallel;
}

public enum SequenceType
{
    Parallel,
    Sequential
}
