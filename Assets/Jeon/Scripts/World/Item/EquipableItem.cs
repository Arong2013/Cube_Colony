using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

// 확장된 장비 타입
public enum EquipmentType
{
    Sword,      // 검
    Gun,        // 총
    OxygenTank, // 산소통
    Battery,    // 배터리
    Backpack,   // 가방
    Helmet,     // 헬멧
    None        // 장비하지 않음
}
[System.Serializable]
public class EquipableItem : Item
{
    // SO에서 가져온 기본 정보들
    [ShowInInspector] public EquipmentType equipmentType;
    [ShowInInspector] public int requiredLevel;
    [ShowInInspector] public float attackBonus;
    [ShowInInspector] public float defenseBonus;
    [ShowInInspector] public float healthBonus;
    [ShowInInspector] public string description;
    [ShowInInspector] public ItemGrade grade;
    // 추가 필요한 필드들
    [TitleGroup("추가 효과")]
    [ShowInInspector] public float maxOxygenBonus = 0f;
    [ShowInInspector] public float maxEnergyBonus = 0f;
    [ShowInInspector] public int extraHitCount = 0;
    [ShowInInspector] public float fireRateBonus = 0f;
    [ShowInInspector] public float oxygenConsumptionReduction = 0f;
    [ShowInInspector] public float energyConsumptionReduction = 0f;
    [ShowInInspector] public int inventorySlotBonus = 0;
    [ShowInInspector] public float damageReduction = 0f;

    // 강화 관련 필드들
    [TitleGroup("강화 시스템")]
    [ShowInInspector]
    [LabelText("현재 강화 레벨")]
    public int currentReinforcementLevel = 0;

    [TitleGroup("강화 시스템")]
    [ShowInInspector]
    [LabelText("최대 강화 레벨")]
    public int maxReinforcementLevel = 10;

    [TitleGroup("강화 시스템")]
    [ShowInInspector]
    [LabelText("특수 효과 1")]
    [Tooltip("아이템별 특수 효과 (추후 확장 가능)")]
    public float specialEffect1 = 0f;

    [TitleGroup("강화 시스템")]
    [ShowInInspector]
    [LabelText("특수 효과 2")]
    [Tooltip("아이템별 특수 효과 (추후 확장 가능)")]
    public float specialEffect2 = 0f;


    [TitleGroup("강화 시스템")]
    [ShowInInspector]
    [LabelText("강화 레시피 ID")]
    public int reinforcementRecipeId; // 강화 테이블 ID 추가

        private ReinforcementRecipeSO GetReinforcementRecipe()
    {
        return DataCenter.Instance.GetReinforcementRecipeSO(reinforcementRecipeId);
    }


    /// <summary>
    /// 아이템이 강화 가능한지 확인
    /// </summary>
    public bool CanReinforce()
    {
        return currentReinforcementLevel < maxReinforcementLevel;
    }

    /// <summary>
    /// 강화 실행
    /// </summary>
    public bool Reinforce()
    {
        if (!CanReinforce())
        {
            Debug.LogWarning($"{ItemName}은 더 이상 강화할 수 없습니다. (최대 레벨: {maxReinforcementLevel})");
            return false;
        }

        currentReinforcementLevel++;
        Debug.Log($"{ItemName} 강화 완료! 현재 레벨: +{currentReinforcementLevel}");
        return true;
    }

    /// <summary>
    /// 강화 성공 확률 가져오기
    /// </summary>
    public float GetReinforcementSuccessRate()
    {
        if (!CanReinforce())
            return 0f;

        // 기본 성공률 계산 (레벨이 높을수록 낮아짐)
        float baseRate = 100f;
        float reductionPerLevel = 5f;
        float successRate = baseRate - (currentReinforcementLevel * reductionPerLevel);

        // 최소 성공률 보장
        float minRate = 10f;
        return Mathf.Max(successRate, minRate);
    }

    /// <summary>
    /// 총 공격력 보너스 계산 (기본 + 강화)
    /// </summary>
    public float GetTotalAttackBonus()
    {
        return GetCurrentEffects().attackBonus;
    }

    /// <summary>
    /// 특정 레벨에서의 총 공격력 보너스 계산
    /// </summary>
    public float GetTotalAttackBonusAtLevel(int level)
    {
        // 현재 레벨 저장
        int currentLevel = currentReinforcementLevel;
        
        // 임시로 레벨 변경
        currentReinforcementLevel = level;
        
        // 효과 계산
        float result = GetCurrentEffects().attackBonus;
        
        // 원래 레벨로 복원
        currentReinforcementLevel = currentLevel;
        
        return result;
    }

    /// <summary>
    /// 총 방어력 보너스 계산 (기본 + 강화)
    /// </summary>
    public float GetTotalDefenseBonus()
    {
        return GetCurrentEffects().defenseBonus;
    }

    /// <summary>
    /// 특정 레벨에서의 총 방어력 보너스 계산
    /// </summary>
    public float GetTotalDefenseBonusAtLevel(int level)
    {
        // 현재 레벨 저장
        int currentLevel = currentReinforcementLevel;
        
        // 임시로 레벨 변경
        currentReinforcementLevel = level;
        
        // 효과 계산
        float result = GetCurrentEffects().defenseBonus;
        
        // 원래 레벨로 복원
        currentReinforcementLevel = currentLevel;
        
        return result;
    }

    /// <summary>
    /// 총 체력 보너스 계산 (기본 + 강화)
    /// </summary>
    public float GetTotalHealthBonus()
    {
        return GetCurrentEffects().healthBonus;
    }

    /// <summary>
    /// 특정 레벨에서의 총 체력 보너스 계산
    /// </summary>
    public float GetTotalHealthBonusAtLevel(int level)
    {
        // 현재 레벨 저장
        int currentLevel = currentReinforcementLevel;
        
        // 임시로 레벨 변경
        currentReinforcementLevel = level;
        
        // 효과 계산
        float result = GetCurrentEffects().healthBonus;
        
        // 원래 레벨로 복원
        currentReinforcementLevel = currentLevel;
        
        return result;
    }

     /// <summary>
    /// 강화에 필요한 재료를 인벤토리에서 확인
    /// </summary>
    public bool CanReinforce(PlayerEntity player)
    {
        // 기본 강화 가능 조건 확인
        if (currentReinforcementLevel >= maxReinforcementLevel)
            return false;

        var recipe = GetReinforcementRecipe();
        if (recipe == null)
        {
            Debug.LogWarning($"{ItemName} - 강화 레시피를 찾을 수 없습니다.");
            return false;
        }

        // 인벤토리에서 재료 확인
        var playerInventory = player.GetInventoryItems();
        for (int i = 0; i < recipe.requiredItemIDs.Count; i++)
        {
            int requiredItemId = recipe.requiredItemIDs[i];
            int requiredCount = recipe.requiredItemCounts[i];

            // 인벤토리에서 해당 아이템의 총 개수 계산
            int currentCount = playerInventory
                .Where(item => item is ConsumableItem consumable && consumable.ID == requiredItemId)
                .Sum(item => (item as ConsumableItem).cunamount);

            if (currentCount < requiredCount)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 강화 실행 (재료 소모 포함)
    /// </summary>
    public bool Reinforce(PlayerEntity player)
    {
        // 강화 가능 여부 먼저 확인
        if (!CanReinforce(player))
        {
            Debug.LogWarning($"{ItemName} 강화 불가");
            return false;
        }

        var recipe = GetReinforcementRecipe();
        var playerData = BattleFlowController.Instance?.playerData;
        
        if (playerData == null) return false;

        // 재료 소모
        for (int i = 0; i < recipe.requiredItemIDs.Count; i++)
        {
            int requiredItemId = recipe.requiredItemIDs[i];
            int requiredCount = recipe.requiredItemCounts[i];

            ConsumeItemsFromInventory(playerData.inventory, requiredItemId, requiredCount);
        }
        // 플레이어 옵저버들에게 알림
        player.NotifyObservers();

        return true;
    }

    /// <summary>
    /// 인벤토리에서 특정 아이템 소모
    /// </summary>
    private void ConsumeItemsFromInventory(List<Item> inventory, int itemId, int requiredCount)
    {
        var consumableItems = inventory
            .OfType<ConsumableItem>()
            .Where(item => item.ID == itemId)
            .OrderBy(item => item.cunamount)
            .ToList();

        foreach (var item in consumableItems)
        {
            if (requiredCount <= 0) break;

            int consumeAmount = Mathf.Min(item.cunamount, requiredCount);
            item.cunamount -= consumeAmount;
            requiredCount -= consumeAmount;

            // 아이템 개수가 0이 되면 인벤토리에서 제거
            if (item.cunamount <= 0)
            {
                inventory.Remove(item);
            }
        }

        // 필요한 개수를 모두 소모하지 못했다면 예외 처리
        if (requiredCount > 0)
        {
            Debug.LogError($"아이템 ID {itemId}의 재료가 부족합니다.");
        }
    }


    /// <summary>
    /// 강화로 인한 추가 공격력만 계산
    /// </summary>
    public float GetReinforcementAttackBonus()
    {
        if (currentReinforcementLevel <= 0) return 0f;
        return GetCurrentEffects().attackBonus - attackBonus;
    }

    /// <summary>
    /// 강화로 인한 추가 방어력만 계산
    /// </summary>
    public float GetReinforcementDefenseBonus()
    {
        if (currentReinforcementLevel <= 0) return 0f;
        return GetCurrentEffects().defenseBonus - defenseBonus;
    }

    /// <summary>
    /// 강화로 인한 추가 체력만 계산
    /// </summary>
    public float GetReinforcementHealthBonus()
    {
        if (currentReinforcementLevel <= 0) return 0f;
        return GetCurrentEffects().healthBonus - healthBonus;
    }

    /// <summary>
    /// 아이템 등급에 따른 표시 이름 반환
    /// </summary>
    public string GetDisplayName()
    {
        string displayName = ItemName;

        if (currentReinforcementLevel > 0)
        {
            displayName += $" +{currentReinforcementLevel}";
        }

        return displayName;
    }

    /// <summary>
    /// 아이템 상세 설명 반환
    /// </summary>
    public string GetDetailedDescription()
    {
        string details = description + "\n\n";
        var currentEffects = GetCurrentEffects();

        // 기본 스탯
        details += "=== 기본 능력치 ===\n";
        if (attackBonus > 0)
            details += $"공격력: +{attackBonus}\n";
        if (defenseBonus > 0)
            details += $"방어력: +{defenseBonus}\n";
        if (healthBonus > 0)
            details += $"체력: +{healthBonus}\n";
        if (maxOxygenBonus > 0)
            details += $"최대 산소: +{maxOxygenBonus}\n";
        if (maxEnergyBonus > 0)
            details += $"최대 에너지: +{maxEnergyBonus}\n";

        // 특수 효과
        if (extraHitCount > 0 || fireRateBonus > 0 || oxygenConsumptionReduction > 0 ||
            energyConsumptionReduction > 0 || inventorySlotBonus > 0 || damageReduction > 0)
        {
            details += "\n=== 특수 효과 ===\n";
            if (extraHitCount > 0)
                details += $"추가 타격 횟수: +{extraHitCount}\n";
            if (fireRateBonus > 0)
                details += $"연사 속도: +{fireRateBonus:P0}\n";
            if (oxygenConsumptionReduction > 0)
                details += $"산소 소모 감소: {oxygenConsumptionReduction:P0}\n";
            if (energyConsumptionReduction > 0)
                details += $"에너지 소모 감소: {energyConsumptionReduction:P0}\n";
            if (inventorySlotBonus > 0)
                details += $"인벤토리 슬롯: +{inventorySlotBonus}칸\n";
            if (damageReduction > 0)
                details += $"피해 감소: {damageReduction:P0}\n";
        }

        // 강화 정보
        if (currentReinforcementLevel > 0)
        {
            details += "\n=== 강화 효과 ===\n";
            if (attackBonus > 0)
                details += $"강화 공격력: +{GetReinforcementAttackBonus():F1}\n";
            if (defenseBonus > 0)
                details += $"강화 방어력: +{GetReinforcementDefenseBonus():F1}\n";
            if (healthBonus > 0)
                details += $"강화 체력: +{GetReinforcementHealthBonus():F1}\n";
            
            // 특수 효과 강화 정보도 추가
            if (extraHitCount > 0)
                details += $"강화 추가 타격: +{currentEffects.extraHitCount - extraHitCount}\n";
            if (fireRateBonus > 0)
                details += $"강화 연사 속도: +{currentEffects.fireRateBonus - fireRateBonus:P0}\n";
            if (oxygenConsumptionReduction > 0)
                details += $"강화 산소 소모 감소: +{currentEffects.oxygenConsumptionReduction - oxygenConsumptionReduction:P0}\n";
        }

        // 총합
        details += "\n=== 총 능력치 ===\n";
        if (attackBonus > 0)
            details += $"총 공격력: +{currentEffects.attackBonus:F1}\n";
        if (defenseBonus > 0)
            details += $"총 방어력: +{currentEffects.defenseBonus:F1}\n";
        if (healthBonus > 0)
            details += $"총 체력: +{currentEffects.healthBonus:F1}\n";
        if (maxOxygenBonus > 0)
            details += $"총 최대 산소: +{currentEffects.maxOxygenBonus:F1}\n";
        if (maxEnergyBonus > 0)
            details += $"총 최대 에너지: +{currentEffects.maxEnergyBonus:F1}\n";

        return details;
    }

    /// <summary>
    /// 아이템 복제 (강화 레벨 포함)
    /// </summary>
    public override Item Clone()
    {
        // DataCenter에서 새 인스턴스를 생성
        var cloned = DataCenter.Instance.CreateEquipableItem(this.ID);

        // 강화 레벨 복사
        if (cloned != null)
        {
            cloned.currentReinforcementLevel = this.currentReinforcementLevel;
        }

        return cloned;
    }

    /// <summary>
    /// 아이템 사용 (장착)
    /// </summary>
    public override void Use(PlayerEntity player)
    {
        if (player == null) return;

        var equipmentComponent = player.GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent == null)
        {
            Debug.LogWarning("플레이어에게 EquipmentComponent가 없습니다.");
            return;
        }

        // 아이템 장착 시도
        if (equipmentComponent.EquipItem(this))
        {
            // 장착 성공 시 인벤토리에서 제거
            if (BattleFlowController.Instance?.playerData != null)
            {
                BattleFlowController.Instance.playerData.RemoveItem(this);
            }

            Debug.Log($"{GetDisplayName()} 장착 완료!");
            player.NotifyObservers();
        }
    }

    /// <summary>
    /// 디버그용 강화 정보 출력
    /// </summary>
    [Button("강화 정보 출력")]
    public void PrintReinforcementInfo()
    {
        Debug.Log($"=== {GetDisplayName()} 강화 정보 ===");
        Debug.Log($"현재 레벨: +{currentReinforcementLevel} / {maxReinforcementLevel}");
        Debug.Log($"강화 가능: {(CanReinforce() ? "예" : "아니오")}");

        if (CanReinforce())
        {
            Debug.Log($"성공 확률: {GetReinforcementSuccessRate():F1}%");
        }

        Debug.Log($"총 공격력: +{GetTotalAttackBonus():F1} (기본 {attackBonus} + 강화 {GetReinforcementAttackBonus():F1})");
        Debug.Log($"총 방어력: +{GetTotalDefenseBonus():F1} (기본 {defenseBonus} + 강화 {GetReinforcementDefenseBonus():F1})");
        Debug.Log($"총 체력: +{GetTotalHealthBonus():F1} (기본 {healthBonus} + 강화 {GetReinforcementHealthBonus():F1})");
    }

    public EquipmentEffects GetCurrentEffects()
    {
        // 기본 효과 생성
        EquipmentEffects effects = new EquipmentEffects
        {
            attackBonus = this.attackBonus,
            defenseBonus = this.defenseBonus,
            healthBonus = this.healthBonus,
            maxOxygenBonus = this.maxOxygenBonus,
            maxEnergyBonus = this.maxEnergyBonus,
            extraHitCount = this.extraHitCount,
            fireRateBonus = this.fireRateBonus,
            oxygenConsumptionReduction = this.oxygenConsumptionReduction,
            energyConsumptionReduction = this.energyConsumptionReduction,
            inventorySlotBonus = this.inventorySlotBonus,
            damageReduction = this.damageReduction
        };
        
        // 강화 레벨에 따른 추가 효과 계산
        if (currentReinforcementLevel > 0)
        {
            // 강화 레벨에 비례해 효과 증가 (10% 씩)
            float bonus = 0.1f * currentReinforcementLevel;
            
            effects.attackBonus *= (1 + bonus);
            effects.defenseBonus *= (1 + bonus);
            effects.healthBonus *= (1 + bonus);
            effects.maxOxygenBonus *= (1 + bonus);
            effects.maxEnergyBonus *= (1 + bonus);
            
            // 강화에 따라 특수 효과도 증가
            effects.extraHitCount += Mathf.RoundToInt(effects.extraHitCount * bonus);
            effects.fireRateBonus *= (1 + bonus);
            effects.oxygenConsumptionReduction *= (1 + bonus);
            effects.energyConsumptionReduction *= (1 + bonus);
            effects.inventorySlotBonus += Mathf.RoundToInt(effects.inventorySlotBonus * bonus);
            effects.damageReduction *= (1 + bonus);
        }
        
        return effects;
    }
}
