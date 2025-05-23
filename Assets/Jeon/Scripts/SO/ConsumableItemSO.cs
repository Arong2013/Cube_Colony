using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Data/ConsumableItem")]
public class ConsumableItemSO : ScriptableObject
{
    [TitleGroup("기본 정보")]
    [LabelText("아이템 ID"), ReadOnly]
    public int ID;

    [TitleGroup("기본 정보")]
    [LabelText("아이템 이름")]
    public string ItemName;

    [TitleGroup("소모품 설정")]
    [LabelText("최대 개수"), MinValue(1)]
    public int maxamount = 99;

    [TitleGroup("소모품 설정")]
    [LabelText("아이템 액션 ID 목록")]
    [InfoBox("해당 아이템을 사용할 때 실행될 액션들의 ID")]
    public List<int> ids = new List<int>();

        [TitleGroup("획득 정보")]
    [LabelText("획득 가능 필드 ID 목록")]
    [InfoBox("이 아이템을 획득할 수 있는 필드 ID들")]
    public List<int> acquirableFieldIds = new List<int>();


    [TitleGroup("디스플레이")]
    [LabelText("설명"), TextArea(2, 4)]
    public string description;

    [TitleGroup("디스플레이")]
    [LabelText("아이템 아이콘"), PreviewField(80)]
    public Sprite itemIcon =>Resources.Load<Sprite>($"Sprites/Items/{ItemName}");

    [TitleGroup("디스플레이")]
    [LabelText("등급"), EnumToggleButtons]
    public ItemGrade grade = ItemGrade.Common;

    [TitleGroup("디버그 정보")]
    [ShowInInspector, ReadOnly]
    [LabelText("액션 개수")]
    public int ActionCount => ids?.Count ?? 0;
}

public enum ItemGrade
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}