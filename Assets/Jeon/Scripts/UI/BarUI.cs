using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;

/// <summary>
/// 플레이어의 다양한 스탯을 표시하는 바 UI 컴포넌트
/// </summary>
public class BarUI : MonoBehaviour
{
    [BoxGroup("기본 설정", order: 0)]
    [LabelText("바 타입"), Tooltip("표시할 스탯의 종류")]
    [EnumToggleButtons]
    [OnValueChanged("UpdateValueFromPlayerData")]
    [SerializeField] private BarType barType = BarType.Health;

    [BoxGroup("UI 요소", order: 1)]
    [HorizontalGroup("UI 요소/Bar", LabelWidth = 90)]
    [LabelText("바 이미지"), Required("바 이미지는 필수입니다")]
    [PreviewField(80)]
    [SerializeField] private Image fillImage;

    [HorizontalGroup("UI 요소/Bar")]
    [OnValueChanged("UpdateShowPercentageOption")]
    [LabelText("텍스트 사용"), LabelWidth(90), Tooltip("퍼센트 텍스트 표시 여부")]
    [SerializeField] private bool usePercentText = true;

    [BoxGroup("UI 요소", order: 1)]
    [HorizontalGroup("UI 요소/Text", LabelWidth = 90)]
    [LabelText("텍스트"), ShowIf("usePercentText")]
    [Required("텍스트가 필요합니다"), InfoBox("텍스트 컴포넌트를 할당해주세요", InfoMessageType.Warning, "@usePercentText && percentText == null")]
    [PreviewField(80)]
    [SerializeField] private TextMeshProUGUI percentText;

    [HorizontalGroup("UI 요소/Text")]
    [LabelText("폰트 크기"), ShowIf("usePercentText"), LabelWidth(90)]
    [Range(10, 60)]
    [SerializeField] private int fontSize = 30;

    [BoxGroup("색상 설정", order: 2)]
    [InlineButton("@useColorChange = !useColorChange", "토글")]
    [LabelText("색상 변화 사용"), LabelWidth(120)]
    [SerializeField] private bool useColorChange = false;

    [BoxGroup("색상 설정", order: 2)]
    [ShowIf("useColorChange")]
    [TableList(ShowIndexLabels = false)]
    [SerializeField]
    private List<ColorThreshold> colorThresholds = new List<ColorThreshold>()
    {
        new ColorThreshold() { threshold = 0.66f, color = Color.green, label = "높음" },
        new ColorThreshold() { threshold = 0.33f, color = Color.yellow, label = "중간" },
        new ColorThreshold() { threshold = 0f, color = Color.red, label = "낮음" }
    };

    [BoxGroup("애니메이션 설정", order: 3)]
    [InlineButton("@useAnimation = !useAnimation", "토글")]
    [LabelText("애니메이션 사용"), LabelWidth(120)]
    [SerializeField] private bool useAnimation = true;

    [BoxGroup("애니메이션 설정")]
    [LabelText("애니메이션 속도"), ShowIf("useAnimation")]
    [Range(0.1f, 5f)]
    [SerializeField] private float animationSpeed = 2f;

    // 색상 임계값 클래스
    [System.Serializable]
    public class ColorThreshold
    {
        [HorizontalGroup("Group"), LabelWidth(50)]
        [LabelText("레이블")]
        public string label;

        [HorizontalGroup("Group"), LabelWidth(70)]
        [LabelText("임계값")]
        [Range(0, 1)]
        public float threshold;

        [HorizontalGroup("Group"), LabelWidth(50)]
        [LabelText("색상")]
        [ColorPalette]
        public Color color = Color.white;
    }

    // 바 타입을 정의하는 열거형
    public enum BarType
    {
        Health,
        Energy,
        Oxygen,
        Attack,
        Defense,
        Custom
    }

    // 현재 값과 최대 값
    [FoldoutGroup("현재 상태", order: 99), LabelWidth(80)]
    [ProgressBar(0, "@maxValue", 0, 1, 0, Height = 20, ColorGetter = "GetProgressBarColor")]
    [ReadOnly]
    [ShowInInspector]
    private float currentValue;

    [FoldoutGroup("현재 상태"), LabelWidth(80)]
    [ReadOnly]
    [ShowInInspector]
    private float maxValue = 100f;

    // 표시 값 (애니메이션 적용)
    private float displayValue;

    // 코루틴 참조
    private Coroutine animationCoroutine;

    // 텍스트 빌더
    private StringBuilder strBuilder = new StringBuilder(6);

    // 프로그레스 바 색상 (에디터 전용)
    private Color GetProgressBarColor()
    {
        float ratio = currentValue / maxValue;

        if (!useColorChange || colorThresholds == null || colorThresholds.Count == 0)
            return Color.blue;

        // 색상 리스트는 임계값 내림차순 정렬을 가정
        foreach (var threshold in colorThresholds)
        {
            if (ratio >= threshold.threshold)
                return threshold.color;
        }

        return colorThresholds[colorThresholds.Count - 1].color;
    }

    // 초기화
    private void Awake()
    {
        // 필수 컴포넌트 확인
        if (fillImage == null)
        {
            Debug.LogError("BarUI에 fillImage가 할당되지 않았습니다.");
            return;
        }

        UpdateShowPercentageOption();

        // 초기 설정
        displayValue = currentValue;
        UpdateVisuals(true);

        // 폰트 크기 설정
        if (percentText != null)
        {
            percentText.fontSize = fontSize;
        }
    }

    private void OnEnable()
    {
        // BattleFlowController에서 초기 값 가져오기
        if (BattleFlowController.Instance != null && BattleFlowController.Instance.playerData != null)
        {
            UpdateValueFromPlayerData();
        }
    }

    private void UpdateShowPercentageOption()
    {
        if (!usePercentText && percentText != null)
        {
            percentText.gameObject.SetActive(false);
        }
        else if (usePercentText && percentText != null)
        {
            percentText.gameObject.SetActive(true);
            percentText.fontSize = fontSize;
        }
    }

    // 바 타입 설정
    [Button("바 타입 설정", ButtonSizes.Medium), GUIColor(0.3f, 0.7f, 0.9f)]
    public void SetBarType(BarType type)
    {
        barType = type;
        UpdateValueFromPlayerData();
    }

    // 값 업데이트 (외부에서 호출)
    [Button("값 설정", ButtonSizes.Medium), GUIColor("GetButtonColor")]
    public void SetValue(
        [LabelText("현재 값"), SuffixLabel("현재값")] float current,
        [LabelText("최대 값"), SuffixLabel("최대값")] float max = 100f)
    {
        maxValue = Mathf.Max(max, 0.1f); // 0으로 나누기 방지

        // 값이 변경되었는지 확인
        bool valueChanged = currentValue != current;
        currentValue = Mathf.Clamp(current, 0, maxValue);

        // 게임 오브젝트가 활성화된 상태이고 애니메이션을 사용하는 경우에만 코루틴 실행
        if (useAnimation && valueChanged && gameObject.activeInHierarchy)
        {
            // 이전 애니메이션 중지
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            // 새 애니메이션 시작
            animationCoroutine = StartCoroutine(AnimateValue());
        }
        else
        {
            // 게임 오브젝트가 비활성화되었거나 애니메이션을 사용하지 않는 경우 바로 값 설정
            displayValue = currentValue;
            UpdateVisuals();
        }
    }

    // 버튼 색상 지정 (Odin 에디터용)
    private Color GetButtonColor()
    {
        return new Color(0.3f, 0.7f, 0.9f);
    }

    // PlayerData에서 값 가져오기
    [Button("PlayerData에서 갱신", ButtonSizes.Medium), GUIColor(0.3f, 0.9f, 0.3f)]
    public void UpdateValueFromPlayerData()
    {
        if (BattleFlowController.Instance == null || BattleFlowController.Instance.playerData == null)
        {
            Debug.LogWarning("BattleFlowController.Instance 또는 playerData가 null입니다.");
            return;
        }

        // 게임 오브젝트가 비활성화된 상태면 업데이트 건너뛰기
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        PlayerData playerData = BattleFlowController.Instance.playerData;
        EntityStat playerStat = playerData.playerStat;

        switch (barType)
        {
            case BarType.Health:
                SetValue(playerStat.GetStat(EntityStatName.HP), playerStat.GetStat(EntityStatName.MaxHP));
                break;

            case BarType.Energy:
                SetValue(playerData.energy, 100f);
                break;

            case BarType.Oxygen:
                SetValue(playerStat.GetStat(EntityStatName.O2), playerStat.GetStat(EntityStatName.MaxO2));
                break;

            case BarType.Attack:
                SetValue(playerStat.GetStat(EntityStatName.ATK), 100f);
                break;

            case BarType.Defense:
                SetValue(playerStat.GetStat(EntityStatName.DEF), 100f);
                break;

            case BarType.Custom:
                // Custom 타입은 외부에서 직접 SetValue 호출해야 함
                break;
        }
    }

    // 옵저버 업데이트 (IObserver 구현 클래스에서 호출)
    public void OnObserverUpdate()
    {
        UpdateValueFromPlayerData();
    }

    // 값 애니메이션 코루틴
    private IEnumerator AnimateValue()
    {
        float startValue = displayValue;
        float targetValue = currentValue;
        float duration = Mathf.Abs(targetValue - startValue) / maxValue * (1f / animationSpeed);
        duration = Mathf.Max(duration, 0.1f); // 최소 지속 시간 보장
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // 부드러운 보간
            t = Mathf.SmoothStep(0, 1, t);

            displayValue = Mathf.Lerp(startValue, targetValue, t);
            UpdateVisuals();

            yield return null;
        }

        displayValue = targetValue;
        UpdateVisuals();
        animationCoroutine = null;
    }

    // 시각적 요소 업데이트
    private void UpdateVisuals(bool force = false)
    {
        // 채움 이미지 업데이트
        float ratio = displayValue / maxValue;
        fillImage.fillAmount = ratio;

        // 색상 업데이트
        if (useColorChange && colorThresholds != null && colorThresholds.Count > 0)
        {
            // 임계값에 따라 색상 설정 (내림차순 정렬 가정)
            foreach (var threshold in colorThresholds)
            {
                if (ratio >= threshold.threshold)
                {
                    fillImage.color = threshold.color;
                    break;
                }
            }
        }

        // 퍼센트 텍스트 업데이트
        if (percentText != null && usePercentText)
        {
            int percentage = Mathf.RoundToInt(ratio * 100);
            strBuilder.Clear();
            strBuilder.Append(percentage).Append("%");
            percentText.text = strBuilder.ToString();

            // 폰트 크기 업데이트
            if (percentText.fontSize != fontSize)
            {
                percentText.fontSize = fontSize;
            }
        }
    }

    // 기본 색상 설정 초기화
    [FoldoutGroup("테스트 도구", order: 100)]
    [Button("색상 설정 초기화", ButtonSizes.Medium), GUIColor(0.9f, 0.6f, 0.1f)]
    private void ResetColorThresholds()
    {
        colorThresholds = new List<ColorThreshold>()
        {
            new ColorThreshold() { threshold = 0.66f, color = Color.green, label = "높음" },
            new ColorThreshold() { threshold = 0.33f, color = Color.yellow, label = "중간" },
            new ColorThreshold() { threshold = 0f, color = Color.red, label = "낮음" }
        };
    }

    // 에디터 테스트용 메서드들
    [FoldoutGroup("테스트 도구")]
    [HorizontalGroup("테스트 도구/Buttons")]
    [Button("0%", ButtonSizes.Small), GUIColor(0.9f, 0.3f, 0.3f)]
    private void TestSetZero()
    {
        SetValue(0, maxValue);
    }

    [HorizontalGroup("테스트 도구/Buttons")]
    [Button("25%", ButtonSizes.Small), GUIColor(0.9f, 0.6f, 0.3f)]
    private void TestSet25()
    {
        SetValue(maxValue * 0.25f, maxValue);
    }

    [HorizontalGroup("테스트 도구/Buttons")]
    [Button("50%", ButtonSizes.Small), GUIColor(0.9f, 0.9f, 0.3f)]
    private void TestSetHalf()
    {
        SetValue(maxValue * 0.5f, maxValue);
    }

    [HorizontalGroup("테스트 도구/Buttons")]
    [Button("75%", ButtonSizes.Small), GUIColor(0.6f, 0.9f, 0.3f)]
    private void TestSet75()
    {
        SetValue(maxValue * 0.75f, maxValue);
    }

    [HorizontalGroup("테스트 도구/Buttons")]
    [Button("100%", ButtonSizes.Small), GUIColor(0.3f, 0.9f, 0.3f)]
    private void TestSetFull()
    {
        SetValue(maxValue, maxValue);
    }

    [FoldoutGroup("테스트 도구")]
    [Button("랜덤 값", ButtonSizes.Medium), GUIColor(0.5f, 0.5f, 0.9f)]
    private void TestSetRandom()
    {
        SetValue(UnityEngine.Random.Range(0, maxValue), maxValue);
    }
}