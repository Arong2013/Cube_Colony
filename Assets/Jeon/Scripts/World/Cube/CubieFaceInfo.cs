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

    public void Initialize(Vector3 pos, CubieFaceSkillType type, int maxLevel = 3)
    {
        this.type = type;
        this.level = 0;
        this.maxLevel = maxLevel;
        this.position = pos;
    }

    public bool CanLevelUp => level < maxLevel;

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void Reset()
    {
        level = 0;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }

    /// <summary>
    /// 다른 CubieFaceInfo와 위치를 비교하는 메서드
    /// </summary>
    public bool IsAtSamePosition(Vector3 otherPosition, float tolerance = 0.1f)
    {
        return Vector3.Distance(position, otherPosition) < tolerance;
    }

    /// <summary>
    /// 위치 기반 해시코드 생성 (Dictionary 등에서 사용)
    /// </summary>
    public override int GetHashCode()
    {
        // 위치를 기반으로 해시코드 생성 (소수점 반올림하여 일관성 유지)
        int x = Mathf.RoundToInt(position.x * 10f);
        int y = Mathf.RoundToInt(position.y * 10f);
        int z = Mathf.RoundToInt(position.z * 10f);
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
    }

    /// <summary>
    /// 다른 CubieFaceInfo와 같은지 비교 (위치 기반)
    /// </summary>
    public override bool Equals(object obj)
    {
        if (obj is CubieFaceInfo other)
        {
            return IsAtSamePosition(other.position);
        }
        return false;
    }

    /// <summary>
    /// 디버그용 문자열 표현
    /// </summary>
    public override string ToString()
    {
        return $"CubieFaceInfo[Type: {type}, Level: {level}, Position: {position}]";
    }
}