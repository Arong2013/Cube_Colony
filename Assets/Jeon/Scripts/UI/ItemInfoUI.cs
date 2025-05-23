using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class ItemInfoUI : SerializedMonoBehaviour
{
    [TitleGroup("정보창 UI 요소")]
    [LabelText("아이템 이름"), Required]
    [SerializeField] private TextMeshProUGUI nameText;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("아이템 설명"), Required]
    [SerializeField] private TextMeshProUGUI descriptionText;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("획득 타일"), Required]
    [SerializeField] private TextMeshProUGUI acquisitionText;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("아이템 이미지"), Required]
    [SerializeField] private Image itemImage;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("사용 버튼"), Required]
    [SerializeField] private Button useButton;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("닫기 버튼"), Required]
    [SerializeField] private Button closeButton;

    [TitleGroup("강화 시스템 UI")]
    [LabelText("강화 패널")]
    [SerializeField] private GameObject reinforcementPanel;

    [TitleGroup("강화 시스템 UI")]
    [LabelText("강화 버튼")]
    [SerializeField] private Button reinforceButton;

    [TitleGroup("강화 시스템 UI")]
    [LabelText("강화 재료 정보 텍스트")]
    [SerializeField] private TextMeshProUGUI reinforcementMaterialText;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector] private Item currentItem;

    private System.Action<Item> onUseItem;

    private void Awake()
    {
        // 닫기 버튼에 이벤트 연결
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        // 사용 버튼에 이벤트 연결
        if (useButton != null)
        {
            useButton.onClick.AddListener(UseItem);
        }

        // 강화 버튼에 이벤트 연결
        if (reinforceButton != null)
        {
            reinforceButton.onClick.AddListener(ReinforceItem);
        }

        // 초기 상태는 비활성화
        gameObject.SetActive(false);
        
        // 강화 패널 초기 비활성화
        if (reinforcementPanel != null)
        {
            reinforcementPanel.SetActive(false);
        }
    }

    public void Initialize()
    {
        Hide();
    }

    public void Show(Item item, System.Action<Item> useCallback = null)
    {
        if (item == null)
        {
            Debug.LogWarning("ItemInfoUI: 표시할 아이템이 없습니다.");
            return;
        }

        currentItem = item;
        onUseItem = useCallback;

        // UI 업데이트
        UpdateUI();

        // 정보창 활성화
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentItem = null;
        onUseItem = null;
        
        if (reinforcementPanel != null)
        {
            reinforcementPanel.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        if (currentItem == null) return;

        // 이름 설정
        if (nameText != null)
            nameText.text = currentItem.ItemName;

        // 설명 설정
        if (descriptionText != null)
            descriptionText.text = currentItem.Description;

        // 획득 타일 설정
        if (acquisitionText != null)
            acquisitionText.text = $"획득: {currentItem.AcquisitionTile}";

        // 이미지 설정
        if (itemImage != null)
            itemImage.sprite = currentItem.ItemIcon;

        // 사용 버튼 활성화 여부 설정
        if (useButton != null)
        {
            bool canUse = currentItem is ConsumableItem || currentItem is EquipableItem;
            useButton.interactable = canUse;
        }

        // 강화 시스템 UI 업데이트 (장비 아이템인 경우만)
        UpdateReinforcementUI();
    }

    private void UpdateReinforcementUI()
    {
        if (reinforcementPanel == null) return;

        // 장비 아이템인 경우에만 강화 패널 표시
        if (currentItem is EquipableItem equipableItem)
        {
            reinforcementPanel.SetActive(true);
            UpdateReinforcementDetails(equipableItem);
        }
        else
        {
            reinforcementPanel.SetActive(false);
        }
    }

    private void UpdateReinforcementDetails(EquipableItem equipableItem)
    {
        bool canReinforce = equipableItem.CanReinforce();

        // 강화 재료 정보 표시 (모든 강화 정보를 하나의 텍스트에 통합)
        if (reinforcementMaterialText != null)
        {
            if (canReinforce)
            {
                string materialInfo = GetReinforcementMaterialInfo(equipableItem);
                reinforcementMaterialText.text = materialInfo;
            }
            else
            {
                reinforcementMaterialText.text = "더 이상 강화할 수 없습니다.";
            }
        }

        // 강화 버튼 활성화 설정
        if (reinforceButton != null)
        {
            // 플레이어 골드와 강화 가능 여부 확인
            int playerGold = BattleFlowController.Instance?.playerData?.gold ?? 0;
            int reinforcementCost = canReinforce ? equipableItem.GetReinforcementCost() : 0;
            bool hasEnoughGold = playerGold >= reinforcementCost;

            reinforceButton.interactable = canReinforce && hasEnoughGold;
        }
    }

    private string GetReinforcementMaterialInfo(EquipableItem equipableItem)
    {
        // 강화 레벨 정보
        string materialInfo = $"강화 레벨: +{equipableItem.currentReinforcementLevel}/{equipableItem.maxReinforcementLevel}\n\n";
        
        int cost = equipableItem.GetReinforcementCost();
        int playerGold = BattleFlowController.Instance?.playerData?.gold ?? 0;
        
        // 강화 비용 정보
        materialInfo += $"강화 비용: {cost} 골드 ";
        if (playerGold >= cost)
        {
            materialInfo += "<color=green>(보유)</color>";
        }
        else
        {
            materialInfo += $"<color=red>(부족: {cost - playerGold}개)</color>";
        }
        
        // 성공 확률 정보
        float successRate = equipableItem.GetReinforcementSuccessRate();
        materialInfo += $"\n성공 확률: {successRate:F1}%\n\n";

        // 강화 시 얻는 효과 미리보기
        if (equipableItem.currentReinforcementLevel < equipableItem.maxReinforcementLevel)
        {
            materialInfo += "강화 시 효과:\n";
            
            var currentEffects = equipableItem.GetCurrentEffects();
            var nextLevelEffects = equipableItem.GetTotalAttackBonusAtLevel(equipableItem.currentReinforcementLevel + 1);
            
            if (equipableItem.attackBonus > 0)
            {
                float increase = nextLevelEffects - currentEffects.attackBonus;
                materialInfo += $"• 공격력: +{increase:F1}\n";
            }
            
            if (equipableItem.defenseBonus > 0)
            {
                float nextDefense = equipableItem.GetTotalDefenseBonusAtLevel(equipableItem.currentReinforcementLevel + 1);
                float increase = nextDefense - currentEffects.defenseBonus;
                materialInfo += $"• 방어력: +{increase:F1}\n";
            }
            
            if (equipableItem.healthBonus > 0)
            {
                float nextHealth = equipableItem.GetTotalHealthBonusAtLevel(equipableItem.currentReinforcementLevel + 1);
                float increase = nextHealth - currentEffects.healthBonus;
                materialInfo += $"• 체력: +{increase:F1}\n";
            }
        }

        return materialInfo;
    }

    private void UseItem()
    {
        if (currentItem == null) return;

        // 사용 콜백 호출
        onUseItem?.Invoke(currentItem);

        // UI 업데이트 (수량 변경 등이 있을 수 있음)
        UpdateUI();

        // 소모품인 경우 수량 체크
        if (currentItem is ConsumableItem consumable && consumable.cunamount <= 0)
        {
            Hide(); // 수량이 0이 되면 정보창 닫기
        }
    }

    private void ReinforceItem()
    {
        if (currentItem == null || !(currentItem is EquipableItem equipableItem))
        {
            Debug.LogWarning("강화할 수 없는 아이템입니다.");
            return;
        }

        if (!equipableItem.CanReinforce())
        {
            Debug.LogWarning("더 이상 강화할 수 없습니다.");
            return;
        }

        // 골드 확인
        int cost = equipableItem.GetReinforcementCost();
        if (!BattleFlowController.Instance.TrySpendGold(cost))
        {
            Debug.LogWarning("골드가 부족합니다!");
            return;
        }

        // 강화 시도
        float successRate = equipableItem.GetReinforcementSuccessRate();
        bool success = Random.Range(0f, 100f) <= successRate;

        if (success)
        {
            equipableItem.Reinforce();
            Debug.Log($"{equipableItem.GetDisplayName()} 강화 성공! 현재 레벨: +{equipableItem.currentReinforcementLevel}");
            
            // 플레이어가 장착하고 있는 아이템이면 효과 재적용
            var player = Utils.GetPlayer();
            if (player != null)
            {
                var equipmentComponent = player.GetEntityComponent<EquipmentComponent>();
                if (equipmentComponent != null && 
                    equipmentComponent.GetEquippedItem(equipableItem.equipmentType) == equipableItem)
                {
                    equipmentComponent.RefreshAllEquipmentEffects();
                    player.NotifyObservers();
                }
            }
        }
        else
        {
            Debug.Log($"{equipableItem.GetDisplayName()} 강화 실패! 골드만 소모되었습니다.");
        }

        // UI 업데이트
        UpdateUI();

        // 게임 상태 알림
        BattleFlowController.Instance?.NotifyObservers();
    }
}