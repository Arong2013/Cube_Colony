using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class InventoryUI : MonoBehaviour, IObserver
{
    [TitleGroup("인벤토리 UI")]
    [LabelText("슬롯 컨테이너"), Required]
    [SerializeField] private Transform _slotContainer;

    [TitleGroup("장비 시스템")]
    [LabelText("장비 슬롯들"), Required]
    [SerializeField] private List<EQSlot> equipmentSlots = new List<EQSlot>();

    [TitleGroup("장비 시스템")]
    [LabelText("장비 현황 UI"), Required]
    [SerializeField] private EquipmentUI equipmentStatusUI;

    [TitleGroup("아이템 정보")]
    [LabelText("아이템 정보 UI"), Required]
    [SerializeField] private ItemInfoUI _itemInfoUI;

    [TitleGroup("장비 선택 UI")]
    [LabelText("장비 선택 패널")]
    [SerializeField] private GameObject equipmentSelectionPanel;

    [TitleGroup("장비 선택 UI")]
    [LabelText("장비 선택 슬롯 컨테이너")]
    [SerializeField] private Transform equipmentSelectionContainer;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private List<ItemSlot> _slots = new();

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private int itemCount => BattleFlowController.Instance?.playerData?.inventory?.Count ?? 0;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private EquipmentType currentSelectingSlotType;

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

        // 장비 현황 UI 찾기
        if (equipmentStatusUI == null)
        {
            equipmentStatusUI = GetComponentInChildren<EquipmentUI>(true);
        }

        // 장비 선택 패널 초기화
        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.SetActive(false);
        }
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
        foreach (var eqSlot in equipmentSlots)
        {
            if (eqSlot != null)
            {
                eqSlot.Initialize(this);
            }
        }
    }

    /// <summary>
    /// 장비 슬롯 업데이트
    /// </summary>
    private void UpdateEquipmentSlots()
    {
        var player = Utils.GetPlayer();
        if (player == null) return;

        var equipmentComponent = player.GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent == null) return;

        // 각 장비 슬롯 업데이트
        foreach (var eqSlot in equipmentSlots)
        {
            if (eqSlot != null)
            {
                var equippedItem = equipmentComponent.GetEquippedItem(eqSlot.SlotType);
                if (equippedItem != null)
                {
                    eqSlot.EquipItem(equippedItem);
                }
                else
                {
                    eqSlot.UnequipItem();
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

            // 닫을 때 모든 관련 UI 닫기
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

        if (equipmentStatusUI != null)
        {
            equipmentStatusUI.Hide();
        }

        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 장비 현황 UI 열기
    /// </summary>
    public void OpenEquipmentStatus(EquipmentType type)
    {
        var player = Utils.GetPlayer();
        if (player == null) return;

        var equipmentComponent = player.GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent == null) return;

        var equippedItem = equipmentComponent.GetEquippedItem(type);
        if (equippedItem != null && equipmentStatusUI != null)
        {
            equipmentStatusUI.ShowEquipmentStatus(equippedItem);
        }
    }

    /// <summary>
    /// 특정 타입의 장비 아이템들 보여주기
    /// </summary>
    public void ShowEquipableItemsForSlot(EquipmentType type)
    {
        if (equipmentSelectionPanel == null || equipmentSelectionContainer == null)
        {
            Debug.LogWarning("장비 선택 UI가 설정되지 않았습니다.");
            return;
        }

        currentSelectingSlotType = type;

        // 기존 선택 슬롯들 제거
        foreach (Transform child in equipmentSelectionContainer)
        {
            Destroy(child.gameObject);
        }

        // 해당 타입의 장비 아이템들 찾기
        var availableItems = GetAvailableEquipableItems(type);

        // 각 아이템에 대한 선택 슬롯 생성
        foreach (var item in availableItems)
        {
            var slotObj = Instantiate(DataCenter.Instance.GetItemSlotPrefab(), equipmentSelectionContainer);
            var itemSlot = slotObj.GetComponent<ItemSlot>();

            if (itemSlot != null)
            {
                itemSlot.SetItem(item);

                // 클릭 이벤트 오버라이드
                var button = slotObj.GetComponent<Button>();
                if (button == null)
                {
                    button = slotObj.AddComponent<Button>();
                }

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnEquipmentSelected(item));
            }
        }

        // 선택 패널 활성화
        equipmentSelectionPanel.SetActive(true);
    }

    /// <summary>
    /// 사용 가능한 장비 아이템들 가져오기
    /// </summary>
    private List<EquipableItem> GetAvailableEquipableItems(EquipmentType type)
    {
        var availableItems = new List<EquipableItem>();

        if (BattleFlowController.Instance?.playerData?.inventory != null)
        {
            foreach (var item in BattleFlowController.Instance.playerData.inventory)
            {
                if (item is EquipableItem equipableItem && equipableItem.equipmentType == type)
                {
                    availableItems.Add(equipableItem);
                }
            }
        }

        return availableItems;
    }

    /// <summary>
    /// 장비 선택 시 호출
    /// </summary>
    private void OnEquipmentSelected(EquipableItem selectedItem)
    {
        if (selectedItem == null) return;

        // 아이템 사용 (장착)
        selectedItem.Use(Utils.GetPlayer());

        // 선택 패널 닫기
        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.SetActive(false);
        }

        // UI 업데이트
        UpdateSlots();
        UpdateEquipmentSlots();
    }

    /// <summary>
    /// 기존 슬롯 업데이트 메서드
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

        // 일반 아이템들만 슬롯 생성 (장착되지 않은 아이템들)
        foreach (var item in BattleFlowController.Instance.playerData.inventory)
        {
            // 장비 아이템이면서 이미 장착된 경우 제외
            if (item is EquipableItem equipable)
            {
                var player = Utils.GetPlayer();
                var equipmentComponent = player?.GetEntityComponent<EquipmentComponent>();

                if (equipmentComponent != null &&
                    equipmentComponent.GetEquippedItem(equipable.equipmentType) == equipable)
                {
                    continue; // 이미 장착된 아이템은 일반 슬롯에 표시하지 않음
                }
            }

            var curSlot = Instantiate(DataCenter.Instance.GetItemSlotPrefab().gameObject, _slotContainer);
            var slot = curSlot.GetComponent<ItemSlot>();
            slot.SetItem(item);
            _slots.Add(slot);
        }
    }

    /// <summary>
    /// 옵저버 업데이트
    /// </summary>
    public void UpdateObserver()
    {
        UpdateSlots();
        UpdateEquipmentSlots();
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
        return equipmentSlots.FirstOrDefault(slot => slot != null && slot.SlotType == type);
    }

    /// <summary>
    /// 특정 타입의 장착된 아이템 가져오기
    /// </summary>
    public EquipableItem GetEquippedItem(EquipmentType type)
    {
        var player = Utils.GetPlayer();
        if (player == null) return null;

        var equipmentComponent = player.GetEntityComponent<EquipmentComponent>();
        return equipmentComponent?.GetEquippedItem(type);
    }

    /// <summary>
    /// 장비 선택 패널 닫기
    /// </summary>
    public void CloseEquipmentSelection()
    {
        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 디버깅용: 현재 장착 상태 출력
    /// </summary>
    [Button("현재 장착 상태 출력")]
    public void PrintEquipmentStatus()
    {
        var player = Utils.GetPlayer();
        if (player == null) return;

        var equipmentComponent = player.GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent == null) return;

        Debug.Log("=== 현재 장착 상태 ===");
        foreach (EquipmentType type in System.Enum.GetValues<EquipmentType>())
        {
            var item = equipmentComponent.GetEquippedItem(type);
            if (item != null)
            {
                Debug.Log($"{type}: {item.GetDisplayName()}");
            }
            else
            {
                Debug.Log($"{type}: 비어있음");
            }
        }
    }
}