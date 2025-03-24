using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Battle/Wave Data")]
public class WaveData : ScriptableObject
{
    public string waveName;
    public float startDelay = 15f; // �� �κ��� 100000�ʷ� ������ ���� 
    public List<EnemySpawnSequence> spawnSequences;  
}