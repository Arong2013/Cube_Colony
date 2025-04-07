using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DataCenter : SerializedMonoBehaviour
{
    public static DataCenter Instance { get; private set; }

    [Title("스킬별 시각 데이터")]
    [SerializeField]
    [DictionaryDrawerSettings(KeyLabel = "Skill Type", ValueLabel = "Face Data")]
    private Dictionary<CubieFaceSkillType, CubieFaceVisualData> cubieFaceDataMap = new();


    [Title("플레이어 데이터")]
    [SerializeField]
    private GameObject playerEntityPreFabs;  
    public CubieFaceVisualData GetFaceData(CubieFaceSkillType type)
    {
        return cubieFaceDataMap.TryGetValue(type, out var data) ? data : null;
    }
    public GameObject GetPlayerEntity()
    {
        return playerEntityPreFabs;
    }   

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
