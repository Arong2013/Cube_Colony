using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EquipableItem", menuName = "Data/EquipableItem")]
public class EquipableItemSO : ScriptableObject
{
    [TitleGroup("기본 정보")]
    [LabelText("아이템 ID"), ReadOnly]
    public int ID;

    [TitleGroup("기본 정보")]
    [LabelText("아이템 이름")]
    public string ItemName;

    [TitleGroup("장비 정보")]
    [LabelText("장비 타입"), EnumToggleButtons]
    public EquipmentType equipmentType;

    [TitleGroup("장비 정보")]
    [LabelText("요구 레벨"), MinValue(1)]
    public int requiredLevel = 1;

    [TitleGroup("기본 스탯 보너스")]
    [LabelText("공격력 증가")]
    public float attackBonus = 0f;

    [TitleGroup("기본 스탯 보너스")]
    [LabelText("방어력 증가")]
    public float defenseBonus = 0f;

    [TitleGroup("기본 스탯 보너스")]
    [LabelText("체력 증가")]
    public float healthBonus = 0f;

    [TitleGroup("강화 설정")]
    [LabelText("최대 강화 레벨"), Range(0, 5)]
    public int maxReinforcementLevel = 3;

    [TitleGroup("강화 설정")]
    [LabelText("강화 비용 (골드)")]
    [InfoBox("각 단계별 강화에 필요한 골드 비용을 설정하세요. [0]=+1강화, [1]=+2강화, [2]=+3강화")]
    public int[] reinforcementCosts = new int[] { 100, 200, 400 };

    [TitleGroup("특수 효과")]
    [LabelText("특수 효과 1")]
    [InfoBox("장비별 특수 효과 값 (검: 타격횟수, 총: 연사속도 등)")]
    public float specialEffect1 = 0f;

    [TitleGroup("특수 효과")]
    [LabelText("특수 효과 2")]
    public float specialEffect2 = 0f;

    [TitleGroup("디스플레이")]
    [LabelText("설명"), TextArea(2, 4)]
    public string description;

    [TitleGroup("디스플레이")]
    [LabelText("아이템 아이콘"), PreviewField(80)]
    public Sprite itemIcon;

    [TitleGroup("디스플레이")]
    [LabelText("등급"), EnumToggleButtons]
    public ItemGrade grade = ItemGrade.Common;

    [TitleGroup("디버그 정보")]
    [ShowInInspector, ReadOnly]
    [LabelText("장비 슬롯")]

    [TitleGroup("디버그 정보")]
    [Button("강화 효과 미리보기")]
    public void PreviewReinforcements()
    {
        Debug.Log($"=== {ItemName} 강화 효과 미리보기 ===");

        for (int level = 0; level <= maxReinforcementLevel; level++)
        {
            var tempItem = CreateTempItem();
            tempItem.currentReinforcementLevel = level;
            var effects = tempItem.GetCurrentEffects();

            Debug.Log($"레벨 {level}: {GetEffectsPreview(effects)}");
        }
    }

    [TitleGroup("디버그 정보")]
    [Button("테스트 아이템 생성")]
    public void CreateTestItem()
    {
        var item = CreateTempItem();
        Debug.Log($"테스트 아이템 생성: {item.GetDisplayName()}");
    }

    /// <summary>
    /// 임시 아이템 생성 (테스트용)
    /// </summary>
    private EquipableItem CreateTempItem()
    {
        var item = new EquipableItem();
        item.ID = this.ID;
        item.ItemName = this.ItemName;
        item.equipmentType = this.equipmentType;
        item.requiredLevel = this.requiredLevel;
        item.attackBonus = this.attackBonus;
        item.defenseBonus = this.defenseBonus;
        item.healthBonus = this.healthBonus;
        item.maxReinforcementLevel = this.maxReinforcementLevel;
        item.reinforcementCosts = (int[])this.reinforcementCosts.Clone();
        item.specialEffect1 = this.specialEffect1;
        item.specialEffect2 = this.specialEffect2;
        item.description = this.description;
        item.grade = this.grade;
        item.itemIcon = this.itemIcon;
        return item;
    }

    /// <summary>
    /// 효과 미리보기 문자열 생성
    /// </summary>
    private string GetEffectsPreview(EquipmentEffects effects)
    {
        var preview = "";

        if (effects.attackBonus > 0)
            preview += $"공격력+{effects.attackBonus} ";
        if (effects.defenseBonus > 0)
            preview += $"방어력+{effects.defenseBonus} ";
        if (effects.healthBonus > 0)
            preview += $"체력+{effects.healthBonus} ";
        if (effects.maxOxygenBonus > 0)
            preview += $"산소+{effects.maxOxygenBonus} ";
        if (effects.maxEnergyBonus > 0)
            preview += $"에너지+{effects.maxEnergyBonus} ";
        if (effects.extraHitCount > 0)
            preview += $"타격+{effects.extraHitCount} ";
        if (effects.fireRateBonus > 0)
            preview += $"연사+{effects.fireRateBonus} ";
        if (effects.oxygenConsumptionReduction > 0)
            preview += $"산소절약{effects.oxygenConsumptionReduction * 100}% ";
        if (effects.energyConsumptionReduction > 0)
            preview += $"에너지절약{effects.energyConsumptionReduction * 100}% ";
        if (effects.inventorySlotBonus > 0)
            preview += $"슬롯+{effects.inventorySlotBonus} ";
        if (effects.damageReduction > 0)
            preview += $"피해감소{effects.damageReduction * 100}% ";

        return string.IsNullOrEmpty(preview) ? "효과없음" : preview.Trim();
    }

    /// <summary>
    /// 강화 비용 배열 유효성 검사
    /// </summary>
    private void OnValidate()
    {
        // 강화 비용 배열 크기 자동 조정
        if (reinforcementCosts == null || reinforcementCosts.Length != maxReinforcementLevel)
        {
            var newCosts = new int[maxReinforcementLevel];

            for (int i = 0; i < maxReinforcementLevel; i++)
            {
                if (reinforcementCosts != null && i < reinforcementCosts.Length)
                {
                    newCosts[i] = reinforcementCosts[i];
                }
                else
                {
                    // 기본 강화 비용 (지수 증가)
                    newCosts[i] = 100 * (int)Mathf.Pow(2, i);
                }
            }

            reinforcementCosts = newCosts;
        }

        // 음수 방지
        for (int i = 0; i < reinforcementCosts.Length; i++)
        {
            reinforcementCosts[i] = Mathf.Max(0, reinforcementCosts[i]);
        }
    }

    /// <summary>
    /// 장비 타입에 따른 설명 자동 생성
    /// </summary>
    [TitleGroup("유틸리티")]
    [Button("설명 자동 생성")]
    public void GenerateDescription()
    {
        description = equipmentType switch
        {
            EquipmentType.Sword => $"날카로운 {ItemName}. 공격력이 증가하고 강화할수록 추가 타격이 가능해집니다.",
            EquipmentType.Gun => $"강력한 {ItemName}. 원거리 공격력과 연사속도가 증가합니다.",
            EquipmentType.OxygenTank => $"고성능 {ItemName}. 산소 용량을 늘리고 소모량을 줄여줍니다.",
            EquipmentType.Battery => $"대용량 {ItemName}. 에너지 용량을 늘리고 소모량을 줄여줍니다.",
            EquipmentType.Backpack => $"넓은 {ItemName}. 인벤토리 공간을 확장해줍니다.",
            EquipmentType.Helmet => $"견고한 {ItemName}. 체력을 늘리고 받는 피해를 줄여줍니다.",
            _ => $"신비한 {ItemName}. 착용자에게 특별한 능력을 부여합니다."
        };

        Debug.Log($"설명이 자동 생성되었습니다: {description}");
    }

    /// <summary>
    /// 장비 타입에 따른 기본 스탯 설정
    /// </summary>
    [TitleGroup("유틸리티")]
    [Button("타입별 기본 스탯 설정")]
    public void SetDefaultStatsByType()
    {
        switch (equipmentType)
        {
            case EquipmentType.Sword:
                attackBonus = 15f;
                defenseBonus = 0f;
                healthBonus = 0f;
                break;
            case EquipmentType.Gun:
                attackBonus = 12f;
                defenseBonus = 0f;
                healthBonus = 0f;
                break;
            case EquipmentType.OxygenTank:
                attackBonus = 0f;
                defenseBonus = 0f;
                healthBonus = 0f;
                break;
            case EquipmentType.Battery:
                attackBonus = 0f;
                defenseBonus = 0f;
                healthBonus = 0f;
                break;
            case EquipmentType.Backpack:
                attackBonus = 0f;
                defenseBonus = 0f;
                healthBonus = 0f;
                break;
            case EquipmentType.Helmet:
                attackBonus = 0f;
                defenseBonus = 5f;
                healthBonus = 20f;
                break;
        }

        Debug.Log($"{equipmentType}에 맞는 기본 스탯이 설정되었습니다.");
    }
}