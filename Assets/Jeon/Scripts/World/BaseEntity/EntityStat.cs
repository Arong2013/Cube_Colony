using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class EntityStat
{
    public readonly int ID;
    public readonly string Name;
    public int Level;
    public Vector3 position;

    [SerializeField]
    private Dictionary<EntityStatName, float> baseStats = new();

    [NonSerialized]
    private Dictionary<EntityStatName, Dictionary<object, float>> updatedStats = new();

    // 스탯 제한용 맵 (HP -> MaxHP, O2 -> MaxO2, Eng -> MaxEng)
    private static readonly Dictionary<EntityStatName, EntityStatName> StatLimitMap = new()
    {
        { EntityStatName.HP, EntityStatName.MaxHP },
        { EntityStatName.O2, EntityStatName.MaxO2 },
        { EntityStatName.Eng, EntityStatName.MaxEng }
    };

    public EntityStat(string name, int level)
    {
        Name = name;
        Level = level;
        foreach (EntityStatName stat in Enum.GetValues(typeof(EntityStatName)))
        {
            baseStats[stat] = 0;
        }
    }

    public static EntityStat CreatePlayerData()
    {
        var data = new EntityStat("Player", 1);
        data.SetBaseStat(EntityStatName.HP, 100);
        data.SetBaseStat(EntityStatName.MaxHP, 100);
        data.SetBaseStat(EntityStatName.O2, 100);
        data.SetBaseStat(EntityStatName.MaxO2, 100);
        data.SetBaseStat(EntityStatName.Eng, 100);
        data.SetBaseStat(EntityStatName.MaxEng, 100);
        data.SetBaseStat(EntityStatName.ATK, 20);
        data.SetBaseStat(EntityStatName.DEF, 20);
        data.SetBaseStat(EntityStatName.SPD, 3);
        return data;
    }

    public void SetBaseStat(EntityStatName statName, float value)
    {
        if (baseStats.ContainsKey(statName))
        {
            baseStats[statName] = value;
            ClampIfNeeded(statName);
            Debug.Log($"{statName}={value}");
        }
    }

    public void UpdateBaseStat(EntityStatName statName, float value)
    {
        if (baseStats.ContainsKey(statName))
        {
            baseStats[statName] += value;
            ClampIfNeeded(statName);
        }
    }

    public void UpdateStat(EntityStatName statName, object source, float value)
    {
        if (!updatedStats.ContainsKey(statName))
            updatedStats[statName] = new();

        if (!updatedStats[statName].ContainsKey(source))
            updatedStats[statName][source] = 0;

        updatedStats[statName][source] += value;
        ClampIfNeeded(statName);
    }

    public void ChangeStat(EntityStatName statName, object source, float value)
    {
        if (!updatedStats.ContainsKey(statName))
            updatedStats[statName] = new();

        updatedStats[statName][source] = value;
        ClampIfNeeded(statName);
    }

    public float GetStat(EntityStatName statName)
    {
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
        if (StatLimitMap.TryGetValue(statName, out var maxStat))
        {
            float maxValue = GetStat(maxStat);
            if (baseStats.ContainsKey(statName))
            {
                baseStats[statName] = Mathf.Min(baseStats[statName], maxValue);
            }
        }
    }
}
