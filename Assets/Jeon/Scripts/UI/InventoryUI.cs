using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UI;

public class InventoryUI : SerializedMonoBehaviour, IObserver
{
    [TitleGroup("인벤토리 UI")]
    [LabelText("슬롯 컨테이너"), Required]
    [SerializeField] private Transform _slotContainer;

    [TitleGroup("고정 장비 슬롯")]
    [LabelText("장비 슬롯들"), Required]
    [DictionaryDrawerSettings(KeyLabel = "장비 타입", ValueLabel = "슬롯")]
    [SerializeField] private Dictionary<EquipmentType, EQSlot> equipmentSlots = new Dictionary<EquipmentType, EQSlot>();

    [TitleGroup("아이템 정보")]
    [LabelText("아이템 정보 UI"), Required]
    [SerializeField] private ItemInfoUI _itemInfoUI;

        [TitleGroup("닫기 버튼튼")]
    [LabelText("닫기 버튼 UI"), Required]
    [SerializeField] private Button _CloseButton;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private List<ItemSlot> _slots = new();

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private int itemCount => BattleFlowController.Instance?.playerData?.inventory?.Count ?? 0;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private int totalSlots => GetTotalInventorySlots();

    private void Start()
    {
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.RegisterObserver(this);
            Initialize();
        }

        // 아이템 정보 UI 찾기
        if (_itemInfoUI == null)
        {
            _itemInfoUI = GetComponentInChildren<ItemInfoUI>(true);
            if (_itemInfoUI == null)
            {
                _itemInfoUI = Utils.GetUI<ItemInfoUI>();
            }
        }

        // 아이템 정보 UI 초기화
        if (_itemInfoUI != null)
        {
            _itemInfoUI.Initialize();
        }

        _CloseButton.onClick.AddListener(() => ToggleInventoryUI());
    }

    private void OnDestroy()
    {
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.UnregisterObserver(this);
        }
    }

    public void Initialize()
    {
        InitializeEquipmentSlots();
        UpdateSlots();
        UpdateEquipmentSlots();

    }

    /// <summary>
    /// 장비 슬롯들 초기화
    /// </summary>
    private void InitializeEquipmentSlots()
    {
        foreach (var kvp in equipmentSlots)
        {
            if (kvp.Value != null)
            {
                kvp.Value.Initialize(this);

                // 장비 슬롯 클릭 이벤트 추가
                var slotType = kvp.Key;
                var slot = kvp.Value;

                // 기존 이벤트 제거 후 새로 추가
                var button = slot.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnEquipmentSlotClicked(slotType));
                }
            }
        }
    }

    /// <summary>
    /// 장비 슬롯 클릭 처리
    /// </summary>
    private void OnEquipmentSlotClicked(EquipmentType slotType)
    {
        var equippedItem = GetEquippedItem(slotType);

        if (equippedItem != null)
        {
            // 장착된 아이템이 있으면 아이템 정보 UI 표시
            ShowItemInfo(equippedItem);
        }
        else
        {
            // 빈 슬롯이면 해당 타입의 장비 아이템 강조 표시
            Debug.Log($"{slotType} 슬롯이 비어있습니다. 인벤토리에서 해당 타입의 장비를 확인하세요.");
            HighlightEquipableItems(slotType);
        }
    }

    /// <summary>
    /// 특정 타입의 장비 아이템들 강조 표시
    /// </summary>
    private void HighlightEquipableItems(EquipmentType targetType)
    {
        foreach (var slot in _slots)
        {
            var item = slot.GetComponent<ItemSlot>(); // 실제 ItemSlot 컴포넌트에서 아이템 가져오기

            // 이 부분은 ItemSlot에 public 프로퍼티나 메서드가 필요할 수 있습니다.
            // 예시로 코메ント 처리
            /*
            if (item != null && item.CurrentItem is EquipableItem equipable)
            {
                if (equipable.equipmentType == targetType)
                {
                    // 해당 타입 아이템 강조
                    HighlightSlot(slot, true);
                }
                else
                {
                    // 다른 타입 아이템 흐리게
                    HighlightSlot(slot, false);
                }
            }
            */
        }
    }

    /// <summary>
    /// 슬롯 강조 표시
    /// </summary>
    private void HighlightSlot(ItemSlot slot, bool highlight)
    {
        var canvasGroup = slot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = slot.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = highlight ? 1f : 0.5f;
    }

    /// <summary>
    /// 장비 슬롯 업데이트
    /// </summary>
    public void UpdateEquipmentSlots()
    {
        if (BattleFlowController.Instance?.playerData == null)
            return;

        Debug.Log($"장비 슬롯 업데이트 시작. 장비 슬롯 수: {equipmentSlots.Count}");

        foreach (var kvp in equipmentSlots)
        {
            Debug.Log($"슬롯 타입 처리 중: {kvp.Key}");

            if (kvp.Value != null)
            {
                var equippedItem = BattleFlowController.Instance.playerData.GetEquippedItem(kvp.Key);

                if (equippedItem != null)
                {
                    Debug.Log($"{kvp.Key} 슬롯에 장착된 아이템: {equippedItem.ItemName}");
                    kvp.Value.EquipItem(equippedItem);
                }
                else
                {
                    Debug.Log($"{kvp.Key} 슬롯에 장착된 아이템 없음");
                    kvp.Value.UnequipItem();
                }
            }
        }
    }
    /// <summary>
    /// 인벤토리 UI 열기
    /// </summary>
    public void OpenInventoryUI()
    {
        gameObject.SetActive(true);
        UpdateSlots();
        UpdateEquipmentSlots();


        // 열 때 아이템 정보 UI 닫기
        if (_itemInfoUI != null)
        {
            _itemInfoUI.Hide();
        }
    }

    /// <summary>
    /// 인벤토리 UI 토글
    /// </summary>
    public void ToggleInventoryUI()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            CloseAllRelatedUIs();
        }
        else
        {
            OpenInventoryUI();
        }
    }

    /// <summary>
    /// 모든 관련 UI 닫기
    /// </summary>
    private void CloseAllRelatedUIs()
    {
        if (_itemInfoUI != null)
        {
            _itemInfoUI.Hide();
        }
    }

    /// <summary>
    /// 아이템 정보 UI 표시
    /// </summary>
    public void ShowItemInfo(Item item)
    {
        if (_itemInfoUI != null && item != null)
        {
            _itemInfoUI.Show(item, UseItemCallback, DiscardItemCallback);
        }
    }

    /// <summary>
    /// 일반 아이템 슬롯 업데이트 (장비 아이템 제외)
    /// </summary>
    public void UpdateSlots()
    {
        if (BattleFlowController.Instance == null ||
            BattleFlowController.Instance.playerData == null ||
            BattleFlowController.Instance.playerData.inventory == null)
            return;

        // 기존 슬롯 제거
        foreach (Transform child in _slotContainer)
        {
            Destroy(child.gameObject);
        }

        _slots.Clear();

        foreach (var item in BattleFlowController.Instance.playerData.inventory)
        {
            if (item is EquipableItem equipable)
            {
                // PlayerData의 장착된 아이템 확인으로 변경
                var equippedItem = BattleFlowController.Instance.playerData.GetEquippedItem(equipable.equipmentType);

                if (equippedItem == equipable)
                {
                    continue; // 이미 장착된 아이템은 일반 슬롯에 표시하지 않음
                }
            }

            // 나머지 아이템들은 슬롯에 추가
            var curSlot = Instantiate(DataCenter.Instance.GetItemSlotPrefab().gameObject, _slotContainer);
            var slot = curSlot.GetComponent<ItemSlot>();
            slot.SetItem(item);
            _slots.Add(slot);
        }

    }

    /// <summary>
    /// 사용 중인 인벤토리 슬롯 수 계산
    /// </summary>
    private int GetUsedInventorySlots()
    {
        if (BattleFlowController.Instance?.playerData?.inventory == null)
            return 0;

        int count = 0;

        foreach (var item in BattleFlowController.Instance.playerData.inventory)
        {
            // 장착된 장비 아이템은 제외
            if (item is EquipableItem equipable)
            {
                var equippedItem = BattleFlowController.Instance.playerData.GetEquippedItem(equipable.equipmentType);
                if (equippedItem == equipable)
                {
                    continue;
                }
            }
            count++;
        }

        return count;
    }
    /// <summary>
    /// 총 인벤토리 슬롯 수 계산 (장비 효과 포함)
    /// </summary>
    private int GetTotalInventorySlots()
    {
        int baseSlots = 10; // 기본 슬롯 수
        var playerData = BattleFlowController.Instance?.playerData;

        if (playerData != null)
        {
            // 장착된 장비 중 인벤토리 슬롯 보너스 계산
            foreach (var item in playerData.GetAllEquippedItems().Values)
            {
                var effects = item.GetCurrentEffects();
                baseSlots += effects.inventorySlotBonus;
            }
        }

        return baseSlots;
    }
    /// <summary>
    /// 옵저버 업데이트
    /// </summary>
    public void UpdateObserver()
    {
        if (!gameObject.activeInHierarchy)
        {
            UpdateSlots();
            UpdateEquipmentSlots();
        }
    }

    /// <summary>
    /// 인벤토리 UI 비활성화
    /// </summary>
    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
        CloseAllRelatedUIs();
    }

    /// <summary>
    /// 장비 슬롯 찾기
    /// </summary>
    public EQSlot GetEquipmentSlot(EquipmentType type)
    {
        return equipmentSlots.TryGetValue(type, out EQSlot slot) ? slot : null;
    }

    /// <summary>
    /// 특정 타입의 장착된 아이템 가져오기
    /// </summary>
    public EquipableItem GetEquippedItem(EquipmentType type)
    {
        return BattleFlowController.Instance?.playerData?.GetEquippedItem(type);
    }



    /// <summary>
    /// 특정 장비 슬롯에 아이템 장착 (드래그앤드롭 등에서 사용)
    /// </summary>
    public bool EquipItemToSlot(EquipmentType slotType, EquipableItem item)
    {
        if (item == null || item.equipmentType != slotType)
        {
            Debug.LogWarning($"잘못된 아이템 타입입니다. 슬롯: {slotType}, 아이템: {item?.equipmentType}");
            return false;
        }

        var player = Utils.GetPlayer();
        if (player == null) return false;

        // 아이템 사용 (장착)
        item.Use(player);

        // UI 업데이트
        UpdateSlots();
        UpdateEquipmentSlots();

        return true;
    }

    /// <summary>
    /// 특정 장비 슬롯에서 아이템 해제
    /// </summary>
    public bool UnequipItemFromSlot(EquipmentType slotType)
    {
        var playerData = BattleFlowController.Instance?.playerData;
        if (playerData == null) return false;

        var unequippedItem = playerData.UnequipItem(slotType);
        if (unequippedItem != null)
        {
            // 인벤토리에 아이템 추가
            playerData.AddItem(unequippedItem);

            // UI 업데이트
            UpdateSlots();
            UpdateEquipmentSlots();

            return true;
        }

        return false;
    }
    
    /// <summary>
    /// 아이템 사용 콜백
    /// </summary>
    private void UseItemCallback(Item item)
    {
        if (item == null) return;

        if (BattleFlowController.Instance == null ||
            BattleFlowController.Instance.GetPlayerEntity() == null)
        {
            Debug.LogWarning("플레이어 엔티티를 찾을 수 없습니다.");
            return;
        }

        Debug.Log($"아이템 사용: {item.ItemName}");
        item.Use(BattleFlowController.Instance.GetPlayerEntity());

        // 소모품이 모두 소진되었는지 확인
        if (item is ConsumableItem consumable && consumable.cunamount <= 0)
        {
            // 인벤토리에서 제거
            BattleFlowController.Instance.playerData.RemoveItem(item);
        }

        // UI 업데이트
        UpdateSlots();
        UpdateEquipmentSlots();

        // 옵저버 알림
        BattleFlowController.Instance.NotifyObservers();
    }

    /// <summary>
    /// 아이템 버리기 콜백
    /// </summary>
    private void DiscardItemCallback(Item item)
    {
        if (item == null) return;

        if (BattleFlowController.Instance == null ||
            BattleFlowController.Instance.playerData == null)
        {
            Debug.LogWarning("플레이어 데이터를 찾을 수 없습니다.");
            return;
        }

        Debug.Log($"아이템 버리기: {item.ItemName}");

        // 인벤토리에서 제거
        BattleFlowController.Instance.playerData.RemoveItem(item);

        // UI 업데이트
        UpdateSlots();
        UpdateEquipmentSlots();

        // 옵저버 알림
        BattleFlowController.Instance.NotifyObservers();
    }
    /// <summary>
    /// 디버깅용: 현재 장착 상태 출력
    /// </summary>
    [Button("현재 장착 상태 출력")]
    public void PrintEquipmentStatus()
    {
        var playerData = BattleFlowController.Instance?.playerData;
        if (playerData == null)
        {
            Debug.LogWarning("플레이어 데이터를 찾을 수 없습니다.");
            return;
        }

        // 장비 상태 출력
        Debug.Log("<color=yellow>=== 현재 장착 상태 ===</color>");
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
        {
            if (type == EquipmentType.None) continue;

            var item = playerData.GetEquippedItem(type);
            if (item != null)
            {
                string reinforcementInfo = item.currentReinforcementLevel > 0 ? $" <color=cyan>(+{item.currentReinforcementLevel})</color>" : "";
                Debug.Log($"{type}: <color=green>{item.GetDisplayName()}</color>{reinforcementInfo}");
            }
            else
            {
                Debug.Log($"{type}: <color=red>비어있음</color>");
            }
        }
    }
    /// <summary>
    /// 장비 슬롯 딕셔너리 정보 출력 (디버그용)
    /// </summary>
    [Button("장비 슬롯 정보 출력")]
    public void PrintEquipmentSlotInfo()
    {
        Debug.Log("<color=cyan>=== 장비 슬롯 딕셔너리 정보 ===</color>");
        foreach (var kvp in equipmentSlots)
        {
            string slotStatus = kvp.Value != null ? "할당됨" : "null";
            Debug.Log($"{kvp.Key}: {slotStatus}");
        }
    }
}