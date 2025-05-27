using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class InCountDownStateUI : MonoBehaviour, IObserver
{
    [TitleGroup("상태 표시 UI")]
    [LabelText("체력 바"), Required]
    [SerializeField] private BarUI hpBar;

    [TitleGroup("상태 표시 UI")]
    [LabelText("산소 바"), Required]
    [SerializeField] private BarUI o2Bar;

    [TitleGroup("상태 표시 UI")]
    [LabelText("에너지 바"), Required]
    [SerializeField] private BarUI engBar;

    [TitleGroup("게임 진행 UI")]
    [LabelText("큐브 사용 횟수 텍스트"), Required]
    [SerializeField] private TMPro.TextMeshProUGUI cubeUsageCountText;

      [TitleGroup("게임 진행 UI")]
    [LabelText("큐브 사용 애니메이션 "), Required]
    [SerializeField] private Animator cubeUsageAnimator;

    [TitleGroup("게임 진행 UI")]
    [LabelText("총 탐험 횟수 텍스트"), Required]
    [SerializeField] private TMPro.TextMeshProUGUI explorationCountText;

    [TitleGroup("게임 진행 UI")]
    [LabelText("총 탐험 진행 바"), Required, Tooltip("전체 탐험 진행률을 표시")]
    [SerializeField] private BarUI explorationProgressBar;

    [TitleGroup("게임 진행 UI")]
    [LabelText("생존 시작 버튼"), Required]
    [SerializeField] private Button survivalStartBtn;

    [TitleGroup("게임 진행 UI")]
    [LabelText("큐브 컨트롤러"), Required]
    [SerializeField] private CubeControllerUI cubeControllerUI;

    [TitleGroup("게임 진행 UI")]
    [LabelText("베이스캠프 이동 버튼"), Required]
    [SerializeField] private Button baseCampButton;



    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private Action survivalStartAction;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private Action<Cubie, CubeAxisType, bool> cubeControlAction;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private Func<bool> canRotate;



    private void Start()
    {
        if (BattleFlowController.Instance != null)
        {
            // 옵저버로 등록
            BattleFlowController.Instance.RegisterObserver(this);
        }

        // BarUI 컴포넌트 초기화 확인
        if (hpBar == null || o2Bar == null || engBar == null)
        {
            Debug.LogError("InCountDownStateUI: 필수 BarUI 컴포넌트가 할당되지 않았습니다.");
        }
        else
        {
            // 바 타입 설정
            hpBar.SetBarType(BarUI.BarType.Health);
            o2Bar.SetBarType(BarUI.BarType.Oxygen);
            engBar.SetBarType(BarUI.BarType.Energy);


            // 탐험 진행 바 설정
            if (explorationProgressBar != null)
            {
                explorationProgressBar.SetBarType(BarUI.BarType.Custom);
            }
        }

        // 생존 시작 버튼 이벤트 연결
        if (survivalStartBtn != null)
        {
            survivalStartBtn.onClick.AddListener(SurvivalStartAction);
        }

        if (baseCampButton != null)
        {
            baseCampButton.onClick.AddListener(MoveToBaseCamp);
        }

    }

    private void OnDestroy()
    {
        if (BattleFlowController.Instance != null)
        {
            // 옵저버 해제
            BattleFlowController.Instance.UnregisterObserver(this);
        }
    }

    public void Initialize(Action survivalStartAction, Action<Cubie, CubeAxisType, bool> cubeControlAction, Func<bool> canRotate)
    {
        this.survivalStartAction = survivalStartAction;
        this.cubeControlAction = cubeControlAction;
        this.canRotate = canRotate;

        if (cubeControllerUI != null)
        {
            cubeControllerUI.SetRotateAction(cubeControlAction);
            cubeControllerUI.gameObject.SetActive(true);
        }

        // 게임 오브젝트 활성화
        gameObject.SetActive(true);

        // 게임 오브젝트가 활성화된 후에 상태 업데이트
        UpdateStatUI();
        UpdateCubeUsageProgress();
        UpdateExplorationCount();
        UpdateExplorationProgress();

        if (cubeUsageCountText != null && BattleFlowController.Instance != null)
        {
            cubeUsageCountText.text = $"1/{BattleFlowController.Instance.GetMaxCubeUsage()}";
        }
    }

    public void Disable()
    {
        if (cubeControllerUI != null)
        {
            cubeControllerUI.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void UpdateObserver()
    {
        UpdateStatUI();
        UpdateCubeUsageProgress();
        UpdateExplorationCount();
        UpdateExplorationProgress();
    }

    private void UpdateStatUI()
    {
        // BarUI 컴포넌트 자체에서 업데이트 처리
        if (hpBar != null && hpBar.gameObject.activeInHierarchy)
            hpBar.UpdateValueFromPlayerData();

        if (o2Bar != null && o2Bar.gameObject.activeInHierarchy)
            o2Bar.UpdateValueFromPlayerData();

        if (engBar != null && engBar.gameObject.activeInHierarchy)
            engBar.UpdateValueFromPlayerData();
    }

    /// <summary>
    /// 큐브 사용 진행률 업데이트
    /// </summary>
    private void UpdateCubeUsageProgress()
    {
        if (BattleFlowController.Instance == null) return;

        int currentUsage = BattleFlowController.Instance.GetCubeUsageCount();
        int maxUsage = BattleFlowController.Instance.GetMaxCubeUsage();

        // 큐브 사용 횟수 텍스트 업데이트
        if (cubeUsageCountText != null)
        {
            cubeUsageCountText.text = $"{currentUsage}/{maxUsage}";

            // 사용 횟수에 따라 텍스트 색상 변경
            if (currentUsage >= maxUsage)
            {
                cubeUsageCountText.color = Color.red; // 최대 사용 횟수 도달
            }
            else if (currentUsage > 0)
            {
                cubeUsageCountText.color = Color.yellow; // 사용 중
            }
            else
            {
                cubeUsageCountText.color = Color.white; // 초기 상태
            }
        }
        cubeUsageAnimator.SetInteger("currentUsage", currentUsage); 
    }

    /// <summary>
    /// 총 탐험 횟수 업데이트
    /// </summary>
    private void UpdateExplorationCount()
    {
        if (BattleFlowController.Instance == null) return;

        // 총 탐험 횟수 표시
        if (explorationCountText != null)
        {
            int totalExplorations = BattleFlowController.Instance.CurrentStage;
            explorationCountText.text = $"총 탐험 횟수: {totalExplorations}";
        }
    }

    /// <summary>
    /// 총 탐험 진행률 업데이트
    /// </summary>
    private void UpdateExplorationProgress()
    {
        if (BattleFlowController.Instance == null) return;

        int totalExplorations = BattleFlowController.Instance.CurrentStage;
        int totalStages = BattleFlowController.Instance.GetTotalStageCount();

        // 탐험 진행 바 업데이트
        if (explorationProgressBar != null && explorationProgressBar.gameObject.activeInHierarchy)
        {
            // 전체 스테이지 수 대비 현재 탐험 횟수의 비율 계산
            float progressRatio = Mathf.Clamp01((float)totalExplorations / totalStages);
            explorationProgressBar.SetValue(totalExplorations, totalStages);
        }
    }


    public void UpdateExplorationProgress(float progress)
    {
        // 이 메서드는 이제 UpdateCubeUsageProgress와 UpdateExplorationProgress로 대체됨
        UpdateCubeUsageProgress();
        UpdateExplorationProgress();
    }

    public void DisableCubeController()
    {
        if (cubeControllerUI != null)
        {
            cubeControllerUI.gameObject.SetActive(false);
        }
    }

    public void RotateCubeAction(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        if (cubeControlAction != null)
        {
            cubeControlAction.Invoke(selectedCubie, axis, isClock);
        }

        // 회전 불가능한 상태라면 큐브 컨트롤러 비활성화
        if (canRotate != null && !canRotate())
        {
            DisableCubeController();
        }
    }

    public void SurvivalStartAction()
    {
        if (survivalStartAction != null)
        {
            survivalStartAction.Invoke();
        }
    }
    

        private void MoveToBaseCamp()
    {
        // 베이스캠프로 이동
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.SetCompleteState(false);
        }
    }

}