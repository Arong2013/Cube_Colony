using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CubieFaceInfo
{
    [SerializeField] private CubieFaceSkillType type;
    [SerializeField] private int level = 0;
    [SerializeField] private int maxLevel = 3;
    [SerializeField] private Vector3 position;  
    public CubieFaceSkillType Type => type;
    public int Level => level;
    public int MaxLevel => maxLevel;
    public Vector3 Position => position;    

    public void Initialize(Vector3 pos,CubieFaceSkillType type, int maxLevel = 3)
    {
        this.type = type;
        this.level = 0;
        this.maxLevel = maxLevel;
        this.position = pos;    
    }

    public bool CanLevelUp => level < maxLevel;

    public bool LevelUp()
    {
        if (CanLevelUp)
        {
            level++;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        level = 0;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }   
}
