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

    [ShowInInspector, ReadOnly]
    private EquipmentEffects totalEffects; // 장비 효과 합계

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
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
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
        if (item == null) return;

        var effects = item.GetCurrentEffects();

        // 공격력 적용
        if (effects.attackBonus > 0)
        {
            _owner.AddEntityStatModifier(EntityStatName.ATK, item, effects.attackBonus);
            AddBonusToTracker(EntityStatName.ATK, effects.attackBonus);
        }

        // 방어력 적용
        if (effects.defenseBonus > 0)
        {
            _owner.AddEntityStatModifier(EntityStatName.DEF, item, effects.defenseBonus);
            AddBonusToTracker(EntityStatName.DEF, effects.defenseBonus);
        }

        // 체력 보너스 적용
        if (effects.healthBonus > 0)
        {
            _owner.AddEntityStatModifier(EntityStatName.MaxHP, item, effects.healthBonus);
            AddBonusToTracker(EntityStatName.MaxHP, effects.healthBonus);
        }

        // 산소 보너스 적용
        if (effects.maxOxygenBonus > 0)
        {
            _owner.AddEntityStatModifier(EntityStatName.MaxO2, item, effects.maxOxygenBonus);
            AddBonusToTracker(EntityStatName.MaxO2, effects.maxOxygenBonus);
        }

        // 에너지 보너스 적용
        if (effects.maxEnergyBonus > 0)
        {
            ApplyEnergyBonus(effects.maxEnergyBonus);
        }

        // 특수 효과 적용 (장비 타입별)
        ApplySpecialEffects(item, effects);

        // 총 효과 업데이트
        totalEffects = GetTotalEquipmentEffects();
    }

    /// <summary>
    /// 모든 장비 효과의 합계 계산
    /// </summary>
    private EquipmentEffects GetTotalEquipmentEffects()
    {
        EquipmentEffects total = new EquipmentEffects();
        
        foreach (var item in equippedItems.Values)
        {
            if (item != null)
            {
                var effects = item.GetCurrentEffects();
                total.attackBonus += effects.attackBonus;
                total.defenseBonus += effects.defenseBonus;
                total.healthBonus += effects.healthBonus;
                total.maxOxygenBonus += effects.maxOxygenBonus;
                total.maxEnergyBonus += effects.maxEnergyBonus;
                total.extraHitCount += effects.extraHitCount;
                total.fireRateBonus += effects.fireRateBonus;
                total.oxygenConsumptionReduction += effects.oxygenConsumptionReduction;
                total.energyConsumptionReduction += effects.energyConsumptionReduction;
                total.inventorySlotBonus += effects.inventorySlotBonus;
                total.damageReduction += effects.damageReduction;
            }
        }
        
        return total;
    }

    /// <summary>
    /// 특수 효과 적용 (장비 타입별)
    /// </summary>
    private void ApplySpecialEffects(EquipableItem item, EquipmentEffects effects)
    {
        switch (item.equipmentType)
        {
            case EquipmentType.Sword:
                // 추가 타격 횟수
                if (effects.extraHitCount > 0)
                {
                    // 직접 효과를 저장 (totalEffects는 나중에 GetTotalEquipmentEffects에서 계산됨)
                    // 별도의 PlayerEntity 메서드 호출 없음
                }
                break;

            case EquipmentType.Gun:
                // 연사 속도 증가
                if (effects.fireRateBonus > 0)
                {
                    // 직접 효과를 저장 (totalEffects는 나중에 GetTotalEquipmentEffects에서 계산됨)
                    // 별도의 PlayerEntity 메서드 호출 없음
                }
                break;

            case EquipmentType.OxygenTank:
                // 산소통 효과
                if (effects.maxOxygenBonus > 0)
                {
                    _owner.AddEntityStatModifier(EntityStatName.MaxO2, item, effects.maxOxygenBonus);
                    AddBonusToTracker(EntityStatName.MaxO2, effects.maxOxygenBonus);
                }

                if (effects.oxygenConsumptionReduction > 0)
                {
                    // 직접 효과를 저장 (totalEffects는 나중에 GetTotalEquipmentEffects에서 계산됨)
                    // 별도의 PlayerEntity 메서드 호출 없음
                }
                break;

            case EquipmentType.Battery:
                // 배터리 효과
                if (effects.maxEnergyBonus > 0)
                {
                    _owner.AddEntityStatModifier(EntityStatName.MaxEng, item, effects.maxEnergyBonus);
                    AddBonusToTracker(EntityStatName.MaxEng, effects.maxEnergyBonus);
                }

                if (effects.energyConsumptionReduction > 0)
                {
                    // 직접 효과를 저장 (totalEffects는 나중에 GetTotalEquipmentEffects에서 계산됨)
                    // 별도의 PlayerEntity 메서드 호출 없음
                }
                break;

            case EquipmentType.Backpack:
                // 인벤토리 슬롯 증가
                if (effects.inventorySlotBonus > 0)
                {
                    if (_owner is PlayerEntity playerEntity)
                    {
                        var inventoryComponent = playerEntity.GetEntityComponent<ExpandedInventoryComponent>();
                        if (inventoryComponent != null)
                        {
                            inventoryComponent.AddSlotBonus(effects.inventorySlotBonus);
                        }
                        else
                        {
                            Debug.LogWarning("ExpandedInventoryComponent를 찾을 수 없습니다.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("PlayerEntity가 아닌 엔티티에 인벤토리 확장을 시도했습니다.");
                    }
                }
                break;

            case EquipmentType.Helmet:
                // 피해 감소
                if (effects.damageReduction > 0)
                {
                    // 직접 효과를 저장 (totalEffects는 나중에 GetTotalEquipmentEffects에서 계산됨)
                    // 별도의 PlayerEntity 메서드 호출 없음
                }
                break;
        }
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

        // 산소 보너스 제거
        if (item.maxOxygenBonus > 0)
        {
            _owner.SetEntityStatModifier(EntityStatName.MaxO2, item, 0);
            RemoveBonusFromTracker(EntityStatName.MaxO2, item.maxOxygenBonus);
        }

        // 에너지 보너스 제거
        if (item.maxEnergyBonus > 0)
        {
            RemoveEnergyBonus(item.maxEnergyBonus);
        }

        // 특수 효과 제거
        RemoveSpecialEffects(item);

        // 강화 효과 제거
        RemoveReinforcementEffects(item);

        // 총 효과 업데이트
        totalEffects = GetTotalEquipmentEffects();
    }

    /// <summary>
    /// 특수 효과 제거
    /// </summary>
    private void RemoveSpecialEffects(EquipableItem item)
    {
        // 특수 효과들은 GetTotalEquipmentEffects()에서 계산되므로
        // 아이템을 제거하면 자동으로 totalEffects에서 제외됩니다.
        // ExpandInventory는 예외적으로 직접 호출합니다.
        if (_owner is PlayerEntity playerEntity && item.equipmentType == EquipmentType.Backpack && item.inventorySlotBonus > 0)
        {
            var inventoryComponent = playerEntity.GetEntityComponent<ExpandedInventoryComponent>();
            if (inventoryComponent != null)
            {
                inventoryComponent.RemoveSlotBonus(item.inventorySlotBonus);
            }
            else
            {
                Debug.LogWarning("ExpandedInventoryComponent를 찾을 수 없습니다.");
            }
        }
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
    /// 에너지 보너스 적용
    /// </summary>
    private void ApplyEnergyBonus(float bonus)
    {
        if (BattleFlowController.Instance?.playerData != null)
        {
            try
            {
                float currentMaxEnergy = BattleFlowController.Instance.playerData.maxEnergy;
                BattleFlowController.Instance.playerData.SetMaxEnergy(currentMaxEnergy + bonus);
                BattleFlowController.Instance.NotifyObservers();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"에너지 보너스 적용 중 오류 발생: {e.Message}");
                
                // 대체 구현: playerData 클래스에 올바른 접근자/프로퍼티 사용
                BattleFlowController.Instance.playerData.SetMaxEnergy(
                    BattleFlowController.Instance.playerData.maxEnergy + bonus);
                BattleFlowController.Instance.NotifyObservers();
            }
        }
    }
    
    /// <summary>
    /// 에너지 보너스 제거
    /// </summary>
    private void RemoveEnergyBonus(float bonus)
    {
        if (BattleFlowController.Instance?.playerData != null)
        {
            try
            {
                float currentMaxEnergy = BattleFlowController.Instance.playerData.maxEnergy;
                BattleFlowController.Instance.playerData.SetMaxEnergy(currentMaxEnergy - bonus);
                BattleFlowController.Instance.NotifyObservers();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"에너지 보너스 제거 중 오류 발생: {e.Message}");
                
                // 대체 구현: playerData 클래스에 올바른 접근자/프로퍼티 사용
                BattleFlowController.Instance.playerData.SetMaxEnergy(
                    BattleFlowController.Instance.playerData.maxEnergy - bonus);
                BattleFlowController.Instance.NotifyObservers();
            }
        }
    }

    /// <summary>
    /// 인벤토리 확장 적용
    /// </summary>
    private void ApplyInventoryExpansion(int slotBonus)
    {
        if (_owner is PlayerEntity playerEntity)
        {
            var inventoryComponent = playerEntity.GetEntityComponent<ExpandedInventoryComponent>();
            if (inventoryComponent != null)
            {
                inventoryComponent.AddSlotBonus(slotBonus);
            }
            else
            {
                Debug.LogWarning("ExpandedInventoryComponent를 찾을 수 없습니다.");
            }
        }
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

    /// <summary>
    /// 총 효과 가져오기
    /// </summary>
    public EquipmentEffects GetTotalEffects()
    {
        return totalEffects;
    }
}