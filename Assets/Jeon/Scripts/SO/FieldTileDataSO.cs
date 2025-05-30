﻿using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "FieldTileData", menuName = "Data/FieldTileData")]
public class FieldTileDataSO : ScriptableObject
{
    [TitleGroup("기본 정보")]
    [LabelText("타일 ID"), ReadOnly]
    public int ID;

    [TitleGroup("기본 정보")]
    [LabelText("타일 이름")]
    public string IconName;

    [TitleGroup("스테이지 정보")]
    [LabelText("필드 레벨"), MinValue(1), Tooltip("현재 스테이지 레벨")]
    public int FieldLevel = 1;
    
    [TitleGroup("스테이지 정보")]
    [LabelText("타일 레벨"), MinValue(0), Tooltip("타일의 강화 레벨 (0=기본, 1=강화, 2=고급)")]
    public int TileLevel = 0;

    [TitleGroup("스테이지 정보")]
    [LabelText("스테이지 타입"), Tooltip("타일의 타입 (RMonster, AMonster, Mine, Plant 등)")]
    public string StageType;

    [TitleGroup("스폰 설정")]
    [LabelText("최소 몬스터 수"), MinValue(0)]
    public int minCount = 1;

    [TitleGroup("스폰 설정")]
    [LabelText("최대 몬스터 수"), MinValue(1)]
    public int maxCount = 5;

    [TitleGroup("스폰 오브젝트")]
    [LabelText("오브젝트 ID 목록")]
    [InfoBox("스폰될 몬스터/오브젝트들의 ID")]
    public List<int> ObjectID = new List<int>();

    [TitleGroup("스폰 오브젝트")]
    [LabelText("스폰 확률/가중치")]
    [InfoBox("각 오브젝트의 스폰 확률 (ObjectID와 1:1 대응)")]
    public List<float> ObjectValue = new List<float>();

    [TitleGroup("스폰 오브젝트")]
    [LabelText("아이템 ID 목록")]
    [InfoBox("스폰될 아이템/오브젝트들의 ID")]
    public List<int> ItemObjID = new List<int>();

    [TitleGroup("디스플레이")]
    [LabelText("타일 설명"), TextArea(2, 3)]
    public string description;


    [TitleGroup("디스플레이")]
    [LabelText("타일 아이콘"), PreviewField(80)]
    public string tileIconName;

    [TitleGroup("디스플레이")]
    [LabelText("타일 아이콘"), PreviewField(80)]
    public Sprite tileIcon => Resources.Load<Sprite>($"Sprites/FieldTiles/{tileIconName}");

    [TitleGroup("디버그 정보")]
    [ShowInInspector, ReadOnly]
    [LabelText("오브젝트 개수")]
    public int ObjectCount => ObjectID?.Count ?? 0;

    [TitleGroup("디버그 정보")]
    [Button("데이터 검증")]
    public void ValidateData()
    {
        if (ObjectID.Count != ObjectValue.Count)
        {
            Debug.LogError($"❌ ObjectID와 ObjectValue의 개수가 일치하지 않습니다! ID: {ObjectID.Count}, Value: {ObjectValue.Count}");
        }
        else
        {
            Debug.Log($"✅ 데이터 검증 통과: {ObjectID.Count}개 오브젝트");
        }
    }
}
