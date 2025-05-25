using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class FieldTile : SerializedMonoBehaviour
{
    [TitleGroup("기본 정보")]
    [SerializeField, ReadOnly] protected CubieFaceInfo faceInfo;

    [TitleGroup("기본 정보")]
    [LabelText("스테이지 레벨"), ReadOnly]
    [SerializeField] private int currentStageLevel;

    [TitleGroup("기본 정보")]
    [LabelText("에니메이션 정보"), ReadOnly]
    [SerializeField] Material AnimeMaterial => DataCenter.Instance.GetFaceData(faceInfo.Type).CubieFaceMaterials[faceInfo.Level];

    [TitleGroup("기본 정보")]
    [LabelText("에니메이션 정보")]
    [SerializeField] MeshRenderer AnimeMeshRender;    

    [TitleGroup("스폰 설정")]
    [LabelText("스폰 포인트들"), Tooltip("몬스터가 생성될 위치")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [TitleGroup("디버그")]
    [ReadOnly, ShowInInspector] private FieldTileData tileData;

    [TitleGroup("디버그")]
    [ReadOnly, ShowInInspector] private List<GameObject> spawnedObjects = new List<GameObject>();

    [TitleGroup("디버그")]
    [ReadOnly, ShowInInspector] private int minMonsterCount => tileData?.minCount ?? 0;

    [TitleGroup("디버그")]
    [ReadOnly, ShowInInspector] private int maxMonsterCount => tileData?.maxCount ?? 0;

    public CubieFaceSkillType Type => faceInfo.Type;
    public int MaxLevel => faceInfo.MaxLevel;
    public int CombinedTypeCode => currentStageLevel * 10 + (int)Type;

public void Initialize(int currentStageLevel, CubieFaceInfo info)
{
    this.currentStageLevel = currentStageLevel;
    faceInfo = info;
    
    // Use the new method to find the matching field tile data
    tileData = FindFieldTileData(faceInfo, currentStageLevel);
    
    if (tileData == null)
    {
        Debug.LogError($"적합한 필드 타일 데이터를 찾을 수 없습니다. 스테이지: {currentStageLevel}, 타입: {faceInfo.Type}, 레벨: {faceInfo.Level}");
        return;
    }

    // 이전에 스폰된 오브젝트 모두 제거
    ClearSpawnedObjects();

    // 새로운 오브젝트 스폰
    SpawnObjects();

    AnimeMeshRender.material = AnimeMaterial;   
}

    [Button("오브젝트 스폰"), GUIColor(0.3f, 0.8f, 0.3f)]
    public void SpawnObjects()
    {
        if (tileData == null || tileData.ObjectID == null || tileData.ObjectID.Count == 0)
        {
            Debug.LogWarning("Tile data is missing or empty.");
            return;
        }

        // 먼저 기존 오브젝트 제거
        ClearSpawnedObjects();

        // 스폰 포인트가 없으면 경고
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points defined for this field tile.");
            return;
        }

        // 몬스터 개수 결정 (데이터의 최소/최대 값 사용, 스폰 포인트 수를 최대로 제한)
        int spawnCount = Mathf.Min(
            Random.Range(tileData.minCount, tileData.maxCount + 1),
            spawnPoints.Count
        );

        // 사용할 스폰 포인트 선택 (랜덤하게 섞음)
        List<Transform> shuffledSpawnPoints = new List<Transform>(spawnPoints);
        ShuffleList(shuffledSpawnPoints);

        // 디버그 로그
        Debug.Log($"<color=cyan>몬스터 생성: {spawnCount}개 (최소: {tileData.minCount}, 최대: {tileData.maxCount}, 스폰포인트: {spawnPoints.Count})</color>");

        // 지정된 개수만큼 오브젝트 스폰
        for (int i = 0; i < spawnCount; i++)
        {
            int objectId = GetRandomWeightedObjectID();
            GameObject prefab = DataCenter.Instance.GetEntity(objectId);

            if (prefab == null)
            {
                Debug.LogWarning($"No prefab found for object ID: {objectId}");
                continue;
            }

            // 선택된 스폰 포인트에 오브젝트 생성
            Transform spawnPoint = shuffledSpawnPoints[i];
            GameObject obj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            obj.transform.SetParent(transform);

            // 생성된 오브젝트 추적
            spawnedObjects.Add(obj);

            // 디버그 로그
            Debug.Log($"<color=lime>오브젝트 스폰: ID={objectId}, 위치={spawnPoint.name}</color>");
        }
    }

    // 리스트 섞기 (Fisher-Yates 알고리즘)
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
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

  public FieldTileData FindFieldTileData(CubieFaceInfo faceInfo, int currentFieldLevel) 
{
    // 1. First try to find by field level
    var fieldTileDatasByLevel = DataCenter.Instance.GetFieldTileDatasByFieldLevel(currentFieldLevel);
    
    if (fieldTileDatasByLevel.Count == 0)
    {
        Debug.LogWarning($"필드 레벨 {currentFieldLevel}에 해당하는 필드 타일 데이터가 없습니다.");
        return null;
    }
    
    // 2. Filter by cube face level (TileLevel)
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


    [Button("스폰된 오브젝트 제거"), GUIColor(0.9f, 0.3f, 0.3f)]
    private void ClearSpawnedObjects()
    {
        // 이전에 스폰된 오브젝트 제거
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        spawnedObjects.Clear();
    }

    [Button("스폰 포인트 생성"), GUIColor(0.3f, 0.5f, 0.9f)]
    private void CreateSpawnPoint()
    {
        // 스폰 포인트 생성
        GameObject spawnPoint = new GameObject($"SpawnPoint_{spawnPoints.Count + 1}");
        spawnPoint.transform.SetParent(transform);
        spawnPoint.transform.localPosition = Vector3.up * 0.5f; // 타일 위에 배치

        // 리스트에 추가
        spawnPoints.Add(spawnPoint.transform);

        // 에디터에서 선택
#if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = spawnPoint;
#endif
    }

    [Button("스폰 포인트 랜덤 분포"), GUIColor(0.5f, 0.5f, 0.9f)]
    private void DistributeSpawnPoints()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        var bounds = renderer.bounds;

        // 기존 스폰 포인트 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (child.name.StartsWith("SpawnPoint_"))
            {
                DestroyImmediate(child.gameObject);
            }
        }

        spawnPoints.Clear();

        // 새 스폰 포인트 생성 (5-8개 랜덤)
        int pointCount = Random.Range(5, 9);
        float marginRatio = 0.2f;
        float marginX = bounds.size.x * marginRatio;
        float marginZ = bounds.size.z * marginRatio;

        for (int i = 0; i < pointCount; i++)
        {
            // 랜덤 위치 계산 (타일 내부)
            float x = Random.Range(bounds.min.x + marginX, bounds.max.x - marginX);
            float z = Random.Range(bounds.min.z + marginZ, bounds.max.z - marginZ);
            float y = bounds.max.y + 0.1f;

            // 스폰 포인트 생성
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i + 1}");
            spawnPoint.transform.SetParent(transform);
            spawnPoint.transform.position = new Vector3(x, y, z);

            // 리스트에 추가
            spawnPoints.Add(spawnPoint.transform);
        }

        Debug.Log($"<color=cyan>스폰 포인트 {pointCount}개가 랜덤 생성되었습니다.</color>");
    }

    [Button("스폰 포인트 균등 분포"), GUIColor(0.5f, 0.8f, 0.5f)]
    private void DistributeSpawnPointsEvenly()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        var bounds = renderer.bounds;

        // 기존 스폰 포인트 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (child.name.StartsWith("SpawnPoint_"))
            {
                DestroyImmediate(child.gameObject);
            }
        }

        spawnPoints.Clear();

        // 새 스폰 포인트 그리드 생성 (3x3 = 9개)
        int rows = 3;
        int cols = 3;
        float marginRatio = 0.15f;
        float marginX = bounds.size.x * marginRatio;
        float marginZ = bounds.size.z * marginRatio;

        float xStep = (bounds.size.x - 2 * marginX) / (cols - 1);
        float zStep = (bounds.size.z - 2 * marginZ) / (rows - 1);

        int pointIndex = 1;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // 그리드 위치 계산
                float x = bounds.min.x + marginX + col * xStep;
                float z = bounds.min.z + marginZ + row * zStep;
                float y = bounds.max.y + 0.1f;

                // 위치에 약간의 랜덤성 추가
                x += Random.Range(-0.5f, 0.5f);
                z += Random.Range(-0.5f, 0.5f);

                // 스폰 포인트 생성
                GameObject spawnPoint = new GameObject($"SpawnPoint_{pointIndex++}");
                spawnPoint.transform.SetParent(transform);
                spawnPoint.transform.position = new Vector3(x, y, z);

                // 리스트에 추가
                spawnPoints.Add(spawnPoint.transform);
            }
        }

        Debug.Log($"<color=cyan>스폰 포인트 {rows * cols}개가 균등하게 생성되었습니다.</color>");
    }

    // 인스펙터에서 스폰 포인트 시각화
    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.yellow;
        foreach (var point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawSphere(point.position, 0.3f);
                Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }
    }
}