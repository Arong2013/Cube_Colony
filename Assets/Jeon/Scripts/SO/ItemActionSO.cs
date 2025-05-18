using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ItemAction", menuName = "Data/ItemAction")]
public class ItemActionSO : ScriptableObject
{
    [TitleGroup("액션 정보")]
    [LabelText("액션 ID"), ReadOnly]
    public int ID;

    [TitleGroup("액션 정보")]
    [LabelText("액션 클래스명")]
    [InfoBox("실제 itemAction 서브클래스의 이름과 정확히 일치해야 합니다")]
    public string ActionName;

    [TitleGroup("액션 정보")]
    [LabelText("액션 데이터")]
    [InfoBox("액션 생성시 전달될 파라미터 (예: 힐량, 데미지 등)")]
    public string Data;

    [TitleGroup("디스플레이")]
    [LabelText("액션 설명"), TextArea(2, 3)]
    public string description;

    [TitleGroup("디스플레이")]
    [LabelText("액션 아이콘"), PreviewField(60)]
    public Sprite actionIcon;

    [TitleGroup("디스플레이")]
    [LabelText("효과 색상"), ColorPalette]
    public Color effectColor = Color.white;

    [TitleGroup("디버그 정보")]
    [Button("액션 테스트")]
    public void TestAction()
    {
        Debug.Log($"액션 테스트: {ActionName} - 데이터: {Data}");
    }
}