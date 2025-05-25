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
    [LabelText("귀환 진행 바"), Required]
    [SerializeField] private BarUI returnProgressBar;

    [TitleGroup("생존 관련 UI")]
    [LabelText("귀환 바 컨테이너"), Tooltip("귀환 진행바를 포함한 부모 UI")]
    [SerializeField] private GameObject returnBarContainer;

    [TitleGroup("생존 관련 UI")]
    [LabelText("귀환 중단 시 효과음"), Tooltip("귀환 중단 시 재생될 효과음")]
    [SerializeField] private AudioClip returnInterruptSound;

    [TitleGroup("생존 관련 UI")]
    [LabelText("귀환 완료 효과음"), Tooltip("귀환 완료 시 재생될 효과음")]
    [SerializeField] private AudioClip returnCompleteSound;

    [TitleGroup("디버그 정보")]
    [ReadOnly]
    [ShowInInspector]
    private bool isReturning = false;

    // 오디오 소스 컴포넌트 
    private AudioSource audioSource;

    private void Awake()
    {
        // 오디오 소스 컴포넌트 추가 (없는 경우)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // 귀환 바 컨테이너가 지정되지 않은 경우
        if (returnBarContainer == null && returnProgressBar != null)
        {
            returnBarContainer = returnProgressBar.gameObject;
        }
    }

    private void Start()
    {
        if (BattleFlowController.Instance != null)
        {
            // 옵저버로 등록
            BattleFlowController.Instance.RegisterObserver(this);
        }

        // BarUI 컴포넌트 초기화 확인
        if (hpBar == null || o2Bar == null || engBar == null || returnProgressBar == null)
        {
            Debug.LogError("InSurvivalStateUI: BarUI 컴포넌트가 할당되지 않았습니다.");
        }
        else
        {
            // 바 타입 설정
            hpBar.SetBarType(BarUI.BarType.Health);
            o2Bar.SetBarType(BarUI.BarType.Oxygen);
            engBar.SetBarType(BarUI.BarType.Energy);
            returnProgressBar.SetBarType(BarUI.BarType.Custom);
        }

        // 귀환 바는 처음에 비활성화
        if (returnBarContainer != null)
        {
            returnBarContainer.SetActive(false);
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
        // UI 비활성화 시 귀환 바도 비활성화
        if (returnBarContainer != null)
        {
            returnBarContainer.SetActive(false);
        }
        isReturning = false;
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        isReturning = false;

        // 귀환 바 초기 상태로 설정
        if (returnBarContainer != null)
        {
            returnBarContainer.SetActive(false);
        }

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

    [Button("귀환 시작")]
    public void EnterReturn()
    {
        if (returnBarContainer != null)
        {
            returnBarContainer.SetActive(true);
        }
        isReturning = true;

        // 귀환 시작 시 진행 바 초기화
        if (returnProgressBar != null)
        {
            returnProgressBar.SetValue(100, 100);
        }

        Debug.Log("<color=cyan>귀환 시작: 진행 바 활성화</color>");
    }

    [Button("귀환 종료")]
public void ExitReturn()
{
    if (returnBarContainer != null)
    {
        returnBarContainer.SetActive(false);
    }
    isReturning = false;

    // 귀환 완료 효과음 재생 (귀환 진행 중이었을 경우에만)
    if (isReturning && returnCompleteSound != null && audioSource != null)
    {
        audioSource.PlayOneShot(returnCompleteSound);
    }

    // 귀환 진행 바 초기화
    if (returnProgressBar != null)
    {
        returnProgressBar.SetValue(0, 100); // 바를 완전히 초기화
    }

    Debug.Log("<color=cyan>귀환 종료: 진행 바 비활성화 및 초기화</color>");
}

    public void UpdateReturn(float maxTime, float currentTime)
    {
        if (!isReturning) return;

        float progress = currentTime / maxTime;
        if (returnProgressBar != null)
        {
            returnProgressBar.SetValue(progress * 100, 100);
        }
    }

    [Button("귀환 진행 바 초기화")]
    public void ResetReturnProgress()
    {
        // 귀환 진행 바가 존재하면 100%로 설정
        if (returnProgressBar != null)
        {
            returnProgressBar.SetValue(100, 100);
        }

        // 귀환 중단 효과음 재생
        if (returnInterruptSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(returnInterruptSound);
        }

        // 귀환 진행을 초기화했음을 로그로 알림
        Debug.Log("<color=orange>귀환 중단됨: 진행 바 초기화</color>");
    }
}