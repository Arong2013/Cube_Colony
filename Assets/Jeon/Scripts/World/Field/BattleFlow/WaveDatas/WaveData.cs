using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Battle/Wave Data")]
public class WaveData : ScriptableObject
{
    public string waveName;
    public float startDelay = 15f; // 이 부분은 100000초로 지정해 놓음 
    public List<EnemySpawnSequence> spawnSequences;  
}