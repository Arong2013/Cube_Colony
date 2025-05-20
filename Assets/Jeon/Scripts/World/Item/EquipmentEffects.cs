// 장비 효과를 담는 구조체
using UnityEngine;

[System.Serializable]
public struct EquipmentEffects
{
    [Header("기본 스탯")]
    public float attackBonus;
    public float defenseBonus;
    public float healthBonus;
    public float maxOxygenBonus;
    public float maxEnergyBonus;

    [Header("특수 효과")]
    public int extraHitCount;           // 추가 타격 횟수 (검)
    public float fireRateBonus;         // 연사속도 증가 (총)
    public float oxygenConsumptionReduction; // 산소 소모 감소 (산소통)
    public float energyConsumptionReduction; // 에너지 소모 감소 (배터리)
    public int inventorySlotBonus;      // 인벤토리 슬롯 증가 (가방)
    public float damageReduction;       // 피해 감소 (헬멧)

    /// <summary>
    /// 두 장비 효과를 합산
    /// </summary>
    public static EquipmentEffects operator +(EquipmentEffects a, EquipmentEffects b)
    {
        return new EquipmentEffects
        {
            attackBonus = a.attackBonus + b.attackBonus,
            defenseBonus = a.defenseBonus + b.defenseBonus,
            healthBonus = a.healthBonus + b.healthBonus,
            maxOxygenBonus = a.maxOxygenBonus + b.maxOxygenBonus,
            maxEnergyBonus = a.maxEnergyBonus + b.maxEnergyBonus,
            extraHitCount = a.extraHitCount + b.extraHitCount,
            fireRateBonus = a.fireRateBonus + b.fireRateBonus,
            oxygenConsumptionReduction = a.oxygenConsumptionReduction + b.oxygenConsumptionReduction,
            energyConsumptionReduction = a.energyConsumptionReduction + b.energyConsumptionReduction,
            inventorySlotBonus = a.inventorySlotBonus + b.inventorySlotBonus,
            damageReduction = a.damageReduction + b.damageReduction
        };
    }
}