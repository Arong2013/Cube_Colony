using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class EquipableItem : Item
{
    // SO에서 가져온 추가 정보들
    [ShowInInspector] public EquipmentType equipmentType;
    [ShowInInspector] public int requiredLevel;
    [ShowInInspector] public float attackBonus;
    [ShowInInspector] public float defenseBonus;
    [ShowInInspector] public float healthBonus;
    [ShowInInspector] public string description;
    [ShowInInspector] public ItemGrade grade;
    [ShowInInspector] public Sprite itemIcon;

    public override Item Clone()
    {
        // DataCenter에서 새 인스턴스를 생성해서 반환
        return DataCenter.Instance.CreateEquipableItem(this.ID);
    }

    public override void Use(PlayerEntity player)
    {
        // 장비 착용 로직
        if (player.GetEntityStat(EntityStatName.HP) < requiredLevel * 10) // 예시 조건
        {
            Debug.Log($"레벨이 부족합니다. 필요 레벨: {requiredLevel}");
            return;
        }

        // 스탯 보너스 적용
        player.AddEntityStatModifier(EntityStatName.ATK, this, attackBonus);
        player.AddEntityStatModifier(EntityStatName.DEF, this, defenseBonus);
        player.AddEntityStatModifier(EntityStatName.MaxHP, this, healthBonus);

        Debug.Log($"{ItemName} 장착! 공격력 +{attackBonus}, 방어력 +{defenseBonus}, 체력 +{healthBonus}");

        // 인벤토리에서 제거 (장착된 아이템은 별도 관리)
        if (BattleFlowController.Instance?.playerData != null)
        {
            BattleFlowController.Instance.playerData.RemoveItem(this);
        }

        player.NotifyObservers();
    }
}