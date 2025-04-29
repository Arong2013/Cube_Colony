using UnityEngine;
using System.Collections.Generic;

public class FieldTile : MonoBehaviour
{
    [SerializeField] protected CubieFaceInfo faceInfo;
    private FieldTileData tileData;

    public CubieFaceSkillType Type => faceInfo.Type;

    private int cunStageLevel;
    public int MaxLevel => faceInfo.MaxLevel;
    public int CombinedTypeCode => cunStageLevel * 10 + (int)Type;
    public void Initialize(int cunStageLevel, CubieFaceInfo info)
    {
        this.cunStageLevel = cunStageLevel;   
        faceInfo = info;
        tileData = ItemDataCenter.Get<FieldTileData>(CombinedTypeCode);
        SpawnObjects();
    }

    public void SpawnObjects()
    {
        if (tileData == null || tileData.ObjectID == null || tileData.ObjectID.Count == 0)
        {
            Debug.LogWarning("Tile data is missing or empty.");
            return;
        }
        int spawnCount = Random.Range(tileData.minCount, tileData.maxCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            int objectId = GetRandomWeightedObjectID();
            GameObject prefab = DataCenter.Instance.GetEntity(objectId);

            if (prefab == null)
            {
                Debug.LogWarning($"No prefab found for object ID: {objectId}");
                continue;
            }

            Vector3 spawnPos = GetRandomPositionWithinBounds();
            GameObject obj = Instantiate(prefab.gameObject, spawnPos, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
    }

    private Vector3 GetRandomPositionWithinBounds()
    {
        var bounds = GetComponent<Renderer>().bounds;

        float marginRatio = 0.1f; 
        float marginX = bounds.size.x * marginRatio;
        float marginZ = bounds.size.z * marginRatio;

        float randX = Random.Range(bounds.min.x + marginX, bounds.max.x - marginX);
        float randZ = Random.Range(bounds.min.z + marginZ, bounds.max.z - marginZ);
        float y = bounds.max.y;

        return new Vector3(randX, y + 0.1f, randZ);
    }

    private int GetRandomWeightedObjectID()
    {
        float totalWeight = 0f;
        for (int i = 0; i < tileData.ObjectValue.Count; i++)
        {
            totalWeight += tileData.ObjectValue[i];
        }

        float rand = Random.Range(0f, totalWeight);
        float current = 0f;

        for (int i = 0; i < tileData.ObjectID.Count; i++)
        {
            current += tileData.ObjectValue[i];
            if (rand <= current)
            {
                return tileData.ObjectID[i];
            }
        }
        return tileData.ObjectID[0];
    }

}
