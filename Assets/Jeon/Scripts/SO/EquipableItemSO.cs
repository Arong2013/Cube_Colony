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

    [TitleGroup("스탯 효과")]
    [LabelText("공격력 증가")]
    public float attackBonus = 0f;

    [TitleGroup("스탯 효과")]
    [LabelText("방어력 증가")]
    public float defenseBonus = 0f;

    [TitleGroup("스탯 효과")]
    [LabelText("체력 증가")]
    public float healthBonus = 0f;

    [TitleGroup("디스플레이")]
    [LabelText("설명"), TextArea(2, 4)]
    public string description;

    [TitleGroup("디스플레이")]
    [LabelText("아이템 아이콘"), PreviewField(80)]
    public Sprite itemIcon;

    [TitleGroup("디스플레이")]
    [LabelText("등급"), EnumToggleButtons]
    public ItemGrade grade = ItemGrade.Common;
}

public enum EquipmentType
{
    Weapon,
    Helmet,
    Armor,
    Boots,
    Accessory
}