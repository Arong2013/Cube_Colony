using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 확장된 인벤토리 컴포넌트 (장비 효과 지원)
/// </summary>
public class ExpandedInventoryComponent : IEntityComponent
{
    private int _baseMaxSlot;
    private int _bonusSlot = 0; // 장비로 인한 추가 슬롯
    private List<Item> _Items;

    private Action<ExpandedInventoryComponent> _OnItemAdded;
    private Action<ExpandedInventoryComponent> _OnItemUsed;
    private Action<ExpandedInventoryComponent> _OnItemRemoved;

    public List<Item> items => _Items;
    public int MaxSlots => _baseMaxSlot + _bonusSlot;
    public int UsedSlots => _Items.Count;
    public int AvailableSlots => MaxSlots - UsedSlots;

    public ExpandedInventoryComponent(int baseMaxSlots = 10)
    {
        _baseMaxSlot = baseMaxSlots;
        _Items = new List<Item>();
    }

    public void Start(Entity entity) { }
    public void Update(Entity entity) { }
    public void Exit(Entity entity) { }

    /// <summary>
    /// 장비로 인한 슬롯 보너스 설정
    /// </summary>
    public void SetSlotBonus(int bonusSlots)
    {
        _bonusSlot = Math.Max(0, bonusSlots);

        // 최대 슬롯을 초과하는 아이템이 있다면 경고
        if (_Items.Count > MaxSlots)
        {
            Debug.LogWarning($"인벤토리 용량 초과! 현재: {_Items.Count}, 최대: {MaxSlots}");
        }
    }

    /// <summary>
    /// 슬롯 보너스 추가
    /// </summary>
    public void AddSlotBonus(int additionalSlots)
    {
        SetSlotBonus(_bonusSlot + additionalSlots);
    }

    /// <summary>
    /// 슬롯 보너스 제거
    /// </summary>
    public void RemoveSlotBonus(int slotsToRemove)
    {
        SetSlotBonus(_bonusSlot - slotsToRemove);
    }

    public Item GetItem(int index)
    {
        if (index >= 0 && index < _Items.Count)
        {
            return _Items[index];
        }
        return null;
    }

    public bool AddItem(Item item)
    {
        bool isAdd = true;
        if (item is ConsumableItem consumable)
        {
            isAdd = AddConsumableItem(consumable);
        }

        if (_Items.Count >= MaxSlots)
        {
            Debug.Log($"인벤토리가 가득 참! (현재: {_Items.Count}/{MaxSlots})");
            return false;
        }

        if (isAdd)
        {
            _Items.Add(item);
            _OnItemAdded?.Invoke(this);
        }
        return true;
    }

    /// <summary>
    /// 아이템 제거
    /// </summary>
    public bool RemoveItem(Item item)
    {
        if (_Items.Remove(item))
        {
            _OnItemRemoved?.Invoke(this);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 특정 인덱스의 아이템 제거
    /// </summary>
    public Item RemoveItemAt(int index)
    {
        if (index >= 0 && index < _Items.Count)
        {
            Item item = _Items[index];
            _Items.RemoveAt(index);
            _OnItemRemoved?.Invoke(this);
            return item;
        }
        return null;
    }

    private bool AddConsumableItem(ConsumableItem consumable)
    {
        var matchingItems = _Items.FindAll(x => x.ID == consumable.ID);

        foreach (var matchingItem in matchingItems)
        {
            int excess = AddAmountAndGetExcess(matchingItem as ConsumableItem, consumable.cunamount);
            SetAmount(consumable, -excess);
            if (consumable.cunamount <= 0)
            {
                return true;
            }
        }
        return consumable.cunamount > 0;
    }

    public int AddAmountAndGetExcess(ConsumableItem consumable, int amount)
    {
        int total = consumable.cunamount + amount;
        int excess = Mathf.Max(0, total - consumable.maxamount);
        SetAmount(consumable, total - excess);
        return excess;
    }

    public void SetAmount(ConsumableItem consumable, int amount)
    {
        consumable.cunamount = Mathf.Clamp(amount, 0, consumable.maxamount);
    }

    public int SeparateItem(ConsumableItem consumable, int amount)
    {
        if (consumable.cunamount <= 1) return 0;

        int splitAmount = Mathf.Min(amount, consumable.cunamount - 1);
        consumable.cunamount -= splitAmount;

        return splitAmount;
    }

    /// <summary>
    /// 인벤토리 정보 문자열 반환
    /// </summary>
    public string GetInventoryInfo()
    {
        return $"인벤토리: {UsedSlots}/{MaxSlots} (기본: {_baseMaxSlot}, 보너스: {_bonusSlot})";
    }

    /// <summary>
    /// 인벤토리가 가득 찼는지 확인
    /// </summary>
    public bool IsFull()
    {
        return _Items.Count >= MaxSlots;
    }

    /// <summary>
    /// 특정 아이템 타입의 개수 반환
    /// </summary>
    public int GetItemCount(int itemID)
    {
        int count = 0;
        foreach (var item in _Items)
        {
            if (item.ID == itemID)
            {
                if (item is ConsumableItem consumable)
                {
                    count += consumable.cunamount;
                }
                else
                {
                    count++;
                }
            }
        }
        return count;
    }

    /// <summary>
    /// 이벤트 등록
    /// </summary>
    public void RegisterEvents(
        Action<ExpandedInventoryComponent> onItemAdded = null,
        Action<ExpandedInventoryComponent> onItemUsed = null,
        Action<ExpandedInventoryComponent> onItemRemoved = null)
    {
        _OnItemAdded += onItemAdded;
        _OnItemUsed += onItemUsed;
        _OnItemRemoved += onItemRemoved;
    }
}

/// <summary>
/// 인벤토리 슬롯 UI 확장
/// </summary>
public class ExpandedInventoryUI : MonoBehaviour, IObserver
{
    [Header("UI 설정")]
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private ItemInfoUI _itemInfoUI;
    [SerializeField] private TMPro.TextMeshProUGUI _inventoryStatusText;

    [Header("슬롯 프리팹")]
    [SerializeField] private GameObject _slotPrefab;

    private List<ItemSlot> _slots = new();
    private int _lastMaxSlots = -1;

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
        UpdateSlots();
    }

    public void UpdateSlots()
    {
        if (BattleFlowController.Instance?.playerData?.inventory == null)
            return;

        // 플레이어 장비 핸들러에서 인벤토리 보너스 가져오기
        var player = Utils.GetPlayer();
        int totalSlots = 10; // 기본 슬롯

        if (player != null)
        {
            var equipmentHandler = player.GetEntityComponent<PlayerEquipmentHandler>();
            if (equipmentHandler != null)
            {
                totalSlots += equipmentHandler.GetInventorySlotBonus();
            }
        }

        // 슬롯 개수가 변경되었을 때만 재생성
        if (_lastMaxSlots != totalSlots)
        {
            CreateSlots(totalSlots);
            _lastMaxSlots = totalSlots;
        }

        // 아이템을 슬롯에 배치
        var items = BattleFlowController.Instance.playerData.inventory;
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < items.Count)
            {
                _slots[i].SetItem(items[i]);
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }

        // 상태 텍스트 업데이트
        UpdateStatusText(items.Count, totalSlots);
    }

    private void CreateSlots(int slotCount)
    {
        // 기존 슬롯 제거
        foreach (Transform child in _slotContainer)
        {
            Destroy(child.gameObject);
        }
        _slots.Clear();

        // 새 슬롯 생성
        for (int i = 0; i < slotCount; i++)
        {
            var slotObj = Instantiate(_slotPrefab, _slotContainer);
            var slot = slotObj.GetComponent<ItemSlot>();
            if (slot != null)
            {
                _slots.Add(slot);
            }
        }
    }

    private void UpdateStatusText(int usedSlots, int maxSlots)
    {
        if (_inventoryStatusText != null)
        {
            _inventoryStatusText.text = $"인벤토리: {usedSlots}/{maxSlots}";

            // 용량에 따라 색상 변경
            if (usedSlots >= maxSlots)
            {
                _inventoryStatusText.color = Color.red;
            }
            else if (usedSlots >= maxSlots * 0.8f)
            {
                _inventoryStatusText.color = Color.yellow;
            }
            else
            {
                _inventoryStatusText.color = Color.white;
            }
        }
    }

    public void UpdateObserver()
    {
        UpdateSlots();
    }

    public void ToggleInventoryUI()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            if (_itemInfoUI != null)
            {
                _itemInfoUI.Hide();
            }
        }
        else
        {
            gameObject.SetActive(true);
            UpdateSlots();
        }
    }
}