using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

/// <summary>
/// 장비 현황을 보여주는 UI
/// </summary>
public class EquipmentStatusUI : MonoBehaviour
{
    [TitleGroup("기본 정보 UI")]
    [LabelText("아이템 이름"), Required]
    [SerializeField] private TextMeshProUGUI itemNameText;

    [TitleGroup("기본 정보 UI")]
    [LabelText("아이템 아이콘"), Required]
    [SerializeField] private Image itemIconImage;

    [TitleGroup("기본 정보 UI")]
    [LabelText("아이템 등급 표시")]
    [SerializeField] private Image gradeBackground;

    [TitleGroup("기본 정보 UI")]
    [LabelText("강화 레벨 표시")]
    [SerializeField] private TextMeshProUGUI reinforcementLevelText;

    [TitleGroup("스탯 정보 UI")]
    [LabelText("공격력 텍스트")]
    [SerializeField] private TextMeshProUGUI attackText;

    [TitleGroup("스탯 정보 UI")]
    [LabelText("방어력 텍스트")]
    [SerializeField] private TextMeshProUGUI defenseText;

    [TitleGroup("스탯 정보 UI")]
    [LabelText("체력 텍스트")]
    [SerializeField] private TextMeshProUGUI healthText;

    [TitleGroup("스탯 정보 UI")]
    [LabelText("설명 텍스트")]
    [SerializeField] private TextMeshProUGUI descriptionText;

    [TitleGroup("버튼들")]
    [LabelText("강화 버튼"), Required]
    [SerializeField] private Button reinforceButton;

    [TitleGroup("버튼들")]
    [LabelText("장비 해제 버튼")]
    [SerializeField] private Button unequipButton;

    [TitleGroup("버튼들")]
    [LabelText("닫기 버튼"), Required]
    [SerializeField] private Button closeButton;

    [TitleGroup("하위 UI")]
    [LabelText("강화 UI"), Required]
    [SerializeField] private EquipmentEnhancementUI enhancementUI;

    [TitleGroup("등급별 색상")]
    [LabelText("등급별 색상 설정")]
    [SerializeField] private GradeColorSettings gradeColors;

    [System.Serializable]
    public class GradeColorSettings
    {
        public Color commonColor = Color.white;
        public Color uncommonColor = Color.green;
        public Color rareColor = Color.blue;
        public Color epicColor = Color.magenta;
        public Color legendaryColor = Color.yellow;

        public Color GetColor(ItemGrade grade)
        {
            return grade switch
            {
                ItemGrade.Common => commonColor,
                ItemGrade.Uncommon => uncommonColor,
                ItemGrade.Rare => rareColor,
                ItemGrade.Epic => epicColor,
                ItemGrade.Legendary => legendaryColor,
                _ => commonColor
            };
        }
    }

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private EquipableItem currentItem;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private PlayerEntity targetPlayer;

    private void Awake()
    {
        // 버튼 이벤트 연결
        if (reinforceButton != null)
            reinforceButton.onClick.AddListener(OnReinforceClicked);

        if (unequipButton != null)
            unequipButton.onClick.AddListener(OnUnequipClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        // 초기에는 비활성화
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 장비 현황 표시
    /// </summary>
    public void ShowEquipmentStatus(EquipableItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("표시할 아이템이 null입니다.");
            return;
        }

        currentItem = item;
        targetPlayer = Utils.GetPlayer();

        // UI 업데이트
        UpdateItemInfo();
        UpdateStats();
        UpdateButtons();

        // UI 활성화
        gameObject.SetActive(true);
    }

    /// <summary>
    /// UI 숨기기
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        currentItem = null;
        targetPlayer = null;
    }

    /// <summary>
    /// 아이템 기본 정보 업데이트
    /// </summary>
    private void UpdateItemInfo()
    {
        if (currentItem == null) return;

        // 이름 설정
        if (itemNameText != null)
        {
            itemNameText.text = currentItem.ItemName;
        }

        // 아이콘 설정
        if (itemIconImage != null)
        {
            itemIconImage.sprite = currentItem.ItemIcon;
        }

        // 등급 색상 설정
        if (gradeBackground != null && gradeColors != null)
        {
            gradeBackground.color = gradeColors.GetColor(currentItem.grade);
        }

        // 강화 레벨 표시
        if (reinforcementLevelText != null)
        {
            if (currentItem.currentReinforcementLevel > 0)
            {
                reinforcementLevelText.text = $"+{currentItem.currentReinforcementLevel}";
                reinforcementLevelText.gameObject.SetActive(true);
            }
            else
            {
                reinforcementLevelText.gameObject.SetActive(false);
            }
        }

        // 설명 설정
        if (descriptionText != null)
        {
            descriptionText.text = currentItem.description;
        }
    }

    /// <summary>
    /// 스탯 정보 업데이트
    /// </summary>
    private void UpdateStats()
    {
        if (currentItem == null) return;

        // 기본 스탯 계산
        float baseAttack = currentItem.attackBonus;
        float baseDefense = currentItem.defenseBonus;
        float baseHealth = currentItem.healthBonus;

        // 강화로 인한 추가 스탯 계산
        float reinforcementMultiplier = currentItem.currentReinforcementLevel * 0.1f;
        float reinforcementAttack = baseAttack * reinforcementMultiplier;
        float reinforcementDefense = baseDefense * reinforcementMultiplier;
        float reinforcementHealth = baseHealth * reinforcementMultiplier;

        // 총 스탯
        float totalAttack = baseAttack + reinforcementAttack;
        float totalDefense = baseDefense + reinforcementDefense;
        float totalHealth = baseHealth + reinforcementHealth;

        // 공격력 표시
        if (attackText != null)
        {
            if (reinforcementAttack > 0)
            {
                attackText.text = $"공격력: {baseAttack} <color=green>(+{reinforcementAttack:F1})</color> = {totalAttack:F1}";
            }
            else
            {
                attackText.text = $"공격력: {totalAttack:F1}";
            }
        }

        // 방어력 표시
        if (defenseText != null)
        {
            if (reinforcementDefense > 0)
            {
                defenseText.text = $"방어력: {baseDefense} <color=green>(+{reinforcementDefense:F1})</color> = {totalDefense:F1}";
            }
            else
            {
                defenseText.text = $"방어력: {totalDefense:F1}";
            }
        }

        // 체력 표시
        if (healthText != null)
        {
            if (reinforcementHealth > 0)
            {
                healthText.text = $"체력: {baseHealth} <color=green>(+{reinforcementHealth:F1})</color> = {totalHealth:F1}";
            }
            else
            {
                healthText.text = $"체력: {totalHealth:F1}";
            }
        }
    }

    /// <summary>
    /// 버튼 상태 업데이트
    /// </summary>
    private void UpdateButtons()
    {
        if (currentItem == null) return;

        // 강화 버튼 상태
        if (reinforceButton != null)
        {
            bool canReinforce = currentItem.CanReinforce();
            reinforceButton.interactable = canReinforce;

            // 버튼 텍스트 업데이트
            var buttonText = reinforceButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (canReinforce)
                {
                    int cost = currentItem.GetReinforcementCost();
                    buttonText.text = $"강화 (비용: {cost})";
                }
                else
                {
                    buttonText.text = "강화 불가";
                }
            }
        }

        // 해제 버튼은 항상 활성화
        if (unequipButton != null)
        {
            unequipButton.interactable = true;
        }
    }

    /// <summary>
    /// 강화 버튼 클릭
    /// </summary>
    private void OnReinforceClicked()
    {
        if (currentItem != null && enhancementUI != null)
        {
            enhancementUI.ShowEnhancementDialog(currentItem);
        }
    }

    /// <summary>
    /// 장비 해제 버튼 클릭
    /// </summary>
    private void OnUnequipClicked()
    {
        if (currentItem == null || targetPlayer == null) return;

        var equipmentComponent = targetPlayer.GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            // 장비 해제
            var unequippedItem = equipmentComponent.UnequipItem(currentItem.equipmentType);

            if (unequippedItem != null)
            {
                // 인벤토리에 다시 추가
                targetPlayer.AddItem(unequippedItem);

                // UI 닫기
                Hide();

                Debug.Log($"{unequippedItem.ItemName} 해제되어 인벤토리에 추가됨");
            }
        }
    }

    /// <summary>
    /// 닫기 버튼 클릭
    /// </summary>
    private void OnCloseClicked()
    {
        Hide();
    }

    /// <summary>
    /// 강화 완료 후 UI 새로고침
    /// </summary>
    public void OnReinforcementCompleted()
    {
        if (currentItem != null)
        {
            UpdateItemInfo();
            UpdateStats();
            UpdateButtons();
        }
    }

    /// <summary>
    /// 외부에서 UI 새로고침
    /// </summary>
    [Button("UI 새로고침")]
    public void RefreshUI()
    {
        if (currentItem != null)
        {
            UpdateItemInfo();
            UpdateStats();
            UpdateButtons();
        }
    }
}