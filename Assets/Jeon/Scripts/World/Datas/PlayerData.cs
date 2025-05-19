using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 플레이어 데이터 클래스 (장비 시스템 및 에너지 시스템 포함)
/// </summary>
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
    public Dictionary<EquipmentSlot, EquipableItem> equippedItems { get; private set; } = new Dictionary<EquipmentSlot, EquipableItem>();

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
        equippedItems = new Dictionary<EquipmentSlot, EquipableItem>();
    }

    // === 에너지 관련 메서드 ===

    /// <summary>
    /// 에너지 업데이트 (회복/소모)
    /// </summary>
    public void UpdateEnergy(float amount)
    {
        energy = Mathf.Clamp(energy + amount, 0f, maxEnergy);
    }

    /// <summary>
    /// 에너지 직접 설정
    /// </summary>
    public void SetEnergy(float value)
    {
        energy = Mathf.Clamp(value, 0f, maxEnergy);
    }

    /// <summary>
    /// 최대 에너지 설정
    /// </summary>
    public void SetMaxEnergy(float value)
    {
        maxEnergy = Mathf.Max(value, 50f); // 최소 50
        energy = Mathf.Min(energy, maxEnergy); // 현재 에너지가 최대를 초과하지 않도록
    }

    /// <summary>
    /// 에너지가 충분한지 확인
    /// </summary>
    public bool HasEnoughEnergy(float required)
    {
        return energy >= required;
    }

    /// <summary>
    /// 에너지 소모 (실패 시 false 반환)
    /// </summary>
    public bool TryConsumeEnergy(float amount)
    {
        if (HasEnoughEnergy(amount))
        {
            UpdateEnergy(-amount);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 에너지 회복 (시간 경과)
    /// </summary>
    public void RegenerateEnergy(float deltaTime)
    {
        if (energy < maxEnergy)
        {
            UpdateEnergy(energyRegenRate * deltaTime);
        }
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

    /// <summary>
    /// 특정 아이템 개수 확인
    /// </summary>
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

    // === 장비 관련 메서드 ===

    /// <summary>
    /// 장비 장착 정보 업데이트
    /// </summary>
    public void SetEquippedItem(EquipmentSlot slot, EquipableItem item)
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

    /// <summary>
    /// 장착된 장비 반환
    /// </summary>
    public EquipableItem GetEquippedItem(EquipmentSlot slot)
    {
        return equippedItems.TryGetValue(slot, out EquipableItem item) ? item : null;
    }

    /// <summary>
    /// 모든 장착된 장비 반환
    /// </summary>
    public Dictionary<EquipmentSlot, EquipableItem> GetAllEquippedItems()
    {
        return new Dictionary<EquipmentSlot, EquipableItem>(equippedItems);
    }

    // === 화폐 관련 메서드 ===

    /// <summary>
    /// 골드 추가
    /// </summary>
    public void AddGold(int amount)
    {
        gold = Mathf.Max(0, gold + amount);
    }

    /// <summary>
    /// 골드 소모 (실패 시 false 반환)
    /// </summary>
    public bool TrySpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 골드가 충분한지 확인
    /// </summary>
    public bool HasEnoughGold(int required)
    {
        return gold >= required;
    }

    // === 게임 리셋 메서드 ===

    /// <summary>
    /// 플레이어 사망 시 초기화
    /// </summary>
    public void Reset()
    {
        playerStat = EntityStat.CreatePlayerData();
        ClearInventory();
        SetEnergy(100f);
        SetMaxEnergy(100f);
        equippedItems.Clear();
        // 골드는 유지 (선택사항)
    }

    /// <summary>
    /// 게임 완전 리셋
    /// </summary>
    public void FullReset()
    {
        Reset();
        gold = 1000; // 초기 골드
    }

    // === 디버그 메서드 ===

    [Button("에너지 회복"), GUIColor(0.3f, 0.8f, 0.3f)]
    public void DebugRestoreEnergy()
    {
        SetEnergy(maxEnergy);
        Debug.Log("에너지 완전 회복!");
    }

    [Button("골드 추가 (+500)"), GUIColor(0.8f, 0.8f, 0.3f)]
    public void DebugAddGold()
    {
        AddGold(500);
        Debug.Log($"골드 500 추가! 현재: {gold}");
    }

    [Button("장비 정보 출력"), GUIColor(0.3f, 0.3f, 0.8f)]
    public void DebugPrintEquipment()
    {
        Debug.Log("=== 장착된 장비 ===");
        foreach (var kvp in equippedItems)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value.GetDisplayName()}");
        }
        if (equippedItems.Count == 0)
        {
            Debug.Log("장착된 장비 없음");
        }
    }
}