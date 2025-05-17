using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class InSurvivalStateUI : MonoBehaviour, IObserver
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

    [TitleGroup("생존 관련 UI")]
    [LabelText("생존 상태 바"), Required]
    [SerializeField] private BarUI survivalStateBar;

    [TitleGroup("디버그 정보")]
    [ReadOnly]
    [ShowInInspector]
    private bool isReturning = false;

    private void Start()
    {
        if (BattleFlowController.Instance != null)
        {
            // 옵저버로 등록
            BattleFlowController.Instance.RegisterObserver(this);
        }

        // BarUI 컴포넌트 초기화 확인
        if (hpBar == null || o2Bar == null || engBar == null || survivalStateBar == null)
        {
            Debug.LogError("InSurvivalStateUI: BarUI 컴포넌트가 할당되지 않았습니다.");
        }
        else
        {
            // 바 타입 설정
            hpBar.SetBarType(BarUI.BarType.Health);
            o2Bar.SetBarType(BarUI.BarType.Oxygen);
            engBar.SetBarType(BarUI.BarType.Energy);
            survivalStateBar.SetBarType(BarUI.BarType.Custom);
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

    private void OnEnable()
    {
        UpdateStatUI();
    }

    private void OnDisable()
    {
        survivalStateBar.gameObject.SetActive(false);
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        isReturning = false;
        UpdateStatUI();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void UpdateObserver()
    {
        UpdateStatUI();
    }

    private void UpdateStatUI()
    {
        // BarUI 컴포넌트 자체에서 업데이트 처리
        hpBar?.UpdateValueFromPlayerData();
        o2Bar?.UpdateValueFromPlayerData();
        engBar?.UpdateValueFromPlayerData();
    }

    public void EnterReturn()
    {
        survivalStateBar.gameObject.SetActive(true);
        isReturning = true;
    }

    public void ExitReturn()
    {
        survivalStateBar.gameObject.SetActive(false);
        isReturning = false;
    }

    public void UpdateReturn(float maxTime, float currentTime)
    {
        if (!isReturning) return;

        float progress = currentTime / maxTime;
        survivalStateBar.SetValue(progress * 100, 100);
    }
}