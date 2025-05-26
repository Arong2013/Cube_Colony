using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class FieldTileData
{
    [ShowInInspector] public int ID;
    [ShowInInspector] public int FieldLevel; // 스테이지 레벨
    [ShowInInspector] public int TileLevel;  // 타일 강화 레벨
    [ShowInInspector] public CubieFaceSkillType StageType; // enum 타입 사용
    [ShowInInspector] public int minCount;
    [ShowInInspector] public int maxCount;
    [ShowInInspector] public List<int> ObjectID;
    [ShowInInspector] public List<float> ObjectValue;
    [ShowInInspector] public string description;

    public int GetRandomObjectID()
    {
        if (ObjectID == null || ObjectID.Count == 0) return -1;

        float totalWeight = 0f;
        foreach (var weight in ObjectValue)
        {
            totalWeight += weight;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        for (int i = 0; i < ObjectID.Count; i++)
        {
            currentWeight += ObjectValue[i];
            if (randomValue <= currentWeight)
            {
                return ObjectID[i];
            }
        }

        return ObjectID[0]; // 폴백
    }
    public FieldTileData FindFieldTileData(CubieFaceInfo faceInfo, int currentFieldLevel)
    {
        // 1. First try to find by field level
        var fieldTileDatasByLevel = DataCenter.Instance.GetFieldTileDatasByFieldLevel(currentFieldLevel);

        if (fieldTileDatasByLevel.Count == 0)
        {
            Debug.LogWarning($"필드 레벨 {currentFieldLevel}에 해당하는 필드 타일 데이터가 없습니다.");
            return null;
        }

        var matchingLevelDatas = fieldTileDatasByLevel.FindAll(data =>
            data.FieldLevel == currentFieldLevel &&
            data.TileLevel == faceInfo.Level);

        if (matchingLevelDatas.Count == 0)
        {
            Debug.LogWarning($"필드 레벨 {currentFieldLevel}, 타일 레벨 {faceInfo.Level}에 해당하는 필드 타일 데이터가 없습니다.");
            return null;
        }

        // 3. Filter by type
        CubieFaceSkillType faceType = faceInfo.Type;
        var matchingTypeDatas = matchingLevelDatas.FindAll(data =>
            data.StageType == faceType);

        if (matchingTypeDatas.Count == 0)
        {
            Debug.LogWarning($"필드 레벨 {currentFieldLevel}, 타일 레벨 {faceInfo.Level}, 타입 {faceType}에 해당하는 필드 타일 데이터가 없습니다.");
            return null;
        }

        // Return the first matching data
        return matchingTypeDatas[0];
    }
}

