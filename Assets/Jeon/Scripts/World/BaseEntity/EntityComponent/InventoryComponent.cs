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

    /// <summary>
    /// 아이템을 분리하여 새로운 아이템 인스턴스를 생성하고 인벤토리에 추가
    /// </summary>
    /// <param name="consumable">분리할 아이템</param>
    /// <param name="amount">분리할 수량</param>
    /// <returns>성공 여부</returns>
    public bool SeparateItem(ConsumableItem consumable, int amount)
    {
        // 1개 이하면 분리 불가
        if (consumable.cunamount <= 1)
            return false;

        // 남겨야 할 최소 1개를 고려해서 분리 가능한 최대 수량 계산
        int splitAmount = Mathf.Min(amount, consumable.cunamount - 1);
        
        if (splitAmount <= 0)
            return false;

        // 새 아이템 인스턴스 생성 (Clone을 통해)
        var newItem = consumable.Clone() as ConsumableItem;
        if (newItem == null)
            return false;

        // 새 아이템의 수량 설정
        newItem.cunamount = splitAmount;
        
        // 원래 아이템의 수량 감소
        consumable.cunamount -= splitAmount;

        // 인벤토리에 충분한 공간이 있는지 확인
        if (_Items.Count >= MaxSlots)
        {
            Debug.LogWarning($"인벤토리 공간 부족으로 아이템 분리 실패!");
            // 원래 아이템의 수량 복구
            consumable.cunamount += splitAmount;
            return false;
        }

        // 새 아이템을 인벤토리에 추가
        _Items.Add(newItem);
        _OnItemAdded?.Invoke(this);
        
        Debug.Log($"아이템 분리 성공: {consumable.ItemName} x{splitAmount}");
        return true;
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