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

    [TitleGroup("필드 설정")]
    [LabelText("스테이지 시간")]
    [MinValue(10)]
    [SerializeField] public float stageTime = 120f;

    [TitleGroup("플레이어 데이터", "플레이어 관련 정보")]
    [ShowInInspector, HideLabel]
    public PlayerData playerData { get; private set; }

    [TitleGroup("큐브 상태 관리", "큐브 지속성 관리")]
    [LabelText("큐브 사용 횟수"), ReadOnly]
    [SerializeField] private int cubeUsageCount = 0;

    [TitleGroup("큐브 상태 관리")]
    [LabelText("최대 사용 횟수")]
    [SerializeField] private int maxCubeUsage = 3;

    [TitleGroup("큐브 상태 관리")]
    [LabelText("현재 큐브 데이터"), ReadOnly]
    [SerializeField] private CubeData currentCubeData;

    [TitleGroup("큐브 상태 관리")]
    [LabelText("사용된 페이스 목록"), ReadOnly]
    [SerializeField] private List<Vector3> usedFacePositions = new List<Vector3>();

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

        // 플레이어 데이터 초기화
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
        currentState?.Exit();
        currentState = newState;
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
        Debug.Log($"큐브 사용 횟수: {cubeUsageCount}/{maxCubeUsage}");

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
                currentCubeData = stageCubeDataMap[currentStage];
                cubeUsageCount = 0;
                usedFacePositions.Clear();

                ChangeState(new CountdownState(cube, currentCubeData));
            }
            else
            {
                Debug.LogWarning($"스테이지 {currentStage}에 대한 데이터가 없습니다. 게임 종료 상태로 전환합니다.");
                ChangeState(new CompleteState(this));
            }
        }
        else
        {
            // 같은 스테이지에서 계속: 기존 큐브 데이터 유지하고 사용된 페이스는 몬스터 타일로 변경
            ChangeState(new CountdownState(cube, currentCubeData, usedFacePositions));
        }
    }

    /// <summary>
    /// 게임오버 상태로 설정합니다.
    /// </summary>
    public void SetGameOverState()
    {
        currentStage = 1;
        playerData.Reset(); // 플레이어 데이터 초기화

        // 큐브 상태도 초기화
        cubeUsageCount = 0;
        usedFacePositions.Clear();

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
}