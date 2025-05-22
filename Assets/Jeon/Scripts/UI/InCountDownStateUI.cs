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
    [LabelText("큐브 사용 진행 바"), Required, Tooltip("현재 큐브의 사용 횟수를 표시")]
    [SerializeField] private BarUI cubeUsageBar;

    [TitleGroup("게임 진행 UI")]
    [LabelText("생존 시작 버튼"), Required]
    [SerializeField] private Button survivalStartBtn;

    [TitleGroup("게임 진행 UI")]
    [LabelText("큐브 컨트롤러"), Required]
    [SerializeField] private CubeControllerUI cubeControllerUI;

    [TitleGroup("진행 상태 UI")]
    [LabelText("사용 횟수 텍스트"), Required, Tooltip("현재/최대 사용 횟수 표시")]
    [SerializeField] private TMPro.TextMeshProUGUI usageCountText;

    [ShowInInspector, ReadOnly]
    [TitleGroup("디버그 정보")]
    private Action survivalStartAction;

    [ShowInInspector, ReadOnly]
    [TitleGroup("디버그 정보")]
    private Action<Cubie, CubeAxisType, bool> cubeControlAction;

    [ShowInInspector, ReadOnly]
    [TitleGroup("디버그 정보")]
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

            // 큐브 사용 진행 바 설정
            if (cubeUsageBar != null)
            {
                cubeUsageBar.SetBarType(BarUI.BarType.Custom);
            }
        }

        // 생존 시작 버튼 이벤트 연결
        if (survivalStartBtn != null)
        {
            survivalStartBtn.onClick.AddListener(SurvivalStartAction);
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

        // 큐브 사용 진행률 업데이트
        UpdateCubeUsageProgress();
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

        // 진행 바 업데이트
        if (cubeUsageBar != null && cubeUsageBar.gameObject.activeInHierarchy)
        {
            cubeUsageBar.SetValue(currentUsage, maxUsage);
        }

        // 사용 횟수 텍스트 업데이트
        if (usageCountText != null)
        {
            usageCountText.text = $"큐브 사용: {currentUsage}/{maxUsage}";

            // 사용 횟수에 따라 텍스트 색상 변경
            if (currentUsage >= maxUsage)
            {
                usageCountText.color = Color.red; // 완료
            }
            else if (currentUsage > 0)
            {
                usageCountText.color = Color.yellow; // 진행 중
            }
            else
            {
                usageCountText.color = Color.white; // 시작 전
            }
        }
    }

    public void UpdateExplorationProgress(float progress)
    {
        // 이 메서드는 이제 UpdateCubeUsageProgress로 대체됨
        UpdateCubeUsageProgress();
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
}