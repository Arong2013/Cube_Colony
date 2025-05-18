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

        // 씬 전환 시에도 BattleFlowController를 유지하려면 아래 라인 추가
        // DontDestroyOnLoad(gameObject);
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
            ChangeState(new CountdownState(cube, stageCubeDataMap[currentStage]));
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
    /// 생존 상태로 설정합니다.
    /// </summary>
    public void SetInSurvivalState()
    {
        // 생존 상태로 전환하는 로직 추가 필요
        if (stageCubeDataMap.ContainsKey(currentStage))
        {
            ChangeState(new InSurvivalState(this, stageCubeDataMap[currentStage], cube.GetTopCubieFace()));
        }
    }

    /// <summary>
    /// 다음 스테이지의 카운트다운 상태로 설정합니다.
    /// </summary>
    public void SetCountDownState()
    {
        currentStage++;

        if (stageCubeDataMap.ContainsKey(currentStage))
        {
            ChangeState(new CountdownState(cube, stageCubeDataMap[currentStage]));
        }
        else
        {
            Debug.LogWarning($"스테이지 {currentStage}에 대한 데이터가 없습니다. 게임 종료 상태로 전환합니다.");
            // 게임 완료 상태로 전환하는 로직 추가 필요
          //  ChangeState(new CompleteState());
        }
    }

    /// <summary>
    /// 게임오버 상태로 설정합니다.
    /// </summary>
    public void SetGameOverState()
    {
        currentStage = 1;
        playerData.Reset(); // 플레이어 데이터 초기화

        if (stageCubeDataMap.ContainsKey(currentStage))
        {
            ChangeState(new CountdownState(cube, stageCubeDataMap[currentStage]));
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
}