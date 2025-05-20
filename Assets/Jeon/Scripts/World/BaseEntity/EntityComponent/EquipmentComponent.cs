using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 엔티티의 장비를 관리하는 컴포넌트
/// </summary>
public class EquipmentComponent : IEntityComponent
{
    private Entity _owner;

    [ShowInInspector, ReadOnly]
    [DictionaryDrawerSettings(KeyLabel = "장비 타입", ValueLabel = "장착된 아이템")]
    private Dictionary<EquipmentType, EquipableItem> equippedItems = new Dictionary<EquipmentType, EquipableItem>();

    // 장비 효과로 인한 스탯 보너스 저장
    private Dictionary<EntityStatName, float> equipmentBonuses = new Dictionary<EntityStatName, float>();

    public IReadOnlyDictionary<EquipmentType, EquipableItem> EquippedItems => equippedItems;

    public void Start(Entity entity)
    {
        _owner = entity;
        InitializeEquipmentSlots();
    }

    public void Update(Entity entity) { }

    public void Exit(Entity entity)
    {
        // 모든 장비 효과 해제
        RemoveAllEquipmentEffects();
    }

    /// <summary>
    /// 장비 슬롯 초기화
    /// </summary>
    private void InitializeEquipmentSlots()
    {
        // 모든 장비 타입에 대해 빈 슬롯 생성
        foreach (EquipmentType type in System.Enum.GetValues<EquipmentType>())
        {
            equippedItems[type] = null;
        }
    }

    /// <summary>
    /// 아이템 장착
    /// </summary>
    public bool EquipItem(EquipableItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("장착하려는 아이템이 null입니다.");
            return false;
        }

        // 레벨 요구사항 확인
        if (!CheckLevelRequirement(item))
        {
            Debug.LogWarning($"레벨이 부족합니다. 필요 레벨: {item.requiredLevel}");
            return false;
        }

        // 기존 장착된 아이템이 있다면 해제
        if (equippedItems[item.equipmentType] != null)
        {
            UnequipItem(item.equipmentType);
        }

        // 새 아이템 장착
        equippedItems[item.equipmentType] = item;
        ApplyEquipmentEffects(item);

        Debug.Log($"{item.ItemName} 장착됨 (타입: {item.equipmentType})");

        // 플레이어에게 알림
        if (_owner is PlayerEntity player)
        {
            player.NotifyObservers();
        }

        return true;
    }

    /// <summary>
    /// 장비 해제
    /// </summary>
    public EquipableItem UnequipItem(EquipmentType type)
    {
        if (!equippedItems.ContainsKey(type) || equippedItems[type] == null)
        {
            Debug.LogWarning($"해제할 장비가 없습니다. 타입: {type}");
            return null;
        }

        EquipableItem unequippedItem = equippedItems[type];
        equippedItems[type] = null;

        // 장비 효과 제거
        RemoveEquipmentEffects(unequippedItem);

        Debug.Log($"{unequippedItem.ItemName} 해제됨 (타입: {type})");

        // 플레이어에게 알림
        if (_owner is PlayerEntity player)
        {
            player.NotifyObservers();
        }

        return unequippedItem;
    }

    /// <summary>
    /// 특정 타입의 장착된 아이템 가져오기
    /// </summary>
    public EquipableItem GetEquippedItem(EquipmentType type)
    {
        return equippedItems.TryGetValue(type, out var item) ? item : null;
    }

    /// <summary>
    /// 모든 장착된 아이템 가져오기
    /// </summary>
    public List<EquipableItem> GetAllEquippedItems()
    {
        var items = new List<EquipableItem>();
        foreach (var item in equippedItems.Values)
        {
            if (item != null)
            {
                items.Add(item);
            }
        }
        return items;
    }

    /// <summary>
    /// 특정 슬롯이 비어있는지 확인
    /// </summary>
    public bool IsSlotEmpty(EquipmentType type)
    {
        return !equippedItems.ContainsKey(type) || equippedItems[type] == null;
    }

    /// <summary>
    /// 레벨 요구사항 확인
    /// </summary>
    private bool CheckLevelRequirement(EquipableItem item)
    {
        // 임시로 레벨 체크를 HP로 대체 (추후 레벨 시스템 구현시 수정)
        float currentLevel = _owner.GetEntityStat(EntityStatName.HP) / 10f; // 예시
        return currentLevel >= item.requiredLevel;
    }

    /// <summary>
    /// 장비 효과 적용
    /// </summary>
    private void ApplyEquipmentEffects(EquipableItem item)
    {
        // 공격력 보너스 적용
        if (item.attackBonus > 0)
        {
            _owner.AddEntityStatModifier(EntityStatName.ATK, item, item.attackBonus);
            AddBonusToTracker(EntityStatName.ATK, item.attackBonus);
        }

        // 방어력 보너스 적용
        if (item.defenseBonus > 0)
        {
            _owner.AddEntityStatModifier(EntityStatName.DEF, item, item.defenseBonus);
            AddBonusToTracker(EntityStatName.DEF, item.defenseBonus);
        }

        // 체력 보너스 적용
        if (item.healthBonus > 0)
        {
            _owner.AddEntityStatModifier(EntityStatName.MaxHP, item, item.healthBonus);
            AddBonusToTracker(EntityStatName.MaxHP, item.healthBonus);
        }

        // 강화 효과 적용
        ApplyReinforcementEffects(item);
    }

    /// <summary>
    /// 장비 효과 제거
    /// </summary>
    private void RemoveEquipmentEffects(EquipableItem item)
    {
        // 공격력 보너스 제거
        if (item.attackBonus > 0)
        {
            _owner.SetEntityStatModifier(EntityStatName.ATK, item, 0);
            RemoveBonusFromTracker(EntityStatName.ATK, item.attackBonus);
        }

        // 방어력 보너스 제거
        if (item.defenseBonus > 0)
        {
            _owner.SetEntityStatModifier(EntityStatName.DEF, item, 0);
            RemoveBonusFromTracker(EntityStatName.DEF, item.defenseBonus);
        }

        // 체력 보너스 제거
        if (item.healthBonus > 0)
        {
            _owner.SetEntityStatModifier(EntityStatName.MaxHP, item, 0);
            RemoveBonusFromTracker(EntityStatName.MaxHP, item.healthBonus);
        }

        // 강화 효과 제거
        RemoveReinforcementEffects(item);
    }

    /// <summary>
    /// 강화 효과 적용
    /// </summary>
    private void ApplyReinforcementEffects(EquipableItem item)
    {
        if (item.currentReinforcementLevel <= 0) return;

        // 강화 레벨당 기본 스탯의 10% 추가 증가
        float reinforcementMultiplier = item.currentReinforcementLevel * 0.1f;

        if (item.attackBonus > 0)
        {
            float reinforcementBonus = item.attackBonus * reinforcementMultiplier;
            _owner.AddEntityStatModifier(EntityStatName.ATK, $"{item}_reinforcement", reinforcementBonus);
            AddBonusToTracker(EntityStatName.ATK, reinforcementBonus);
        }

        if (item.defenseBonus > 0)
        {
            float reinforcementBonus = item.defenseBonus * reinforcementMultiplier;
            _owner.AddEntityStatModifier(EntityStatName.DEF, $"{item}_reinforcement", reinforcementBonus);
            AddBonusToTracker(EntityStatName.DEF, reinforcementBonus);
        }

        if (item.healthBonus > 0)
        {
            float reinforcementBonus = item.healthBonus * reinforcementMultiplier;
            _owner.AddEntityStatModifier(EntityStatName.MaxHP, $"{item}_reinforcement", reinforcementBonus);
            AddBonusToTracker(EntityStatName.MaxHP, reinforcementBonus);
        }
    }

    /// <summary>
    /// 강화 효과 제거
    /// </summary>
    private void RemoveReinforcementEffects(EquipableItem item)
    {
        if (item.currentReinforcementLevel <= 0) return;

        float reinforcementMultiplier = item.currentReinforcementLevel * 0.1f;

        if (item.attackBonus > 0)
        {
            float reinforcementBonus = item.attackBonus * reinforcementMultiplier;
            _owner.SetEntityStatModifier(EntityStatName.ATK, $"{item}_reinforcement", 0);
            RemoveBonusFromTracker(EntityStatName.ATK, reinforcementBonus);
        }

        if (item.defenseBonus > 0)
        {
            float reinforcementBonus = item.defenseBonus * reinforcementMultiplier;
            _owner.SetEntityStatModifier(EntityStatName.DEF, $"{item}_reinforcement", 0);
            RemoveBonusFromTracker(EntityStatName.DEF, reinforcementBonus);
        }

        if (item.healthBonus > 0)
        {
            float reinforcementBonus = item.healthBonus * reinforcementMultiplier;
            _owner.SetEntityStatModifier(EntityStatName.MaxHP, $"{item}_reinforcement", 0);
            RemoveBonusFromTracker(EntityStatName.MaxHP, reinforcementBonus);
        }
    }

    /// <summary>
    /// 보너스 추적기에 추가
    /// </summary>
    private void AddBonusToTracker(EntityStatName stat, float bonus)
    {
        if (!equipmentBonuses.ContainsKey(stat))
            equipmentBonuses[stat] = 0;
        equipmentBonuses[stat] += bonus;
    }

    /// <summary>
    /// 보너스 추적기에서 제거
    /// </summary>
    private void RemoveBonusFromTracker(EntityStatName stat, float bonus)
    {
        if (equipmentBonuses.ContainsKey(stat))
        {
            equipmentBonuses[stat] -= bonus;
            if (equipmentBonuses[stat] <= 0)
                equipmentBonuses.Remove(stat);
        }
    }

    /// <summary>
    /// 총 장비 보너스 가져오기
    /// </summary>
    public float GetTotalEquipmentBonus(EntityStatName stat)
    {
        return equipmentBonuses.TryGetValue(stat, out var bonus) ? bonus : 0f;
    }

    /// <summary>
    /// 모든 장비 효과 제거
    /// </summary>
    private void RemoveAllEquipmentEffects()
    {
        foreach (var item in equippedItems.Values)
        {
            if (item != null)
            {
                RemoveEquipmentEffects(item);
            }
        }
        equipmentBonuses.Clear();
    }

    /// <summary>
    /// 모든 장비 효과 재적용 (강화 후 등에 사용)
    /// </summary>
    public void RefreshAllEquipmentEffects()
    {
        // 기존 효과 제거
        RemoveAllEquipmentEffects();

        // 다시 적용
        foreach (var item in equippedItems.Values)
        {
            if (item != null)
            {
                ApplyEquipmentEffects(item);
            }
        }
    }

    /// <summary>
    /// 장비 상태 정보 가져오기 (디버그용)
    /// </summary>
    [Button("장비 상태 출력")]
    public void PrintEquipmentStatus()
    {
        Debug.Log("=== 현재 장착 상태 ===");
        foreach (var kvp in equippedItems)
        {
            if (kvp.Value != null)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value.ItemName} (+{kvp.Value.currentReinforcementLevel})");
            }
            else
            {
                Debug.Log($"{kvp.Key}: 비어있음");
            }
        }

        Debug.Log("=== 장비 보너스 ===");
        foreach (var kvp in equipmentBonuses)
        {
            Debug.Log($"{kvp.Key}: +{kvp.Value}");
        }
    }
}