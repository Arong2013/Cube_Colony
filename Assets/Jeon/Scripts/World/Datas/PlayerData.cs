using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class PlayerData
{
    [TitleGroup("플레이어 데이터", "인벤토리 및 스탯 정보")]
    [ReadOnly, ShowInInspector, ListDrawerSettings(ShowIndexLabels = true)]
    public List<Item> inventory { get; private set; } = new List<Item>();

    [TitleGroup("플레이어 데이터")]
    [ReadOnly, ShowInInspector]
    public EntityStat playerStat { get; private set; }

    [TitleGroup("에너지 시스템", "배터리 장비 관련")]
    [ProgressBar(0, "@maxEnergy", ColorMember = "GetEnergyColor")]
    [ShowInInspector]
    public float energy { get; private set; } = 100f;

    [TitleGroup("에너지 시스템")]
    [ShowInInspector]
    public float maxEnergy { get; private set; } = 100f;

    [TitleGroup("에너지 시스템")]
    [ReadOnly, ShowInInspector]
    public float energyRegenRate = 1f; // 초당 에너지 회복량

    [TitleGroup("장비 시스템", "장착된 장비 정보")]
    [ReadOnly, ShowInInspector]
    public Dictionary<EquipmentType, EquipableItem> equippedItems { get; private set; } = new Dictionary<EquipmentType, EquipableItem>();

    [TitleGroup("게임 화폐", "강화 및 구매용 화폐")]
    [ShowInInspector]
    public int gold { get; private set; } = 1000; // 강화용 골드

    // 인벤토리 슬롯 관련 필드
    private int _baseMaxSlot = 10; // 기본 슬롯 수
    private int _bonusSlot = 0; // 장비로 인한 추가 슬롯

    // 에너지 색상 변화 (보기만 가능)
    private Color GetEnergyColor
    {
        get
        {
            float ratio = energy / maxEnergy;
            if (ratio > 0.66f) return Color.green;
            if (ratio > 0.33f) return Color.yellow;
            return Color.red;
        }
    }

    // 초기화
    public PlayerData()
    {
        playerStat = EntityStat.CreatePlayerData();
        inventory = new List<Item>();
        energy = 100f;
        maxEnergy = 100f;
        gold = 1000;
        equippedItems = new Dictionary<EquipmentType, EquipableItem>();

        // 기본 장비 장착
        EquipDefaultItems();
    }

    // === 인벤토리 슬롯 관련 메서드 ===
    public int MaxInventorySlots => _baseMaxSlot + _bonusSlot;
    public int UsedInventorySlots => inventory.Count;
    public int AvailableInventorySlots => MaxInventorySlots - UsedInventorySlots;

    public void SetInventorySlotBonus(int bonusSlots)
    {
        _bonusSlot = Mathf.Max(0, bonusSlots);

        if (inventory.Count > MaxInventorySlots)
        {
            Debug.LogWarning($"인벤토리 용량 초과! 현재: {inventory.Count}, 최대: {MaxInventorySlots}");
        }
    }

    public void AddInventorySlotBonus(int additionalSlots)
    {
        SetInventorySlotBonus(_bonusSlot + additionalSlots);
    }

    public void RemoveInventorySlotBonus(int slotsToRemove)
    {
        SetInventorySlotBonus(_bonusSlot - slotsToRemove);
    }

    // === 장비 관련 메서드들 ===
    public void SetEquippedItem(EquipmentType slot, EquipableItem item)
    {
        if (item != null)
        {
            equippedItems[slot] = item;
            UpdateInventorySlotBonus(); // 슬롯 보너스 업데이트
        }
        else
        {
            equippedItems.Remove(slot);
            UpdateInventorySlotBonus(); // 슬롯 보너스 업데이트
        }
    }

    public EquipableItem GetEquippedItem(EquipmentType slot)
    {
        return equippedItems.TryGetValue(slot, out EquipableItem item) ? item : null;
    }

    public Dictionary<EquipmentType, EquipableItem> GetAllEquippedItems()
    {
        return new Dictionary<EquipmentType, EquipableItem>(equippedItems);
    }

    // === 아이템 관련 메서드 ===
    public bool AddItem(Item item)
    {
        if (item is ConsumableItem consumable)
        {
            return MergeConsumableItem(consumable);
        }
        
        if (inventory.Count >= MaxInventorySlots)
        {
            Debug.LogWarning("인벤토리 슬롯이 부족합니다.");
            return false;
        }

        inventory.Add(item);
        return true;
    }

    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
    }

    public bool SeparateItem(ConsumableItem consumable, int amount)
    {
        if (consumable.cunamount <= 1) return false;

        int splitAmount = Mathf.Min(amount, consumable.cunamount - 1);
        
        if (splitAmount <= 0) return false;

        var newItem = consumable.Clone() as ConsumableItem;
        if (newItem == null) return false;

        newItem.cunamount = splitAmount;
        consumable.cunamount -= splitAmount;

        if (inventory.Count >= MaxInventorySlots)
        {
            Debug.LogWarning("인벤토리 공간 부족으로 아이템 분리 실패!");
            consumable.cunamount += splitAmount;
            return false;
        }

        inventory.Add(newItem);
        return true;
    }

    public bool MergeConsumableItem(ConsumableItem newConsumable)
    {
        var existingItems = inventory
            .OfType<ConsumableItem>()
            .Where(x => x.ID == newConsumable.ID)
            .ToList();

        int remainingAmount = newConsumable.cunamount;

        foreach (var existingItem in existingItems)
        {
            if (remainingAmount <= 0) break;

            int canAdd = existingItem.maxamount - existingItem.cunamount;
            int toAdd = Mathf.Min(canAdd, remainingAmount);

            if (toAdd > 0)
            {
                existingItem.cunamount += toAdd;
                remainingAmount -= toAdd;
            }
        }

        if (remainingAmount > 0)
        {
            if (inventory.Count >= MaxInventorySlots)
            {
                Debug.LogWarning("인벤토리 슬롯이 부족하여 아이템을 추가할 수 없습니다.");
                return false;
            }

            var newItem = newConsumable.Clone() as ConsumableItem;
            if (newItem != null)
            {
                newItem.cunamount = remainingAmount;
                inventory.Add(newItem);
            }
        }

        return true;
    }

    public int GetItemCount(int itemID)
    {
        return inventory
            .Where(item => item.ID == itemID)
            .Sum(item => item is ConsumableItem consumable ? consumable.cunamount : 1);
    }

    // === 장비 장착 메서드 ===
    public bool EquipItem(EquipableItem item)
    {
        if (item == null) return false;

        // 같은 타입의 기존 아이템 해제
        if (equippedItems.ContainsKey(item.equipmentType))
        {
            UnequipItem(item.equipmentType);
        }

        // 새 아이템 장착
        equippedItems[item.equipmentType] = item;
        UpdateInventorySlotBonus();
        return true;
    }

    public EquipableItem UnequipItem(EquipmentType type)
    {
        if (!equippedItems.ContainsKey(type) || equippedItems[type] == null)
        {
            Debug.Log($"해제할 장비가 없습니다. 타입: {type}");
            return null;
        }

        EquipableItem unequippedItem = equippedItems[type];
        equippedItems[type] = null;
        UpdateInventorySlotBonus();

        return unequippedItem;
    }

    // === 인벤토리 슬롯 보너스 업데이트 ===
    private void UpdateInventorySlotBonus()
    {
        int bonusSlots = 0;
        foreach (var item in equippedItems.Values)
        {
            if (item != null)
            {
                var effects = item.GetCurrentEffects();
                bonusSlots += effects.inventorySlotBonus;
            }
        }
        SetInventorySlotBonus(bonusSlots);
    }

    // 기본 장비 장착
    private void EquipDefaultItems()
    {
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
        {
            if (type == EquipmentType.None) continue;

            var equipableItemIds = DataCenter.Instance.GetAllIds<EquipableItem>();

            if (equipableItemIds.Count > 0)
            {
                var firstItemId = equipableItemIds
                    .Select(id => DataCenter.Instance.CreateEquipableItem(id))
                    .FirstOrDefault(item => item.equipmentType == type);

                if (firstItemId != null)
                {
                    equippedItems[type] = firstItemId;
                    Debug.Log($"기본 장비 장착: {type} - {firstItemId.ItemName}");
                }
            }
        }

        UpdateInventorySlotBonus();
    }

    // === 에너지 관련 메서드 ===
    public void UpdateEnergy(float amount)
    {
        energy = Mathf.Clamp(energy + amount, 0f, maxEnergy);
    }

    public void SetEnergy(float value)
    {
        energy = Mathf.Clamp(value, 0f, maxEnergy);
    }

    public void SetMaxEnergy(float value)
    {
        maxEnergy = Mathf.Max(value, 50f);
        energy = Mathf.Min(energy, maxEnergy);
    }

    public bool HasEnoughEnergy(float required)
    {
        return energy >= required;
    }

    public bool TryConsumeEnergy(float amount)
    {
        if (HasEnoughEnergy(amount))
        {
            UpdateEnergy(-amount);
            return true;
        }
        return false;
    }

    // === 골드 관련 메서드 ===
    public void AddGold(int amount)
    {
        gold = Mathf.Max(0, gold + amount);
    }

    public bool TrySpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    public bool HasEnoughGold(int required)
    {
        return gold >= required;
    }

    public void FullReset()
    {
        playerStat = EntityStat.CreatePlayerData();
        inventory.Clear();
        SetEnergy(100f);
        SetMaxEnergy(100f);
        equippedItems.Clear();
        gold = 1000;
        EquipDefaultItems();
        _bonusSlot = 0; // 슬롯 보너스 초기화
    }
}