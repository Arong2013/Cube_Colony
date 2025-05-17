using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerData
{
    [TitleGroup("플레이어 데이터", "인벤토리 및 스탯 정보")]
    [ReadOnly, ShowInInspector, ListDrawerSettings(ShowIndexLabels = true)]
    public List<Item> inventory { get; private set; } = new List<Item>();

    [TitleGroup("플레이어 데이터")]
    [ReadOnly, ShowInInspector]
    public EntityStat playerStat { get; private set; }

    [TitleGroup("플레이어 데이터")]
    [ProgressBar(0, 100, ColorMember = "GetEnergyColor")]
    [ShowInInspector]
    public float energy { get; private set; } = 100f;

    // 에너지 색상 변화 (보기만 가능)
    private Color GetEnergyColor
    {
        get
        {
            if (energy > 66) return Color.green;
            if (energy > 33) return Color.yellow;
            return Color.red;
        }
    }

    // 초기화
    public PlayerData()
    {
        playerStat = EntityStat.CreatePlayerData();
        inventory = new List<Item>();
        energy = 100f;
    }

    // 에너지 관련 메서드
    public void UpdateEnergy(float amount)
    {
        energy = Mathf.Clamp(energy + amount, 0f, 100f);
    }

    public void SetEnergy(float value)
    {
        energy = Mathf.Clamp(value, 0f, 100f);
    }

    // 인벤토리 관련 메서드
    public void AddItem(Item item)
    {
        inventory.Add(item);
    }

    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
    }

    public void ClearInventory()
    {
        inventory.Clear();
    }

    // 플레이어 사망 시 초기화
    public void Reset()
    {
        playerStat = EntityStat.CreatePlayerData();
        ClearInventory();
        SetEnergy(100f);
    }
}