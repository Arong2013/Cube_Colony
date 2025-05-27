using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
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
    [LabelText("캠프"), Required]
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
    [SerializeField] private int cubeUsageCount = 0;// 0에서 1로 변경


    [TitleGroup("큐브 상태 관리")]
    [LabelText("최대 큐브 사용 횟수")]
    [SerializeField] private int maxCubeUsage = 3;

    [TitleGroup("큐브 상태 관리")]
    [LabelText("총 탐험 횟수"), ReadOnly]
    [SerializeField] private int currentTotalStage = 0;

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

    [TitleGroup("플레이어 데이터", "플레이어 관련 정보")]
    [ShowInInspector]
    public List<Item> storageItems = new List<Item>();


    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    public IGameSequenceState currentState { get; private set; } // 현재 게임 상태  ;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private List<IObserver> observers = new List<IObserver>();

    public int CurrentStage => currentTotalStage;

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
        if (Input.GetKeyDown(KeyCode.I))
        {
            Utils.GetUI<InventoryUI>()?.ToggleInventoryUI();
        }
    }

    /// <summary>
    /// 첫 번째 스테이지를 초기화합니다.
    /// </summary>
    private void InitializeFirstStage()
    {

        GameObject playerPrefab = DataCenter.Instance.GetPlayerEntity();
        GameObject playerObj = Instantiate(playerPrefab);
        PlayerEntity playerEntity = playerObj.GetComponent<PlayerEntity>();
        playerEntity.gameObject.SetActive(false);
        playerEntity.Init();


        currentTotalStage = 1;

        SetCompleteState(true); // 게임 시작 시 컴플리트 상태로 설정

        // if (stageCubeDataMap.ContainsKey(currentTotalStage))
        // {
        //     // 첫 번째 스테이지에서는 새로운 큐브 생성
        //     currentCubeData = stageCubeDataMap[currentTotalStage];
        //     cubeUsageCount = 0;
        //     usedFacePositions.Clear();

        //     ChangeState(new CountdownState(cube, currentCubeData));
        // }
        // else
        // {
        //     Debug.LogError($"스테이지 {currentTotalStage}에 대한 큐브 데이터가 없습니다.");
        // }
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

        // 오브젝트 및 UI 활성화/비활성화 로직 추가
        if (newState is CountdownState)
        {
            // 카운트다운 상태
            cube.gameObject.SetActive(true);
            field.gameObject.SetActive(false);
            baseCampPrefab.SetActive(false);

            // UI 활성화/비활성화
            Utils.GetUI<InCountDownStateUI>()?.gameObject.SetActive(true);
            Utils.GetUI<InSurvivalStateUI>()?.gameObject.SetActive(false);
            Utils.GetUI<InBaseCampUI>()?.gameObject.SetActive(false);
            Utils.GetUI<InventoryUI>()?.SetActiveFalse();
        }
        else if (newState is InSurvivalState)
        {
            // 서바이벌 상태
            cube.gameObject.SetActive(false);
            field.gameObject.SetActive(true);
            baseCampPrefab.SetActive(false);

            // UI 활성화/비활성화
            Utils.GetUI<InCountDownStateUI>()?.gameObject.SetActive(false);
            Utils.GetUI<InSurvivalStateUI>()?.gameObject.SetActive(true);
            Utils.GetUI<InBaseCampUI>()?.gameObject.SetActive(false);
            Utils.GetUI<InventoryUI>()?.SetActiveFalse();
        }
        else if (newState is CompleteState)
        {
            // 컴플리트 상태
            cube.gameObject.SetActive(false);
            field.gameObject.SetActive(false);
            baseCampPrefab.SetActive(true);

            // UI 활성화/비활성화
            Utils.GetUI<InCountDownStateUI>()?.gameObject.SetActive(false);
            Utils.GetUI<InSurvivalStateUI>()?.gameObject.SetActive(false);
            Utils.GetUI<InBaseCampUI>()?.gameObject.SetActive(true);
            Utils.GetUI<InventoryUI>()?.SetActiveFalse();
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

        ChangeState(new InSurvivalState(this, currentCubeData, topFaces));
    }

    /// <summary>
    /// 창고에 아이템 추가
    /// </summary>
    public bool AddItemToStorage(Item item)
    {
        if (item == null) return false;

        // 소모품인 경우 병합 시도
        if (item is ConsumableItem newConsumable)
        {
            return MergeConsumableItemInStorage(newConsumable);
        }

        storageItems.Add(item);
        NotifyObservers();
        return true;
    }

    /// <summary>
    /// 창고에서 아이템 제거
    /// </summary>
    public void RemoveItemFromStorage(Item item)
    {
        if (item == null) return;

        storageItems.Remove(item);
        NotifyObservers();
    }

    /// <summary>
    /// 창고에 소모품 병합
    /// </summary>
    private bool MergeConsumableItemInStorage(ConsumableItem newConsumable)
    {
        // 같은 ID의 기존 아이템들을 찾음
        var existingItems = storageItems
            .OfType<ConsumableItem>()
            .Where(x => x.ID == newConsumable.ID)
            .ToList();

        int remainingAmount = newConsumable.cunamount;

        foreach (var existingItem in existingItems)
        {
            if (remainingAmount <= 0) break;

            int canAdd = existingItem.maxamount - existingItem.cunamount;
            int toAdd = Mathf.Min(canAdd, remainingAmount);

            if (toAdd > 0)
            {
                existingItem.cunamount += toAdd;
                remainingAmount -= toAdd;
            }
        }

        if (remainingAmount > 0)
        {
            var newItem = newConsumable.Clone() as ConsumableItem;
            if (newItem != null)
            {
                newItem.cunamount = remainingAmount;
                storageItems.Add(newItem);
            }
        }

        NotifyObservers();
        return true;
    }


    /// <summary>
    /// 다음 스테이지의 카운트다운 상태로 설정합니다.
    /// </summary>
    public void SetCountDownState()
    {
        // 귀환할 때마다 스테이지와 큐브 사용 횟수 모두 증가

        cubeUsageCount++;

        // 큐브 사용 횟수가 최대값에 도달하면 새 큐브 데이터로 변경
        if (cubeUsageCount > maxCubeUsage)
        {
            currentTotalStage++;
            if (stageCubeDataMap.ContainsKey(currentTotalStage))
            {
                // 새로운 큐브 데이터로 초기화
                currentCubeData = stageCubeDataMap[currentTotalStage];
                cubeUsageCount = 0; // 큐브 사용 횟수 초기화
                usedFacePositions.Clear(); // 사용된 페이스 목록 초기화

                Debug.Log($"<color=green>스테이지 {currentTotalStage}, 새 큐브 데이터 적용! 플레이어 데이터 유지</color>");
                ChangeState(new CountdownState(cube, currentCubeData));
            }
            else
            {
                // 모든 스테이지 클리어 시 완료 상태로
                Debug.Log($"<color=gold>모든 스테이지 클리어! 게임 완료</color>");
                ChangeState(new CompleteState(this, false)); // 게임 오버 상태로 변경
                return;
            }
        }
        else
        {
            // 같은 큐브 데이터를 계속 사용, 이미 사용된 페이스는 몬스터 타일로 변경
            Debug.Log($"<color=cyan>스테이지 {currentTotalStage}, 큐브 사용 횟수: {cubeUsageCount}/{maxCubeUsage}</color>");
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

    /// <summary>
    /// 게임을 완전히 초기화합니다. (게임 재시작 시 사용)
    /// </summary>
    // [Button("게임 완전 초기화")]
    // public void ResetGame()
    // {
    //     Debug.Log($"<color=yellow>게임 완전 초기화</color>");

    //     currentStage = 1;
    //     playerData.FullReset(); // 초기화 (골드, 장비 등 모두 초기화완전 )

    //     // 큐브 상태 초기화
    //     cubeUsageCount = 0;
    //     usedFacePositions.Clear();
    //     totalExplorationCount = 0;

    //     if (stageCubeDataMap.ContainsKey(currentStage))
    //     {
    //         currentCubeData = stageCubeDataMap[currentStage];
    //         ChangeState(new CountdownState(cube, currentCubeData));
    //     }
    // }

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
        return currentTotalStage > stageCubeDataMap.Count;
    }

    // InSurvivalState에서 귀환 시 호출될 메서드
    public void CheckStageCompletionOnReturn()
    {
        if (IsAllStagesCompleted())
        {
            SetCompleteState(false);
        }
        else
        {
            // 기존 카운트다운 상태로 전환
            SetCountDownState();
        }
    }
    public void SetCompleteState(bool isGameOver)
    {
        cubeUsageCount = 0;
        usedFacePositions.Clear();

        // 컴플리트 상태로 변경
        ChangeState(new CompleteState(this, isGameOver));

        // 옵저버들에게 상태 변경 알림
        NotifyObservers();
    }

    public void StartNewGame()
    {
        cubeUsageCount = 0;
        usedFacePositions.Clear();

        // 첫 번째 스테이지의 큐브 데이터 설정
        if (stageCubeDataMap.ContainsKey(currentTotalStage))
        {
            currentCubeData = stageCubeDataMap[currentTotalStage];
        }
        else
        {
            Debug.LogError($"스테이지 {currentTotalStage}에 대한 큐브 데이터가 없습니다.");
            return;
        }
        ChangeState(new CountdownState(cube, currentCubeData));
        NotifyObservers();
    }
}