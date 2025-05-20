using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

[System.Serializable]
public class FieldTileData
{
    [ShowInInspector] public int ID;
    [ShowInInspector] public int StageLevel;
    [ShowInInspector] public int minCount;
    [ShowInInspector] public int maxCount;
    [ShowInInspector] public List<int> ObjectID;
    [ShowInInspector] public List<float> ObjectValue;
    [ShowInInspector] public string description;

    // 편의성 메서드들
    [ShowInInspector, ReadOnly]
    public int CombinedTypeCode => StageLevel * 10 + ID;

    public int GetRandomObjectID()
    {
        if (ObjectID == null || ObjectID.Count == 0) return -1;

        float totalWeight = ObjectValue.Sum();
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
}