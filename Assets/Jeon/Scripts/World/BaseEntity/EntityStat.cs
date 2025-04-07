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
    private Dictionary<EntityStatName, float> baseStats = new Dictionary<EntityStatName, float>();


    [NonSerialized]
    private Dictionary<EntityStatName, Dictionary<object, float>> updatedStats = new Dictionary<EntityStatName, Dictionary<object, float>>();

    public EntityStat(string name, int level)
    {
        Name = name;
        Level = level;
        foreach (EntityStatName stat in Enum.GetValues(typeof(EntityStatName)))
        {
            baseStats[stat] = 0;
        }
    }

    public static EntityStat CreatPlayerData()
    {
        var data = new EntityStat("Player", 1);
        data.SetBaseStat(EntityStatName.HP, 100);
        data.SetBaseStat(EntityStatName.MaxHP, 100);
        data.SetBaseStat(EntityStatName.SP, 100);
        data.SetBaseStat(EntityStatName.MaxSP, 100);
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
            Debug.Log($"{statName}={value}");
        }
    }

    public void UpdateBaseStat(EntityStatName statName, float value)
    {
        if (baseStats.ContainsKey(statName))
        {
            baseStats[statName] += value;
            if (statName == EntityStatName.HP)
            {
                baseStats[statName] = Mathf.Min(baseStats[statName], GetStat(EntityStatName.MaxHP));
            }
            else if (statName == EntityStatName.SP)
            {
                baseStats[statName] = Mathf.Min(baseStats[statName], GetStat(EntityStatName.MaxSP));
            }
        }
    }
    public void UpdateStat(EntityStatName statName, object source, float value)
    {
        if (!updatedStats.ContainsKey(statName))
        {
            updatedStats[statName] = new Dictionary<object, float>();
        }
        if (!updatedStats[statName].ContainsKey(source))
        {
            updatedStats[statName][source] = 0;
        }

        updatedStats[statName][source] += value;

        if (statName == EntityStatName.HP)
        {
            baseStats[statName] = Mathf.Min(baseStats[statName], GetStat(EntityStatName.MaxHP));
        }
        else if (statName == EntityStatName.SP)
        {
            baseStats[statName] = Mathf.Min(baseStats[statName], GetStat(EntityStatName.MaxSP));
        }
    }
    public void ChangeStat(EntityStatName statName, object source, float value)
    {
        if (!updatedStats.ContainsKey(statName))
        {
            updatedStats[statName] = new Dictionary<object, float>();
        }
        updatedStats[statName][source] = value;
    }

    public float GetStat(EntityStatName statName)
    {
        float baseValue = baseStats.ContainsKey(statName) ? baseStats[statName] : 0;

        if (updatedStats.ContainsKey(statName))
        {
            foreach (var bonus in updatedStats[statName].Values)
            {
                baseValue += bonus;
            }
        }

        return baseValue;
    }
}