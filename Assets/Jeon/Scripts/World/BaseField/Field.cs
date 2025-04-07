using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public struct FieldData
{
    public Vector3 position;
    public List<CubieFaceInfo> faceinfos;
    public int size;
}
public class Field : MonoBehaviour
{
    private PlayerEntity spawnedPlayer;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private GameObject[] randomPrefabs;
    private FieldData fieldData;

    public void Initialize(FieldData fieldData)
    {
        gameObject.SetActive(true);
        this.fieldData = fieldData;
        navMeshSurface = GetComponent<NavMeshSurface>();

        transform.position = fieldData.position;
        transform.localScale = new Vector3(fieldData.size, 0.1f, fieldData.size);
        navMeshSurface.BuildNavMesh();

        var spawnPos = transform.position + Vector3.up;

        if (spawnedPlayer == null)
        {

            GameObject playerObj = Instantiate(DataCenter.Instance.GetPlayerEntity(), spawnPos, Quaternion.identity);
            spawnedPlayer = playerObj.GetComponent<PlayerEntity>();
            playerObj.transform.SetParent(transform);
        }
        else
        {
            spawnedPlayer.transform.position = spawnPos;
        }


        int size = fieldData.size;
        float spacing = size == 2 ? 5 : 3;
        float half = (size - 1) * 0.5f;

        foreach (var info in fieldData.faceinfos)
        {
            float x = (info.Position.x - half) * spacing;
            float z = (info.Position.z - half) * spacing;

            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.SetParent(transform, false);
            plane.transform.localPosition = new Vector3(x, 0.5f, z);
            var scaleSize = 1f / size;  
            plane.transform.localScale = new Vector3(scaleSize, 1f, scaleSize); // or just Vector3.one if 1x1 tile

            SpawnRandomObjectOnPlane(plane);
        }
    }
    void SpawnRandomObjectOnPlane(GameObject plane)
    {
        var bounds = plane.GetComponent<Renderer>().bounds;

        // 랜덤 위치 (XZ 평면)
        float randX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float randZ = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.max.y; 

        Vector3 spawnPos = new Vector3(randX, y + 0.1f, randZ);

        // 랜덤 오브젝트 선택
        var prefab = randomPrefabs[UnityEngine.Random.Range(0, randomPrefabs.Length)];
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Plane 밑에 붙이기 (선택사항)
        obj.transform.SetParent(plane.transform);
    }
}