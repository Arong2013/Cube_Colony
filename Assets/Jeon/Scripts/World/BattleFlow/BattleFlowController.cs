using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowController : SerializedMonoBehaviour
{
    public static BattleFlowController Instance { get; private set; }

    [TitleGroup("큐브 설정", "스테이지별 큐브 데이터")]
    [LabelText("큐브"), Required]
    [SerializeField] private Cube cube;

    [TitleGroup("큐브 설정")]
    [LabelText("큐브 데이터 맵"), Required]
    [DictionaryDrawerSettings(KeyLabel = "스테이지", ValueLabel = "큐브 데이터")]
    [SerializeField] private Dictionary<int, CubeData> stageCubeDataMap;

    [TitleGroup("필드 설정", "스테이지 생존 모드")]
    [LabelText("필드"), Required]
    [SerializeField] private Field field;


        [TitleGroup("베이스캠프프 설정", "베이스캠프프 모드")]
    [LabelText("캠프프"), Required]
    [SerializeField] private GameObject baseCampPrefab;


    [TitleGroup("필드 설정")]
    [LabelText("스테이지 시간")]
    [MinValue(10)]
    [SerializeField] public float stageTime = 120f;

    [TitleGroup("플레이어 데이터", "플레이어 관련 정보")]
    [ShowInInspector, HideLabel]
    public PlayerData playerData;

    [TitleGroup("큐브 상태 관리", "큐브 지속성 관리")]
    [LabelText("현재 스테이지 큐브 사용 횟수"), ReadOnly]
    [SerializeField] private int cubeUsageCount = 0;

    [TitleGroup("큐브 상태 관리")]
    [LabelText("최대 큐브 사용 횟수")]
    [SerializeField] private int maxCubeUsage = 3;

    [TitleGroup("큐브 상태 관리")]
    [LabelText("총 탐험 횟수"), ReadOnly]
    [SerializeField] private int totalExplorationCount = 0;

    [TitleGroup("큐브 상태 관리")]
    [LabelText("현재 큐브 데이터"), ReadOnly]
    [SerializeField] private CubeData currentCubeData;

    [TitleGroup("큐브 상태 관리")]
    [LabelText("사용된 페이스 목록"), ReadOnly]
    [SerializeField] private List<Vector3> usedFacePositions = new List<Vector3>();

    [TitleGroup("에너지 설정", "에너지 관련 설정")]
    [LabelText("에너지 회복 속도")]
    [SerializeField] private float energyRegenRate = 1f; // 초당 회복량

    [TitleGroup("에너지 설정")]
    [LabelText("에너지 자동 회복 사용")]
    [SerializeField] private bool useEnergyRegen = true;

    

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private int currentStage = 1;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private IGameSequenceState currentState;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private List<IObserver> observers = new List<IObserver>();

    public int CurrentStage => currentStage;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 플레이어 데이터 초기화 (게임 시작 시 한 번만)
        playerData = new PlayerData();

        // 큐브 상태 초기화
        cubeUsageCount = 0;
        usedFacePositions = new List<Vector3>();
    }

    private void Start()
    {
        InitializeFirstStage();
    }

    private void Update()
    {
        currentState?.Update();
    }

    /// <summary>
    /// 첫 번째 스테이지를 초기화합니다.
    /// </summary>
    private void InitializeFirstStage()
    {
        if (stageCubeDataMap.ContainsKey(currentStage))
        {
            // 첫 번째 스테이지에서는 새로운 큐브 생성
            currentCubeData = stageCubeDataMap[currentStage];
            cubeUsageCount = 0;
            usedFacePositions.Clear();

            ChangeState(new CountdownState(cube, currentCubeData));
        }
        else
        {
            Debug.LogError($"스테이지 {currentStage}에 대한 큐브 데이터가 없습니다.");
        }
    }

    /// <summary>
    /// 게임 상태를 변경합니다.
    /// </summary>
    /// <param name="newState">새로운 게임 상태</param>
public void ChangeState(IGameSequenceState newState)
{
    // 이전 상태 Exit
    currentState?.Exit();
    
    // 새로운 상태 설정
    currentState = newState;
    
    // 오브젝트 활성화/비활성화 로직 추가
    if (newState is CountdownState)
    {
        // 카운트다운 상태
        cube.gameObject.SetActive(true);
        field.gameObject.SetActive(false);
        baseCampPrefab.SetActive(false);
    }
    else if (newState is InSurvivalState)
    {
        // 서바이벌 상태
        cube.gameObject.SetActive(false);
        field.gameObject.SetActive(true);
        baseCampPrefab.SetActive(false);
    }
    else if (newState is CompleteState)
    {
        // 컴플리트 상태
        cube.gameObject.SetActive(false);
        field.gameObject.SetActive(false);
        baseCampPrefab.SetActive(true);
    }

    // 새로운 상태 Enter
    currentState.Enter();
}

    /// <summary>
    /// 필드 인스턴스를 반환합니다.
    /// </summary>
    public Field GetField() => field;

    /// <summary>
    /// 플레이어 엔티티를 반환합니다.
    /// </summary>
    public PlayerEntity GetPlayerEntity()
    {
        return FindObjectOfType<PlayerEntity>();
    }

    /// <summary>
    /// 총 탐험 횟수를 반환합니다.
    /// </summary>
    public int GetTotalExplorationCount()
    {
        return totalExplorationCount;
    }

    /// <summary>
    /// 총 탐험 횟수를 증가시킵니다.
    /// </summary>
    private void IncrementTotalExplorationCount()
    {
        totalExplorationCount++;
        NotifyObservers();
    }

    /// <summary>
    /// 생존 상태로 설정하고 사용된 페이스를 기록합니다.
    /// </summary>
    public void SetInSurvivalState()
    {
        var topFaces = cube.GetTopCubieFace();

        // 현재 TOP 페이스 위치들을 사용된 페이스로 기록
        foreach (var faceInfo in topFaces)
        {
            if (!usedFacePositions.Contains(faceInfo.Position))
            {
                usedFacePositions.Add(faceInfo.Position);
            }
        }

        cubeUsageCount++;
        IncrementTotalExplorationCount(); // 총 탐험 횟수 증가
        Debug.Log($"큐브 사용 횟수: {cubeUsageCount}/{maxCubeUsage}, 총 탐험 횟수: {totalExplorationCount}");

        ChangeState(new InSurvivalState(this, currentCubeData, topFaces));
    }

    /// <summary>
    /// 다음 스테이지의 카운트다운 상태로 설정합니다.
    /// </summary>
    public void SetCountDownState()
    {
        // 큐브를 3번 사용했다면 다음 스테이지로
        if (cubeUsageCount >= maxCubeUsage)
        {
            currentStage++;

            if (stageCubeDataMap.ContainsKey(currentStage))
            {
                // 새로운 스테이지: 새로운 큐브 데이터와 상태 초기화
                // ✅ 플레이어 데이터는 유지하고 큐브 상태만 초기화
                currentCubeData = stageCubeDataMap[currentStage];
                cubeUsageCount = 0;
                usedFacePositions.Clear();

                Debug.Log($"<color=green>스테이지 {currentStage}로 진행! 플레이어 데이터 유지</color>");
                ChangeState(new CountdownState(cube, currentCubeData));
            }
            else
            {
                // ✅ 모든 스테이지 클리어 시 완료 상태로 (플레이어 데이터 유지)
                Debug.Log($"<color=gold>모든 스테이지 클리어! 게임 완료</color>");
                ChangeState(new CompleteState(this));
            }
        }
        else
        {
            // 같은 스테이지에서 계속: 기존 큐브 데이터 유지하고 사용된 페이스는 몬스터 타일로 변경
            // ✅ 플레이어 데이터는 당연히 유지
            Debug.Log($"<color=cyan>스테이지 {currentStage} 내에서 다음 큐브 탐색 ({cubeUsageCount}/{maxCubeUsage})</color>");
            ChangeState(new CountdownState(cube, currentCubeData, usedFacePositions));
        }
    }
public int GetTotalStageCount()
{
    return stageCubeDataMap.Count;
}
    /// <summary>
    /// 게임오버 상태로 설정합니다. (플레이어가 죽었을 때만 호출)
    /// </summary>
 public void SetGameOverState()
{
    // 베이스캠프로 이동 (게임오버로)
    ChangeState(new CompleteState(this));

}

    /// <summary>
    /// 게임을 완전히 초기화합니다. (게임 재시작 시 사용)
    /// </summary>
    [Button("게임 완전 초기화")]
    public void ResetGame()
    {
        Debug.Log($"<color=yellow>게임 완전 초기화</color>");

        currentStage = 1;
        playerData.FullReset(); // 초기화 (골드, 장비 등 모두 초기화완전 )

        // 큐브 상태 초기화
        cubeUsageCount = 0;
        usedFacePositions.Clear();
        totalExplorationCount = 0;

        if (stageCubeDataMap.ContainsKey(currentStage))
        {
            currentCubeData = stageCubeDataMap[currentStage];
            ChangeState(new CountdownState(cube, currentCubeData));
        }
    }

    /// <summary>
    /// 사용된 페이스 위치 목록을 반환합니다.
    /// </summary>
    public List<Vector3> GetUsedFacePositions()
    {
        return new List<Vector3>(usedFacePositions);
    }

    /// <summary>
    /// 큐브 사용 횟수를 반환합니다.
    /// </summary>
    public int GetCubeUsageCount()
    {
        return cubeUsageCount;
    }

    /// <summary>
    /// 최대 큐브 사용 횟수를 반환합니다.
    /// </summary>
    public int GetMaxCubeUsage()
    {
        return maxCubeUsage;
    }

    /// <summary>
    /// 에너지 소모 (큐브 회전 등)
    /// </summary>
    public bool TryConsumeEnergy(float amount)
    {
        if (playerData != null && playerData.TryConsumeEnergy(amount))
        {
            NotifyObservers();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 골드 소모 (강화 등)
    /// </summary>
    public bool TrySpendGold(int amount)
    {
        if (playerData != null && playerData.TrySpendGold(amount))
        {
            NotifyObservers();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 골드 획득
    /// </summary>
    public void AddGold(int amount)
    {
        if (playerData != null)
        {
            playerData.AddGold(amount);
            NotifyObservers();
            Debug.Log($"골드 {amount} 획득! 현재: {playerData.gold}");
        }
    }
    /// <summary>
    /// 옵저버를 등록합니다.
    /// </summary>
    public void RegisterObserver(IObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    /// <summary>
    /// 옵저버를 해제합니다.
    /// </summary>
    public void UnregisterObserver(IObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    /// <summary>
    /// 모든 옵저버에게 상태 변경을 알립니다.
    /// </summary>
    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.UpdateObserver();
        }
    }
        // 기존 코드에 추가
    public bool IsAllStagesCompleted()
    {
        // 현재 스테이지가 최대 스테이지 수와 같거나 큰지 확인
        return currentStage >= stageCubeDataMap.Count;
    }

    // InSurvivalState에서 귀환 시 호출될 메서드
    public void CheckStageCompletionOnReturn()
    {
        if (IsAllStagesCompleted())
        {
            SetCompleteState();
        }
        else
        {
            // 기존 카운트다운 상태로 전환
            SetCountDownState();
        }
    }
public void SetCompleteState()
{
    // 현재 스테이지가 최대 스테이지를 넘었는지 확인
    if (currentStage < stageCubeDataMap.Count)
    {
        currentStage++; // 다음 스테이지로 진행
    }

    // 큐브 상태 초기화
    cubeUsageCount = 0;
    usedFacePositions.Clear();

    // 컴플리트 상태로 변경
    ChangeState(new CompleteState(this));

    // 옵저버들에게 상태 변경 알림
    NotifyObservers();
}

    
}