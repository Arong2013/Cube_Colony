using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public struct FieldData
{
    public int currentStageLevel;
    public Vector3 position;
    public List<CubieFaceInfo> faceinfos;
    public int size;
}
public class Field : MonoBehaviour
{
    [SerializeField] private Transform disableField;
    private PlayerEntity spawnedPlayer;
    private NavMeshSurface navMeshSurface;
    private FieldData fieldData;
    public void Initialize(FieldData fieldData, Action returnAction,Action gameOverAction)
    {
        gameObject.SetActive(true);
        this.fieldData = fieldData;
        navMeshSurface = GetComponent<NavMeshSurface>();

        transform.position = fieldData.position;
        transform.localScale = new Vector3(fieldData.size-0.5f, 0.1f, fieldData.size-0.5f);
        navMeshSurface.BuildNavMesh();

        SpawnPlayer(returnAction, gameOverAction);
        SpawnFieldTile();
    }
    public void SpawnNextStage()
    {
        var spawnPos = transform.position + Vector3.up;
        var spawnOBj = Instantiate(DataCenter.Instance.GetExitGate(fieldData.currentStageLevel).gameObject, spawnPos, Quaternion.identity);
        spawnOBj.transform.SetParent(disableField);    
    }
    public void SpawnFieldTile()
    {
        int size = fieldData.size;
        float spacing = size == 2 ? 5 : 3;
        float half = (size - 1) * 0.5f;

        foreach (var info in fieldData.faceinfos)
        {
            float x = (info.Position.x - half) * spacing;
            float z = (info.Position.z - half) * spacing;

            GameObject plane = Instantiate(DataCenter.Instance.GetFaceData(info.Type).FieldMesh.gameObject, Vector3.zero, Quaternion.identity); 
            plane.transform.SetParent(disableField, false);
            plane.transform.localPosition = new Vector3(x, 0.5f, z);
            var scaleSize = 1f / size;
            plane.transform.localScale = new Vector3(scaleSize, 1f, scaleSize);
            plane.GetComponent<FieldTile>().Initialize(fieldData.currentStageLevel, info);   
        }
    }
    public void OnDisableField()
    {
        gameObject.SetActive(false);
        for (int i = disableField.childCount - 1; i >= 0; i--)
        {
            Destroy(disableField.GetChild(i).gameObject);
        }
        spawnedPlayer.gameObject.SetActive(false);  
    }
    private void SpawnPlayer(Action returnAction, Action gameOverAction)
    {
        var spawnPos = transform.position + Vector3.up;
        if (spawnedPlayer == null)
        {

            GameObject playerObj = Instantiate(DataCenter.Instance.GetPlayerEntity(), spawnPos, Quaternion.identity);
            spawnedPlayer = playerObj.GetComponent<PlayerEntity>();
            playerObj.transform.SetParent(transform);
            spawnedPlayer.SetScurivalAction(returnAction, gameOverAction);
        }
        else
        {
            spawnedPlayer.gameObject.SetActive(true);   
            spawnedPlayer.transform.position = spawnPos;
        }
    }
}