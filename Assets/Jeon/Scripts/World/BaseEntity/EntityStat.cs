﻿using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class EntityStat
{
    [SerializeField]
    [Header("기본 정보")]
    public readonly int ID;

    [SerializeField]
    [Header("기본 정보")]
    public string Name;

    [SerializeField]
    [Header("기본 정보")]
    public int Level;

    [SerializeField]
    [Header("위치 정보")]
    public Vector3 position;

    [SerializeField]
    [Header("기본 스탯")]
    [ShowInInspector] public Dictionary<EntityStatName, float> baseStats = new();
    [NonSerialized]
    private Dictionary<EntityStatName, Dictionary<object, float>> updatedStats = new();

    public EntityStat()
    {
        foreach (EntityStatName stat in Enum.GetValues(typeof(EntityStatName)))
        {
            // Eng와 MaxEng는 PlayerData로 이동했으므로 여기서는 초기화하지 않음
            if (stat != EntityStatName.Eng && stat != EntityStatName.MaxEng)
            {
                baseStats[stat] = 0;
            }
        }
    }

    public static EntityStat CreatePlayerData()
    {
        var data = new EntityStat();
        data.SetBaseStat(EntityStatName.HP, 100);
        data.SetBaseStat(EntityStatName.MaxHP, 100);
        data.SetBaseStat(EntityStatName.O2, 100);
        data.SetBaseStat(EntityStatName.MaxO2, 100);
        // Eng와 MaxEng 제거
        data.SetBaseStat(EntityStatName.ATK, 20);
        data.SetBaseStat(EntityStatName.DEF, 20);
        data.SetBaseStat(EntityStatName.SPD, 3);
        return data;
    }

    public void SetBaseStat(EntityStatName statName, float value)
    {
        // Eng와 MaxEng는 PlayerData에서 관리
        if (statName == EntityStatName.Eng || statName == EntityStatName.MaxEng)
        {
            Debug.LogWarning($"Energy is now managed by PlayerData, not EntityStat");
            return;
        }

        if (baseStats.ContainsKey(statName))
        {
            baseStats[statName] = value;
            ClampIfNeeded(statName);
            Debug.Log($"{statName}={value}");
        }
    }

    public void UpdateBaseStat(EntityStatName statName, float value)
    {
        // Eng와 MaxEng는 PlayerData에서 관리
        if (statName == EntityStatName.Eng || statName == EntityStatName.MaxEng)
        {
            Debug.LogWarning($"Energy is now managed by PlayerData, not EntityStat");
            return;
        }

        if (baseStats.ContainsKey(statName))
        {
            baseStats[statName] += value;
            ClampIfNeeded(statName);
        }
    }

    public void UpdateStat(EntityStatName statName, object source, float value)
    {
        // Eng와 MaxEng는 PlayerData에서 관리
        if (statName == EntityStatName.Eng || statName == EntityStatName.MaxEng)
        {
            Debug.LogWarning($"Energy is now managed by PlayerData, not EntityStat");
            return;
        }

        if (!updatedStats.ContainsKey(statName))
            updatedStats[statName] = new();

        if (!updatedStats[statName].ContainsKey(source))
            updatedStats[statName][source] = 0;

        updatedStats[statName][source] += value;
        ClampIfNeeded(statName);
    }

    public void ChangeStat(EntityStatName statName, object source, float value)
    {
        // Eng와 MaxEng는 PlayerData에서 관리
        if (statName == EntityStatName.Eng || statName == EntityStatName.MaxEng)
        {
            Debug.LogWarning($"Energy is now managed by PlayerData, not EntityStat");
            return;
        }

        if (!updatedStats.ContainsKey(statName))
            updatedStats[statName] = new();

        updatedStats[statName][source] = value;
        ClampIfNeeded(statName);
    }

    public float GetStat(EntityStatName statName)
    {
        // Eng와 MaxEng는 PlayerData에서 관리
        if (statName == EntityStatName.Eng || statName == EntityStatName.MaxEng)
        {
            Debug.LogWarning($"Energy is now managed by PlayerData, not EntityStat");
            return 0;
        }

        float baseValue = baseStats.ContainsKey(statName) ? baseStats[statName] : 0;

        if (updatedStats.ContainsKey(statName))
        {
            foreach (var bonus in updatedStats[statName].Values)
                baseValue += bonus;
        }

        return baseValue;
    }

    private void ClampIfNeeded(EntityStatName statName)
    {
        //if (StatLimitMap.TryGetValue(statName, out var maxStat))
        //{
        //    float maxValue = GetStat(maxStat);
        //    if (baseStats.ContainsKey(statName))
        //    {
        //        baseStats[statName] = Mathf.Min(baseStats[statName], maxValue);
        //    }
        //}
    }
    
    
}