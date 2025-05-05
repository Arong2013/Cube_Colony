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

    [Title("리턴 게이트 데이터")]
    [SerializeField]
    private Dictionary<int, GameObject> ExitGateData = new();


    [Title("UI 데이터")]
    [SerializeField] private GameObject itemSlotPrefab;

    [Title("드롭 아이템 프리펩")]
    [SerializeField] private GameObject dropItemPrefab; 
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
    public GameObject GetExitGate(int id)
    {
        if (ExitGateData.TryGetValue(id, out var entity))
        {
            return entity;
        }
        else
        {
            Debug.LogError($"[DataCenter] Entity with ID {id} not found.");
            return null;
        }
    }
    public GameObject GetItemSlotPrefab()
    {
        return itemSlotPrefab;
    }   

    public GameObject GetDropItemPrefab()
    {
        return dropItemPrefab;
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
