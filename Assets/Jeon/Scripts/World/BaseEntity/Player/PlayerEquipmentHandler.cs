using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

/// <summary>
/// 플레이어의 장비 관리를 담당하는 컴포넌트
/// </summary>
public class PlayerEquipmentHandler : IEntityComponent
{
    private Entity _entity;
    private Dictionary<EquipmentType, EquipableItem> equippedItems;
    private EquipmentEffects totalEffects;

    public event Action<EquipableItem> OnItemEquipped;
    public event Action<EquipableItem> OnItemUnequipped;
    public event Action OnEquipmentChanged;

    public PlayerEquipmentHandler()
    {
        equippedItems = new Dictionary<EquipmentType, EquipableItem>();
        totalEffects = new EquipmentEffects();
    }

    public void Start(Entity entity)
    {
        _entity = entity;
    }

    public void Update(Entity entity) { }

    public void Exit(Entity entity)
    {
        // 모든 장비 효과 제거
        foreach (var item in equippedItems.Values)
        {
            RemoveEquipmentEffects(item);
        }
        equippedItems.Clear();
    }

    /// <summary>
    /// 아이템 장착
    /// </summary>
    public bool EquipItem(EquipableItem item)
    {
        if (item == null) return false;

        EquipmentType slot = item.equipmentType;

        // 기존 장비가 있다면 해제
        if (equippedItems.ContainsKey(slot))
        {
            UnequipItem(slot);
        }

        // 새 장비 장착
        equippedItems[slot] = item;
        ApplyEquipmentEffects(item);

        // 인벤토리에서 제거
        if (_entity is PlayerEntity player)
        {
            BattleFlowController.Instance.playerData.RemoveItem(item);
            player.NotifyObservers();
        }

        OnItemEquipped?.Invoke(item);
        OnEquipmentChanged?.Invoke();

        Debug.Log($"{item.GetDisplayName()}을(를) 장착했습니다!");
        return true;
    }

    /// <summary>
    /// 아이템 해제
    /// </summary>
    public bool UnequipItem(EquipmentType slot)
    {
        if (!equippedItems.ContainsKey(slot)) return false;

        EquipableItem item = equippedItems[slot];
        equippedItems.Remove(slot);
        RemoveEquipmentEffects(item);

        // 인벤토리에 다시 추가
        if (_entity is PlayerEntity player)
        {
            BattleFlowController.Instance.playerData.AddItem(item);
            player.NotifyObservers();
        }

        OnItemUnequipped?.Invoke(item);
        OnEquipmentChanged?.Invoke();

        Debug.Log($"{item.GetDisplayName()}을(를) 해제했습니다!");
        return true;
    }

    /// <summary>
    /// 특정 아이템이 장착되어 있는지 확인
    /// </summary>
    public bool IsEquipped(EquipableItem item)
    {
        foreach (var equippedItem in equippedItems.Values)
        {
            if (equippedItem == item) return true;
        }
        return false;
    }

    /// <summary>
    /// 특정 슬롯에 장착된 아이템 반환
    /// </summary>
    public EquipableItem GetEquippedItem(EquipmentType slot)
    {
        return equippedItems.TryGetValue(slot, out EquipableItem item) ? item : null;
    }

    /// <summary>
    /// 모든 장착된 아이템 반환
    /// </summary>
    public Dictionary<EquipmentType, EquipableItem> GetAllEquippedItems()
    {
        return new Dictionary<EquipmentType, EquipableItem>(equippedItems);
    }

    /// <summary>
    /// 장비 효과 적용
    /// </summary>
    private void ApplyEquipmentEffects(EquipableItem item)
    {
        if (item == null) return;

        // 아이템의 현재 효과 가져오기
        var effects = item.GetCurrentEffects();

        // 공격력 보너스 적용
        if (effects.attackBonus > 0)
        {
            _entity.AddEntityStatModifier(EntityStatName.ATK, item, effects.attackBonus);
            AddStatBonus(EntityStatName.ATK, effects.attackBonus);
        }

        // 방어력 보너스 적용
        if (effects.defenseBonus > 0)
        {
            _entity.AddEntityStatModifier(EntityStatName.DEF, item, effects.defenseBonus);
            AddStatBonus(EntityStatName.DEF, effects.defenseBonus);
        }

        // 체력 보너스 적용
        if (effects.healthBonus > 0)
        {
            _entity.AddEntityStatModifier(EntityStatName.MaxHP, item, effects.healthBonus);
            AddStatBonus(EntityStatName.MaxHP, effects.healthBonus);
        }

        // 산소 보너스 적용
        if (effects.maxOxygenBonus > 0)
        {
            _entity.AddEntityStatModifier(EntityStatName.MaxO2, item, effects.maxOxygenBonus);
            AddStatBonus(EntityStatName.MaxO2, effects.maxOxygenBonus);
        }

        // 에너지 보너스 적용
        if (effects.maxEnergyBonus > 0)
        {
            // 에너지는 PlayerData에서 관리하므로 별도 처리 필요
            ApplyEnergyBonus(effects.maxEnergyBonus);
        }

        // 특수 효과는 totalEffects에 누적
        totalEffects += effects;

        // 인벤토리 확장 (가방)
        if (effects.inventorySlotBonus > 0)
        {
            ApplyInventoryExpansion(effects.inventorySlotBonus);
        }
    }

    /// <summary>
    /// 장비 효과 제거
    /// </summary>
    private void RemoveEquipmentEffects(EquipableItem item)
    {
        var effects = item.GetCurrentEffects();

        // 스탯 보너스 제거
        _entity.SetEntityStatModifier(EntityStatName.ATK, item, 0);
        _entity.SetEntityStatModifier(EntityStatName.DEF, item, 0);
        _entity.SetEntityStatModifier(EntityStatName.MaxHP, item, 0);
        _entity.SetEntityStatModifier(EntityStatName.MaxO2, item, 0);

        // 에너지 보너스 제거
        if (_entity is PlayerEntity player)
        {
            RemoveEnergyBonus(effects.maxEnergyBonus);
        }

        // 총 효과 재계산
        RecalculateTotalEffects();

        // 인벤토리 축소 (가방)
        if (effects.inventorySlotBonus > 0)
        {
            RemoveInventoryExpansion(effects.inventorySlotBonus);
        }
    }

    /// <summary>
    /// 전체 장비 효과 새로고침
    /// </summary>
    public void RefreshEquipmentEffects()
    {
        // 모든 장비 효과 제거 후 재적용
        var tempEquipped = new Dictionary<EquipmentType, EquipableItem>(equippedItems);

        foreach (var item in tempEquipped.Values)
        {
            RemoveEquipmentEffects(item);
        }

        foreach (var item in tempEquipped.Values)
        {
            ApplyEquipmentEffects(item);
        }
    }

    /// <summary>
    /// 총 효과 재계산
    /// </summary>
    private void RecalculateTotalEffects()
    {
        totalEffects = new EquipmentEffects();

        foreach (var item in equippedItems.Values)
        {
            if (item != null)
            {
                totalEffects += item.GetCurrentEffects();
            }
        }
    }

    /// <summary>
    /// 에너지 보너스 적용 (PlayerData의 maxEnergy 확장)
    /// </summary>
    private void ApplyEnergyBonus(float bonus)
    {
        // 실제 구현에서는 PlayerData에 maxEnergy 필드를 추가해야 함
        // 여기서는 임시로 로그만 출력
        Debug.Log($"에너지 최대치 +{bonus} 적용됨");
    }

    /// <summary>
    /// 에너지 보너스 제거
    /// </summary>
    private void RemoveEnergyBonus(float bonus)
    {
        Debug.Log($"에너지 최대치 +{bonus} 제거됨");
    }

    /// <summary>
    /// 인벤토리 확장 적용
    /// </summary>
    private void ApplyInventoryExpansion(int slotCount)
    {
        // InventoryComponent의 최대 슬롯 수 증가
        // 실제 구현에서는 InventoryComponent를 확장해야 함
        Debug.Log($"인벤토리 슬롯 +{slotCount} 추가됨");
    }

    /// <summary>
    /// 인벤토리 확장 제거
    /// </summary>
    private void RemoveInventoryExpansion(int slotCount)
    {
        Debug.Log($"인벤토리 슬롯 +{slotCount} 제거됨");
    }

    /// <summary>
    /// 현재 총 장비 효과 반환
    /// </summary>
    public EquipmentEffects GetTotalEffects()
    {
        return totalEffects;
    }

    /// <summary>
    /// 공격 시 추가 타격 처리 (검용)
    /// </summary>
    public void ProcessExtraHits(Entity target)
    {
        if (totalEffects.extraHitCount > 0 && target != null)
        {
            // 추가 타격 처리
            float attackDamage = _entity.GetEntityStat(EntityStatName.ATK);

            for (int i = 0; i < totalEffects.extraHitCount; i++)
            {
                target.TakeDamage(attackDamage);
                Debug.Log($"추가 타격! (+{i + 1})");
            }
        }
    }

    /// <summary>
    /// 연사속도 보너스 반환 (총용)
    /// </summary>
    public float GetFireRateBonus()
    {
        return totalEffects.fireRateBonus;
    }

    /// <summary>
    /// 산소 소모 감소량 반환
    /// </summary>
    public float GetOxygenConsumptionReduction()
    {
        return totalEffects.oxygenConsumptionReduction;
    }

    /// <summary>
    /// 에너지 소모 감소량 반환
    /// </summary>
    public float GetEnergyConsumptionReduction()
    {
        return totalEffects.energyConsumptionReduction;
    }

    /// <summary>
    /// 피해 감소량 반환 (헬멧용)
    /// </summary>
    public float GetDamageReduction()
    {
        return totalEffects.damageReduction;
    }

    /// <summary>
    /// 인벤토리 슬롯 보너스 반환
    /// </summary>
    public int GetInventorySlotBonus()
    {
        return totalEffects.inventorySlotBonus;
    }

    /// <summary>
    /// 스탯 보너스 딕셔너리 반환
    /// </summary>
    public Dictionary<EntityStatName, float> GetStatBonuses()
    {
        return new Dictionary<EntityStatName, float>(statBonuses);
    }

    // 스탯 보너스 추가를 위한 helper 메서드 (클래스 내에 추가 필요)
    private Dictionary<EntityStatName, float> statBonuses = new Dictionary<EntityStatName, float>();

    private void AddStatBonus(EntityStatName stat, float bonus)
    {
        if (!statBonuses.ContainsKey(stat))
            statBonuses[stat] = 0;

        statBonuses[stat] += bonus;
    }

    private void RemoveStatBonus(EntityStatName stat, float bonus)
    {
        if (statBonuses.ContainsKey(stat))
        {
            statBonuses[stat] -= bonus;
            if (statBonuses[stat] <= 0)
                statBonuses.Remove(stat);
        }
    }
}