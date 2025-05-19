using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections;

/// <summary>
/// 장비 강화 UI
/// </summary>
public class EquipmentEnhancementUI : MonoBehaviour
{
    [TitleGroup("강화 정보 UI")]
    [LabelText("아이템 이름"), Required]
    [SerializeField] private TextMeshProUGUI itemNameText;

    [TitleGroup("강화 정보 UI")]
    [LabelText("현재 강화 레벨")]
    [SerializeField] private TextMeshProUGUI currentLevelText;

    [TitleGroup("강화 정보 UI")]
    [LabelText("다음 강화 레벨")]
    [SerializeField] private TextMeshProUGUI nextLevelText;

    [TitleGroup("강화 정보 UI")]
    [LabelText("강화 비용 표시")]
    [SerializeField] private TextMeshProUGUI costText;

    [TitleGroup("강화 정보 UI")]
    [LabelText("성공 확률 표시")]
    [SerializeField] private TextMeshProUGUI successRateText;

    [TitleGroup("스탯 비교 UI")]
    [LabelText("현재 스탯 표시")]
    [SerializeField] private Transform currentStatsParent;

    [TitleGroup("스탯 비교 UI")]
    [LabelText("다음 스탯 표시")]
    [SerializeField] private Transform nextStatsParent;

    [TitleGroup("스탯 비교 UI")]
    [LabelText("스탯 비교 프리팹")]
    [SerializeField] private GameObject statComparisonPrefab;

    [TitleGroup("강화 진행 UI")]
    [LabelText("진행 바")]
    [SerializeField] private Slider progressBar;

    [TitleGroup("강화 진행 UI")]
    [LabelText("진행 텍스트")]
    [SerializeField] private TextMeshProUGUI progressText;

    [TitleGroup("결과 표시 UI")]
    [LabelText("결과 팝업")]
    [SerializeField] private GameObject resultPopup;

    [TitleGroup("결과 표시 UI")]
    [LabelText("결과 텍스트")]
    [SerializeField] private TextMeshProUGUI resultText;

    [TitleGroup("결과 표시 UI")]
    [LabelText("결과 이미지")]
    [SerializeField] private Image resultImage;

    [TitleGroup("버튼들")]
    [LabelText("강화 확인 버튼"), Required]
    [SerializeField] private Button confirmButton;

    [TitleGroup("버튼들")]
    [LabelText("취소 버튼"), Required]
    [SerializeField] private Button cancelButton;

    [TitleGroup("버튼들")]
    [LabelText("결과 확인 버튼")]
    [SerializeField] private Button resultConfirmButton;

    [TitleGroup("효과 설정")]
    [LabelText("성공 스프라이트")]
    [SerializeField] private Sprite successSprite;

    [TitleGroup("효과 설정")]
    [LabelText("실패 스프라이트")]
    [SerializeField] private Sprite failureSprite;

    [TitleGroup("효과 설정")]
    [LabelText("강화 진행 시간")]
    [Range(1f, 5f)]
    [SerializeField] private float enhancementDuration = 2f;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private EquipableItem targetItem;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private PlayerEntity targetPlayer;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private bool isEnhancing = false;

    private EquipmentStatusUI parentStatusUI;

    private void Awake()
    {
        // 버튼 이벤트 연결
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmReinforce);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);

        if (resultConfirmButton != null)
            resultConfirmButton.onClick.AddListener(OnResultConfirmClicked);

        // 초기 상태 설정
        gameObject.SetActive(false);
        if (resultPopup != null)
            resultPopup.SetActive(false);
    }

    /// <summary>
    /// 강화 다이얼로그 표시
    /// </summary>
    public void ShowEnhancementDialog(EquipableItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("강화할 아이템이 null입니다.");
            return;
        }

        targetItem = item;
        targetPlayer = Utils.GetPlayer();
        parentStatusUI = GetComponentInParent<EquipmentStatusUI>();

        // UI 업데이트
        UpdateEnhancementInfo();
        UpdateStatComparison();
        UpdateCostAndRate();

        // UI 활성화
        gameObject.SetActive(true);

        // 진행 관련 UI 초기화
        ResetProgressUI();
    }

    /// <summary>
    /// 다이얼로그 숨기기
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        targetItem = null;
        targetPlayer = null;
        parentStatusUI = null;
    }

    /// <summary>
    /// 강화 정보 업데이트
    /// </summary>
    private void UpdateEnhancementInfo()
    {
        if (targetItem == null) return;

        // 아이템 이름
        if (itemNameText != null)
        {
            itemNameText.text = targetItem.ItemName;
        }

        // 현재 강화 레벨
        if (currentLevelText != null)
        {
            currentLevelText.text = $"현재: +{targetItem.currentReinforcementLevel}";
        }

        // 다음 강화 레벨
        if (nextLevelText != null)
        {
            if (targetItem.CanReinforce())
            {
                nextLevelText.text = $"다음: +{targetItem.currentReinforcementLevel + 1}";
            }
            else
            {
                nextLevelText.text = "최대 레벨";
            }
        }
    }

    /// <summary>
    /// 스탯 비교 업데이트
    /// </summary>
    private void UpdateStatComparison()
    {
        if (targetItem == null || statComparisonPrefab == null) return;

        // 기존 스탯 비교 UI 제거
        ClearStatComparisons();

        if (!targetItem.CanReinforce()) return;

        // 현재 스탯 계산
        float currentAttack = targetItem.GetTotalAttackBonus();
        float currentDefense = targetItem.GetTotalDefenseBonus();
        float currentHealth = targetItem.GetTotalHealthBonus();

        // 다음 레벨 스탯 계산
        float nextAttack = targetItem.GetTotalAttackBonusAtLevel(targetItem.currentReinforcementLevel + 1);
        float nextDefense = targetItem.GetTotalDefenseBonusAtLevel(targetItem.currentReinforcementLevel + 1);
        float nextHealth = targetItem.GetTotalHealthBonusAtLevel(targetItem.currentReinforcementLevel + 1);

        // 스탯 비교 UI 생성
        if (currentAttack > 0 || nextAttack > 0)
        {
            CreateStatComparison("공격력", currentAttack, nextAttack);
        }

        if (currentDefense > 0 || nextDefense > 0)
        {
            CreateStatComparison("방어력", currentDefense, nextDefense);
        }

        if (currentHealth > 0 || nextHealth > 0)
        {
            CreateStatComparison("체력", currentHealth, nextHealth);
        }
    }

    /// <summary>
    /// 개별 스탯 비교 UI 생성
    /// </summary>
    private void CreateStatComparison(string statName, float currentValue, float nextValue)
    {
        if (statComparisonPrefab == null) return;

        // 현재 스탯 UI
        if (currentStatsParent != null)
        {
            var currentObj = Instantiate(statComparisonPrefab, currentStatsParent);
            var currentText = currentObj.GetComponentInChildren<TextMeshProUGUI>();
            if (currentText != null)
            {
                currentText.text = $"{statName}: {currentValue:F1}";
            }
        }

        // 다음 스탯 UI
        if (nextStatsParent != null)
        {
            var nextObj = Instantiate(statComparisonPrefab, nextStatsParent);
            var nextText = nextObj.GetComponentInChildren<TextMeshProUGUI>();
            if (nextText != null)
            {
                float increase = nextValue - currentValue;
                nextText.text = $"{statName}: {nextValue:F1} <color=green>(+{increase:F1})</color>";
            }
        }
    }

    /// <summary>
    /// 기존 스탯 비교 UI 제거
    /// </summary>
    private void ClearStatComparisons()
    {
        if (currentStatsParent != null)
        {
            foreach (Transform child in currentStatsParent)
            {
                Destroy(child.gameObject);
            }
        }

        if (nextStatsParent != null)
        {
            foreach (Transform child in nextStatsParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    /// 비용과 성공률 업데이트
    /// </summary>
    private void UpdateCostAndRate()
    {
        if (targetItem == null) return;

        // 강화 비용 표시
        if (costText != null)
        {
            if (targetItem.CanReinforce())
            {
                int cost = targetItem.GetReinforcementCost();
                costText.text = $"비용: {cost} 골드";
            }
            else
            {
                costText.text = "강화 불가";
            }
        }

        // 성공 확률 표시
        if (successRateText != null)
        {
            if (targetItem.CanReinforce())
            {
                float successRate = targetItem.GetReinforcementSuccessRate();
                successRateText.text = $"성공 확률: {successRate:F1}%";
            }
            else
            {
                successRateText.text = "-";
            }
        }
    }

    /// <summary>
    /// 진행 UI 초기화
    /// </summary>
    private void ResetProgressUI()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0f;
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(false);
        }

        if (resultPopup != null)
        {
            resultPopup.SetActive(false);
        }
    }

    /// <summary>
    /// 강화 확인 버튼 클릭
    /// </summary>
    private void OnConfirmReinforce()
    {
        if (targetItem == null || targetPlayer == null || isEnhancing) return;

        if (!targetItem.CanReinforce())
        {
            Debug.LogWarning("강화할 수 없는 아이템입니다.");
            return;
        }

        // 비용 확인 (임시로 플레이어 HP로 대체)
        int cost = targetItem.GetReinforcementCost();
        float currentGold = targetPlayer.GetEntityStat(EntityStatName.HP); // 임시

        if (currentGold < cost)
        {
            Debug.LogWarning("골드가 부족합니다.");
            ShowResult(false, "골드가 부족합니다!");
            return;
        }

        // 강화 진행
        StartCoroutine(EnhancementProcess());
    }

    /// <summary>
    /// 강화 진행 코루틴
    /// </summary>
    private IEnumerator EnhancementProcess()
    {
        isEnhancing = true;

        // 버튼 비활성화
        if (confirmButton != null) confirmButton.interactable = false;
        if (cancelButton != null) cancelButton.interactable = false;

        // 진행 바 표시
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f;
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "강화 중...";
        }

        // 진행 바 애니메이션
        float elapsed = 0f;
        while (elapsed < enhancementDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / enhancementDuration;

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            yield return null;
        }

        // 강화 시도
        bool success = AttemptReinforcement();

        // 결과 표시
        if (success)
        {
            ShowResult(true, "강화 성공!");

            // 부모 UI 새로고침
            if (parentStatusUI != null)
            {
                parentStatusUI.OnReinforcementCompleted();
            }
        }
        else
        {
            ShowResult(false, "강화 실패...");
        }

        isEnhancing = false;
    }

    /// <summary>
    /// 실제 강화 시도
    /// </summary>
    private bool AttemptReinforcement()
    {
        if (targetItem == null || targetPlayer == null) return false;

        // 비용 차감
        int cost = targetItem.GetReinforcementCost();
        targetPlayer.UpdateEntityBaseStat(EntityStatName.HP, -cost); // 임시

        // 성공 여부 판정
        float successRate = targetItem.GetReinforcementSuccessRate();
        bool success = Random.Range(0f, 100f) < successRate;

        if (success)
        {
            // 강화 성공
            targetItem.Reinforce();

            // 장비가 장착되어 있다면 효과 재적용
            var equipmentComponent = targetPlayer.GetEntityComponent<EquipmentComponent>();
            if (equipmentComponent != null && equipmentComponent.GetEquippedItem(targetItem.equipmentType) == targetItem)
            {
                equipmentComponent.RefreshAllEquipmentEffects();
            }

            Debug.Log($"{targetItem.ItemName} 강화 성공! 레벨: +{targetItem.currentReinforcementLevel}");
        }
        else
        {
            Debug.Log($"{targetItem.ItemName} 강화 실패");
        }

        return success;
    }

    /// <summary>
    /// 결과 표시
    /// </summary>
    private void ShowResult(bool success, string message)
    {
        if (resultPopup == null) return;

        resultPopup.SetActive(true);

        // 결과 텍스트 설정
        if (resultText != null)
        {
            resultText.text = message;
            resultText.color = success ? Color.green : Color.red;
        }

        // 결과 이미지 설정
        if (resultImage != null)
        {
            resultImage.sprite = success ? successSprite : failureSprite;
        }

        // 진행 UI 숨기기
        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 취소 버튼 클릭
    /// </summary>
    private void OnCancelClicked()
    {
        if (!isEnhancing)
        {
            Hide();
        }
    }

    /// <summary>
    /// 결과 확인 버튼 클릭
    /// </summary>
    private void OnResultConfirmClicked()
    {
        if (resultPopup != null)
        {
            resultPopup.SetActive(false);
        }

        // 강화가 더 가능한지 확인하고 UI 업데이트
        if (targetItem != null && targetItem.CanReinforce())
        {
            UpdateEnhancementInfo();
            UpdateStatComparison();
            UpdateCostAndRate();

            // 버튼 다시 활성화
            if (confirmButton != null) confirmButton.interactable = true;
            if (cancelButton != null) cancelButton.interactable = true;
        }
        else
        {
            // 더 이상 강화할 수 없으면 다이얼로그 닫기
            Hide();
        }
    }

    /// <summary>
    /// 외부에서 강화 진행 상태 확인
    /// </summary>
    public bool IsEnhancing => isEnhancing;
}