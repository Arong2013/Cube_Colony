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

    [Title("엔티티 데이터")]
    [SerializeField]
    private Dictionary<int, GameObject> EntityData = new();
    public CubieFaceVisualData GetFaceData(CubieFaceSkillType type)
    {
        return cubieFaceDataMap.TryGetValue(type, out var data) ? data : null;
    }
    public GameObject GetPlayerEntity()
    {
        return playerEntityPreFabs;
    }   
    public GameObject GetEntity(int id)
    {
        if (EntityData.TryGetValue(id, out var entity))
        {
            return entity;
        }
        else
        {
            Debug.LogError($"[DataCenter] Entity with ID {id} not found.");
            return null;
        }
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
