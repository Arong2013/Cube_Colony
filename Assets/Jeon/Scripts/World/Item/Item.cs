using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ICloneableItem<T>
{
    T Clone();
}

[System.Serializable]
public abstract class Item : ICloneableItem<Item>
{
    [TitleGroup("기본 정보")]
    [ShowInInspector] public int ID;

    [TitleGroup("기본 정보")]
    [ShowInInspector] public string ItemName;

    [TitleGroup("기본 정보")]
    [ShowInInspector, TextArea(2, 5)]
    public string Description = "아이템 설명이 없습니다.";

    [TitleGroup("기본 정보")]
    [ShowInInspector]
    public string AcquisitionTile = "알 수 없음";  // 획득 타일

    [TitleGroup("시각 정보")]
    [ShowInInspector, PreviewField(50)]
    public Sprite ItemIcon => Resources.Load<Sprite>($"Sprites/Items/{ItemName}");

    public abstract void Use(PlayerEntity player);
    public abstract Item Clone();
}