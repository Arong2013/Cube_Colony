using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System.Reflection;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class DataCenter : SerializedMonoBehaviour
{
    public static DataCenter Instance { get; private set; }

    [TitleGroup("자동 등록")]
    [InfoBox("ScriptableObject 데이터들이 리플렉션으로 자동 등록됩니다.", InfoMessageType.Info)]
    [Button("🔄 모든 SO 리플렉션 스캔 및 등록", ButtonSizes.Large), GUIColor(0.3f, 0.8f, 0.3f)]
    public void AutoRegisterAllScriptableObjects()
    {
#if UNITY_EDITOR
        ScriptableObjectAutoRegistrar.RegisterSOsToDataCenterPrefab();
#endif
    }

    [TitleGroup("자동 등록")]
    [Button("🔍 등록된 데이터 정보 확인", ButtonSizes.Medium), GUIColor(0.3f, 0.5f, 0.9f)]
    public void PrintRegisteredData()
    {
        PrintAllRegisteredData();
    }

    // ===== SO 데이터 저장소 =====
    [FoldoutGroup("📦 아이템 데이터", expanded: false)]
    [DictionaryDrawerSettings(KeyLabel = "ID", ValueLabel = "SO")]
    [SerializeField] private Dictionary<int, ConsumableItemSO> consumableItems = new();

    [FoldoutGroup("📦 아이템 데이터")]
    [DictionaryDrawerSettings(KeyLabel = "ID", ValueLabel = "SO")]
    [SerializeField] private Dictionary<int, EquipableItemSO> equipableItems = new();

    [FoldoutGroup("📦 아이템 데이터")]
    [DictionaryDrawerSettings(KeyLabel = "ID", ValueLabel = "SO")]
    [SerializeField] private Dictionary<int, ItemActionSO> itemActions = new();

    [FoldoutGroup("🗺️ 필드 데이터", expanded: false)]
    [DictionaryDrawerSettings(KeyLabel = "ID", ValueLabel = "SO")]
    [SerializeField] private Dictionary<int, FieldTileDataSO> fieldTileDatas = new();

    // ===== 기존 수동 등록 데이터들 =====
    [FoldoutGroup("🎲 큐브 데이터", expanded: true)]
    [LabelText("스킬별 시각 데이터")]
    [DictionaryDrawerSettings(KeyLabel = "스킬 타입", ValueLabel = "시각 데이터")]
    [SerializeField] private Dictionary<CubieFaceSkillType, CubieFaceVisualData> cubieFaceDataMap = new();

    [FoldoutGroup("🎭 엔티티 데이터", expanded: false)]
    [LabelText("플레이어 엔티티")]
    [AssetsOnly, Required]
    [SerializeField] private GameObject playerEntityPreFabs;

    [FoldoutGroup("🎭 엔티티 데이터")]
    [LabelText("일반 엔티티")]
    [DictionaryDrawerSettings(KeyLabel = "ID", ValueLabel = "프리팹")]
    [SerializeField] private Dictionary<int, GameObject> EntityData = new();

    [FoldoutGroup("🎭 엔티티 데이터")]
    [LabelText("출구 게이트")]
    [DictionaryDrawerSettings(KeyLabel = "ID", ValueLabel = "프리팹")]
    [SerializeField] private Dictionary<int, GameObject> ExitGateData = new();

    [FoldoutGroup("🎨 UI 데이터", expanded: false)]
    [LabelText("아이템 슬롯 프리팹")]
    [AssetsOnly, Required]
    [SerializeField] private GameObject itemSlotPrefab;

    [FoldoutGroup("🎨 UI 데이터")]
    [LabelText("드롭 아이템 프리팹")]
    [AssetsOnly, Required]
    [SerializeField] private GameObject dropItemPrefab;

    // ===== 동적 정보 =====
    [FoldoutGroup("🔍 동적 정보", expanded: false)]
    [ShowInInspector, ReadOnly]
    public Dictionary<string, int> 등록된_SO_개수 => GetAllDictionaryCounts();

    [FoldoutGroup("🔍 동적 정보")]
    [ShowInInspector, ReadOnly]
    public int 총_SO_개수 => GetAllDictionaryCounts().Values.Sum();

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

        DontDestroyOnLoad(gameObject);
    }

    // ===== 새 인스턴스 생성해서 반환하는 메서드들 =====

    /// <summary>
    /// 새로운 ConsumableItem 인스턴스를 생성해서 반환
    /// </summary>
    public ConsumableItem CreateConsumableItem(int id)
    {
        var so = GetConsumableItemSO(id);
        if (so == null) return null;

        var item = new ConsumableItem();
        item.ID = so.ID;
        item.ItemName = so.ItemName;
        item.maxamount = so.maxamount;
        item.ids = new List<int>(so.ids); // 리스트 복사
        item.cunamount = 1; // 기본 개수

        // SO에서 추가 정보도 복사
        item.description = so.description;
        item.grade = so.grade;
        item.itemIcon = so.itemIcon;

        return item;
    }

    /// <summary>
    /// 새로운 EquipableItem 인스턴스를 생성해서 반환
    /// </summary>
    public EquipableItem CreateEquipableItem(int id)
    {
        var so = GetEquipableItemSO(id);
        if (so == null) return null;

        var item = new EquipableItem();
        item.ID = so.ID;
        item.ItemName = so.ItemName;
        item.equipmentType = so.equipmentType;
        item.requiredLevel = so.requiredLevel;
        item.attackBonus = so.attackBonus;
        item.defenseBonus = so.defenseBonus;
        item.healthBonus = so.healthBonus;
        item.description = so.description;
        item.grade = so.grade;
        item.itemIcon = so.itemIcon;

        return item;
    }

    /// <summary>
    /// 새로운 itemAction 인스턴스를 생성해서 반환
    /// </summary>
    public itemAction CreateItemAction(int id)
    {
        var so = GetItemActionSO(id);
        if (so == null) return null;

        // ActionName으로 적절한 타입 찾기
        Type actionType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t =>
                t.Name == so.ActionName &&
                typeof(itemAction).IsAssignableFrom(t) &&
                !t.IsAbstract);

        if (actionType == null)
        {
            Debug.LogWarning($"[DataCenter] ActionType '{so.ActionName}' 찾을 수 없음");
            return null;
        }

        try
        {
            // 임시 인스턴스 생성
            itemAction tempInstance = (itemAction)Activator.CreateInstance(actionType);

            // Data 파싱 (현재는 int 하나만 지원, 확장 가능)
            object[] args = ParseActionData(so.Data);

            // CreatAction으로 실제 인스턴스 생성
            itemAction finalAction = tempInstance.CreatAction(args);
            finalAction.ID = so.ID;

            return finalAction;
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataCenter] ItemAction 생성 실패 {so.ActionName}: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 새로운 FieldTileData 인스턴스를 생성해서 반환
    /// </summary>
    public FieldTileData CreateFieldTileData(int id)
    {
        var so = GetFieldTileDataSO(id);
        if (so == null) return null;

        var data = new FieldTileData();
        data.ID = so.ID;
        data.StageLevel = so.StageLevel;
        data.minCount = so.minCount;
        data.maxCount = so.maxCount;
        data.ObjectID = new List<int>(so.ObjectID); // 리스트 복사
        data.ObjectValue = new List<float>(so.ObjectValue); // 리스트 복사
        data.description = so.description;

        return data;
    }

    /// <summary>
    /// 리플렉션으로 모든 타입의 새 인스턴스를 생성 (범용)
    /// </summary>
    public T CreateData<T>(int id)
    {
        // DataCenter에서 Create 메서드를 리플렉션으로 찾기
        string methodName = $"Create{typeof(T).Name}";
        var method = typeof(DataCenter).GetMethod(methodName);

        if (method != null && method.ReturnType == typeof(T))
        {
            try
            {
                return (T)method.Invoke(this, new object[] { id });
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataCenter] {methodName} 호출 실패: {e.Message}");
            }
        }

        // 직접 매칭이 안되면 상속 관계 고려한 메서드 찾기
        var createMethods = typeof(DataCenter).GetMethods()
            .Where(m => m.Name.StartsWith("Create") &&
                       m.GetParameters().Length == 1 &&
                       m.GetParameters()[0].ParameterType == typeof(int));

        foreach (var createMethod in createMethods)
        {
            if (typeof(T).IsAssignableFrom(createMethod.ReturnType))
            {
                try
                {
                    object result = createMethod.Invoke(this, new object[] { id });
                    if (result is T typedResult)
                    {
                        return typedResult;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[DataCenter] {createMethod.Name} 호출 실패: {e.Message}");
                }
            }
        }

        Debug.LogWarning($"[DataCenter] {typeof(T).Name}을 위한 Create 메서드를 찾을 수 없습니다. ID: {id}");
        return default;
    }

    /// <summary>
    /// Item 타입으로 요청시 모든 아이템 타입 시도
    /// </summary>
    public Item CreateItem(int id)
    {
        // ConsumableItem 시도
        var consumable = CreateConsumableItem(id);
        if (consumable != null) return consumable;

        // EquipableItem 시도
        var equipable = CreateEquipableItem(id);
        if (equipable != null) return equipable;

        return null;
    }

    /// <summary>
    /// 액션 데이터를 파싱하여 object[] 반환
    /// </summary>
    private object[] ParseActionData(string data)
    {
        if (string.IsNullOrEmpty(data))
            return new object[0];

        // 단순한 구분자로 분할 (확장 가능)
        string[] parts = data.Split(',');
        object[] result = new object[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i].Trim();

            // 숫자인지 확인
            if (int.TryParse(part, out int intValue))
                result[i] = intValue;
            else if (float.TryParse(part, out float floatValue))
                result[i] = floatValue;
            else if (bool.TryParse(part, out bool boolValue))
                result[i] = boolValue;
            else
                result[i] = part; // 문자열로 처리
        }

        return result;
    }

    // ===== SO 접근 메서드들 =====
    public ConsumableItemSO GetConsumableItemSO(int id) => consumableItems.TryGetValue(id, out var item) ? item : null;
    public EquipableItemSO GetEquipableItemSO(int id) => equipableItems.TryGetValue(id, out var item) ? item : null;
    public ItemActionSO GetItemActionSO(int id) => itemActions.TryGetValue(id, out var action) ? action : null;
    public FieldTileDataSO GetFieldTileDataSO(int id) => fieldTileDatas.TryGetValue(id, out var data) ? data : null;

    // ===== 등록 메서드들 =====
    public void RegisterConsumableItem(int id, ConsumableItemSO item) => consumableItems[id] = item;
    public void RegisterEquipableItem(int id, EquipableItemSO item) => equipableItems[id] = item;
    public void RegisterItemAction(int id, ItemActionSO action) => itemActions[id] = action;
    public void RegisterFieldTileData(int id, FieldTileDataSO data) => fieldTileDatas[id] = data;

    // ===== 기존 접근 메서드들 (하위 호환성) =====
    public CubieFaceVisualData GetFaceData(CubieFaceSkillType type) => cubieFaceDataMap.TryGetValue(type, out var data) ? data : null;
    public GameObject GetPlayerEntity() => playerEntityPreFabs;
    public GameObject GetEntity(int id) => EntityData.TryGetValue(id, out var entity) ? entity : null;
    public GameObject GetExitGate(int id) => ExitGateData.TryGetValue(id, out var entity) ? entity : null;
    public GameObject GetItemSlotPrefab() => itemSlotPrefab;
    public GameObject GetDropItemPrefab() => dropItemPrefab;

    // ===== 리플렉션 기반 유틸리티 메서드들 =====

    /// <summary>
    /// 모든 Dictionary 필드의 개수를 리플렉션으로 가져오기
    /// </summary>
    private Dictionary<string, int> GetAllDictionaryCounts()
    {
        var counts = new Dictionary<string, int>();
        var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.FieldType.IsGenericType &&
                field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var dict = field.GetValue(this) as System.Collections.IDictionary;
                if (dict != null)
                {
                    counts[field.Name] = dict.Count;
                }
            }
        }

        return counts;
    }

    /// <summary>
    /// 등록된 모든 데이터 정보 출력
    /// </summary>
    private void PrintAllRegisteredData()
    {
        Debug.Log("=== DataCenter 등록 정보 ===");

        var counts = GetAllDictionaryCounts();
        foreach (var kvp in counts)
        {
            Debug.Log($"📚 {kvp.Key}: {kvp.Value}개");
        }

        Debug.Log($"🔢 총합: {counts.Values.Sum()}개");
    }

    /// <summary>
    /// 리플렉션으로 모든 데이터 검증
    /// </summary>
    [Button("🔍 모든 데이터 검증"), GUIColor(0.8f, 0.8f, 0.3f)]
    public void ValidateAllDataUsingReflection()
    {
        int totalErrors = 0;
        var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.FieldType.IsGenericType &&
                field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var dict = field.GetValue(this) as System.Collections.IDictionary;
                if (dict != null)
                {
                    int errors = ValidateDictionary(field.Name, dict);
                    totalErrors += errors;
                }
            }
        }

        if (totalErrors == 0)
        {
            Debug.Log("✅ 모든 데이터가 유효합니다!");
        }
        else
        {
            Debug.LogWarning($"⚠️ 총 {totalErrors}개의 오류가 발견되었습니다.");
        }
    }

    /// <summary>
    /// Dictionary 데이터 검증
    /// </summary>
    private int ValidateDictionary(string dictionaryName, System.Collections.IDictionary dict)
    {
        int errors = 0;
        foreach (System.Collections.DictionaryEntry entry in dict)
        {
            if (entry.Value == null)
            {
                Debug.LogError($"❌ {dictionaryName} ID {entry.Key}: null 참조");
                errors++;
            }
            else if (entry.Value is ScriptableObject so && so == null)
            {
                Debug.LogError($"❌ {dictionaryName} ID {entry.Key}: 손상된 SO 참조");
                errors++;
            }
        }
        return errors;
    }

    /// <summary>
    /// Item Clone 인터페이스 호환성을 위한 메서드 (ItemDataCenter 대체)
    /// </summary>
    public T GetCloneData<T>(int id) where T : class
    {
        // 리플렉션으로 적절한 Create 메서드 찾기
        return CreateData<T>(id);
    }

    /// <summary>
    /// 다중 아이템 생성
    /// </summary>
    public List<T> CreateMultipleData<T>(int[] ids)
    {
        var results = new List<T>();
        foreach (int id in ids)
        {
            var data = CreateData<T>(id);
            if (data != null)
            {
                results.Add(data);
            }
        }
        return results;
    }

    /// <summary>
    /// 아이템 존재 여부 확인
    /// </summary>
    public bool HasData<T>(int id)
    {
        // SO 존재 여부로 확인
        if (typeof(T) == typeof(ConsumableItem))
            return GetConsumableItemSO(id) != null;
        if (typeof(T) == typeof(EquipableItem))
            return GetEquipableItemSO(id) != null;
        if (typeof(T) == typeof(itemAction))
            return GetItemActionSO(id) != null;
        if (typeof(T) == typeof(FieldTileData))
            return GetFieldTileDataSO(id) != null;

        // 기타 타입은 실제 생성 시도
        return CreateData<T>(id) != null;
    }

    /// <summary>
    /// 특정 타입의 모든 ID 반환
    /// </summary>
    public List<int> GetAllIds<T>()
    {
        if (typeof(T) == typeof(ConsumableItem))
            return consumableItems.Keys.ToList();
        if (typeof(T) == typeof(EquipableItem))
            return equipableItems.Keys.ToList();
        if (typeof(T) == typeof(itemAction))
            return itemActions.Keys.ToList();
        if (typeof(T) == typeof(FieldTileData))
            return fieldTileDatas.Keys.ToList();

        return new List<int>();
    }

    // ===== 디버그 메서드들 =====
    [Button("📊 데이터 통계 출력")]
    public void PrintDataStatistics()
    {
        Debug.Log("=== DataCenter 데이터 통계 ===");
        Debug.Log($"📦 ConsumableItem: {GetAllIds<ConsumableItem>().Count}개");
        Debug.Log($"⚔️ EquipableItem: {GetAllIds<EquipableItem>().Count}개");
        Debug.Log($"🎯 ItemAction: {GetAllIds<itemAction>().Count}개");
        Debug.Log($"🗺️ FieldTileData: {GetAllIds<FieldTileData>().Count}개");
        Debug.Log($"🔢 총 아이템 데이터: {총_SO_개수}개");
    }

    [Button("🧪 데이터 생성 테스트")]
    public void TestDataCreation()
    {
        Debug.Log("=== 데이터 생성 테스트 ===");

        // 각 타입별로 첫 번째 ID로 테스트
        var consumableIds = GetAllIds<ConsumableItem>();
        if (consumableIds.Count > 0)
        {
            var item = CreateConsumableItem(consumableIds[0]);
            Debug.Log($"✅ ConsumableItem 생성 성공: {item?.ItemName}");
        }

        var equipableIds = GetAllIds<EquipableItem>();
        if (equipableIds.Count > 0)
        {
            var item = CreateEquipableItem(equipableIds[0]);
            Debug.Log($"✅ EquipableItem 생성 성공: {item?.ItemName}");
        }

        var actionIds = GetAllIds<itemAction>();
        if (actionIds.Count > 0)
        {
            var action = CreateItemAction(actionIds[0]);
            Debug.Log($"✅ ItemAction 생성 성공: ID {action?.ID}");
        }

        var fieldIds = GetAllIds<FieldTileData>();
        if (fieldIds.Count > 0)
        {
            var field = CreateFieldTileData(fieldIds[0]);
            Debug.Log($"✅ FieldTileData 생성 성공: ID {field?.ID}");
        }
    }
}