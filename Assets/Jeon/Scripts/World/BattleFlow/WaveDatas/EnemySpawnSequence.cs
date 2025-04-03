using System;
using UnityEngine;

[Serializable]
public class EnemySpawnSequence
{
    public string label = "Sequence A";
    public int spawnerId;
    public int monsterId;                 
    public int count = 5;
    public float spawnInterval = 1f;
    public float delayBeforeStart = 0f; 

    [Header("Advanced Settings")]
    public AudioClip spawnSound;
    public float warningTime = 0f;
}