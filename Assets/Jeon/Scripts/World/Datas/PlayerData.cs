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

    // 장비 관련 메서드들
    public void SetEquippedItem(EquipmentType slot, EquipableItem item)
    {
        if (item != null)
        {
            equippedItems[slot] = item;
        }
        else
        {
            equippedItems.Remove(slot);
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

    // 장비 장착
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
        return true;
    }

    // 장비 해제
    public EquipableItem UnequipItem(EquipmentType type)
    {
        if (!equippedItems.ContainsKey(type) || equippedItems[type] == null)
        {
            Debug.Log($"해제할 장비가 없습니다. 타입: {type}");
            return null;
        }

        EquipableItem unequippedItem = equippedItems[type];
        equippedItems[type] = null;

        return unequippedItem;
    }

    // 기본 장비 장착
    private void EquipDefaultItems()
    {
        // 각 장비 타입별로 첫 번째 아이템 찾아 장착
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
        {
            if (type == EquipmentType.None) continue;

            // DataCenter에서 해당 타입의 첫 번째 아이템 찾기
            var equipableItemIds = DataCenter.Instance.GetAllIds<EquipableItem>();

            if (equipableItemIds.Count > 0)
            {
                var firstItemId = equipableItemIds
                    .Select(id => DataCenter.Instance.CreateEquipableItem(id))
                    .FirstOrDefault(item => item.equipmentType == type);

                if (firstItemId != null)
                {
                    // 아이템 장착
                    equippedItems[type] = firstItemId;
                    Debug.Log($"기본 장비 장착: {type} - {firstItemId.ItemName}");
                }
            }
        }
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
        maxEnergy = Mathf.Max(value, 50f); // 최소 50
        energy = Mathf.Min(energy, maxEnergy); // 현재 에너지가 최대를 초과하지 않도록
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


    // === 인벤토리 관련 메서드 ===

    public void AddItem(Item item)
    {
        inventory.Add(item);
    }

    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
    }

    public void ClearInventory()
    {
        inventory.Clear();
    }

    public int GetItemCount(int itemID)
    {
        int count = 0;
        foreach (var item in inventory)
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

    // === 화폐 관련 메서드 ===

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

    // === 게임 리셋 메서드 ===

public void Reset()
{
    playerStat = EntityStat.CreatePlayerData();
    SetEnergy(100f);
    SetMaxEnergy(100f);
}

public void FullReset()
{
    playerStat = EntityStat.CreatePlayerData();
    ClearInventory();
    SetEnergy(100f);
    SetMaxEnergy(100f);
    equippedItems.Clear();
    gold = 1000; // 초기 골드로 리셋
    EquipDefaultItems(); // 기본 장비 다시 장착
}

    
}