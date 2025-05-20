using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 장비 관리 UI
/// </summary>
public class EquipmentUI : MonoBehaviour, IObserver
{
    [TitleGroup("장비 슬롯")]
    [SerializeField] private EQSlot weaponSlot;
    [SerializeField] private EQSlot oxygenTankSlot;
    [SerializeField] private EQSlot batterySlot;
    [SerializeField] private EQSlot backpackSlot;
    [SerializeField] private EQSlot helmetSlot;

    [TitleGroup("정보 UI")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemStatsText;
    [SerializeField] private TextMeshProUGUI totalEffectsText;

    [TitleGroup("강화 UI")]
    [SerializeField] private GameObject reinforcementPanel;
    [SerializeField] private TextMeshProUGUI reinforcementCostText;
    [SerializeField] private TextMeshProUGUI reinforcementLevelText;
    [SerializeField] private TextMeshProUGUI successRateText;
    [SerializeField] private Button reinforceButton;
    [SerializeField] private Button closeButton;

    // 이벤트
    public event Action<EquipableItem> OnItemReinforced;

    private Dictionary<EquipmentType, EQSlot> slotUIMap;
    private PlayerEquipmentHandler equipmentHandler;
    private EquipableItem selectedItem;

    private void Start()
    {
        InitializeSlotMap();
        SetupButtons();

        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.RegisterObserver(this);
        }

        // 초기에는 숨겨둠
        gameObject.SetActive(false);
        if (reinforcementPanel != null)
        {
            reinforcementPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.UnregisterObserver(this);
        }
    }

    private void InitializeSlotMap()
    {
        slotUIMap = new Dictionary<EquipmentType, EQSlot>
        {
            { EquipmentType.Sword, weaponSlot },
            { EquipmentType.OxygenTank, oxygenTankSlot },
            { EquipmentType.Battery, batterySlot },
            { EquipmentType.Backpack, backpackSlot },
            { EquipmentType.Helmet, helmetSlot }
        };

        // 각 슬롯 초기화 및 클릭 리스너 추가
        foreach (var kvp in slotUIMap)
        {
            if (kvp.Value != null)
            {
                // EQSlot은 null 또는 적절한 InventoryUI 참조를 전달
                kvp.Value.Initialize(null); 
                
                // 슬롯 클릭 감지를 위한 이벤트 트리거 추가
                EventTrigger trigger = kvp.Value.gameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = kvp.Value.gameObject.AddComponent<EventTrigger>();
                }
                
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((data) => 
                {
                    OnSlotClicked(kvp.Key, kvp.Value.EquippedItem);
                });
                trigger.triggers.Add(entry);
            }
        }
    }

    private void OnSlotClicked(EquipmentType type, EquipableItem item)
    {
        selectedItem = item;
        UpdateItemDetails();
    }

    private void SetupButtons()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        if (reinforceButton != null)
        {
            reinforceButton.onClick.AddListener(() => ReinforceSelectedItem());
        }
    }

    public void ShowEquipmentStatus(EquipableItem item)
    {
        // 플레이어의 장비 핸들러 가져오기
        var player = Utils.GetPlayer();
        if (player != null)
        {
            equipmentHandler = player.GetEntityComponent<PlayerEquipmentHandler>();
        }

        selectedItem = item;
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        selectedItem = null;
        
        if (reinforcementPanel != null)
        {
            reinforcementPanel.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        if (equipmentHandler == null) return;

        // 슬롯 UI 업데이트
        foreach (var kvp in slotUIMap)
        {
            if (kvp.Value != null)
            {
                var equippedItem = equipmentHandler.GetEquippedItem(kvp.Key);
                if (equippedItem != null)
                {
                    kvp.Value.EquipItem(equippedItem);
                }
                else
                {
                    kvp.Value.UnequipItem();
                }
            }
        }

        UpdateItemDetails();
        UpdateTotalEffectsDisplay();
    }

    private void UpdateItemDetails()
    {
        if (selectedItem == null)
        {
            if (itemNameText != null) itemNameText.text = "장비를 선택하세요";
            if (itemDescriptionText != null) itemDescriptionText.text = "";
            if (itemStatsText != null) itemStatsText.text = "";
            if (reinforcementPanel != null) reinforcementPanel.SetActive(false);
            return;
        }

        // 기본 정보 표시
        if (itemNameText != null)
        {
            itemNameText.text = selectedItem.GetDisplayName();
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = selectedItem.description;
        }

        if (itemStatsText != null)
        {
            string stats = "";
            var effects = selectedItem.GetCurrentEffects();
            
            if (effects.attackBonus > 0) stats += $"공격력: +{effects.attackBonus}\n";
            if (effects.defenseBonus > 0) stats += $"방어력: +{effects.defenseBonus}\n";
            if (effects.healthBonus > 0) stats += $"체력: +{effects.healthBonus}\n";
            if (effects.maxOxygenBonus > 0) stats += $"최대 산소: +{effects.maxOxygenBonus}\n";
            if (effects.maxEnergyBonus > 0) stats += $"최대 에너지: +{effects.maxEnergyBonus}\n";
            if (effects.extraHitCount > 0) stats += $"추가 타격: +{effects.extraHitCount}\n";
            if (effects.fireRateBonus > 0) stats += $"연사 속도: +{effects.fireRateBonus}\n";
            if (effects.oxygenConsumptionReduction > 0) stats += $"산소 소모 감소: {effects.oxygenConsumptionReduction * 100}%\n";
            if (effects.energyConsumptionReduction > 0) stats += $"에너지 소모 감소: {effects.energyConsumptionReduction * 100}%\n";
            if (effects.inventorySlotBonus > 0) stats += $"인벤토리 슬롯: +{effects.inventorySlotBonus}\n";
            if (effects.damageReduction > 0) stats += $"피해 감소: {effects.damageReduction * 100}%\n";
            
            itemStatsText.text = stats;
        }

        // 강화 패널 업데이트
        UpdateReinforcementPanel();
    }

    private void UpdateReinforcementPanel()
    {
        // 기존 코드 유지
        if (reinforcementPanel == null || selectedItem == null) return;

        bool canReinforce = selectedItem.CanReinforce();
        reinforcementPanel.SetActive(true);

        if (reinforcementLevelText != null)
        {
            reinforcementLevelText.text = $"강화 레벨: +{selectedItem.currentReinforcementLevel}/{selectedItem.maxReinforcementLevel}";
        }

        if (reinforcementCostText != null)
        {
            if (canReinforce)
            {
                reinforcementCostText.text = $"비용: {selectedItem.GetReinforcementCost()} 골드";
            }
            else
            {
                reinforcementCostText.text = "최대 레벨 도달";
            }
        }

        if (successRateText != null)
        {
            if (canReinforce)
            {
                successRateText.text = $"성공 확률: {selectedItem.GetReinforcementSuccessRate():F1}%";
            }
            else
            {
                successRateText.text = "";
            }
        }

        if (reinforceButton != null)
        {
            // 플레이어 골드 충분한지 확인
            int playerGold = BattleFlowController.Instance?.playerData?.gold ?? 0;
            bool hasEnoughGold = playerGold >= selectedItem.GetReinforcementCost();
            
            reinforceButton.interactable = canReinforce && hasEnoughGold;
        }
    }

    private void UpdateTotalEffectsDisplay()
    {
        // 기존 코드 유지
        if (totalEffectsText == null || equipmentHandler == null) return;

        var totalEffects = equipmentHandler.GetTotalEffects();
        var text = "=== 장비 총 효과 ===\n";

        if (totalEffects.attackBonus > 0)
            text += $"공격력 +{totalEffects.attackBonus}\n";
        if (totalEffects.defenseBonus > 0)
            text += $"방어력 +{totalEffects.defenseBonus}\n";
        if (totalEffects.healthBonus > 0)
            text += $"체력 +{totalEffects.healthBonus}\n";
        if (totalEffects.maxOxygenBonus > 0)
            text += $"최대 산소 +{totalEffects.maxOxygenBonus}\n";
        if (totalEffects.maxEnergyBonus > 0)
            text += $"최대 에너지 +{totalEffects.maxEnergyBonus}\n";
        if (totalEffects.extraHitCount > 0)
            text += $"추가 타격 +{totalEffects.extraHitCount}\n";
        if (totalEffects.fireRateBonus > 0)
            text += $"연사속도 +{totalEffects.fireRateBonus}\n";
        if (totalEffects.oxygenConsumptionReduction > 0)
            text += $"산소 소모 -{totalEffects.oxygenConsumptionReduction * 100}%\n";
        if (totalEffects.energyConsumptionReduction > 0)
            text += $"에너지 소모 -{totalEffects.energyConsumptionReduction * 100}%\n";
        if (totalEffects.inventorySlotBonus > 0)
            text += $"인벤토리 +{totalEffects.inventorySlotBonus}칸\n";
        if (totalEffects.damageReduction > 0)
            text += $"피해 감소 -{totalEffects.damageReduction * 100}%\n";

        totalEffectsText.text = text;
    }

    private void ReinforceSelectedItem()
    {
        // 기존 코드 유지
        if (selectedItem == null || !selectedItem.CanReinforce()) return;

        // 골드 확인
        int cost = selectedItem.GetReinforcementCost();
        if (BattleFlowController.Instance?.playerData?.gold < cost)
        {
            Debug.Log("골드가 부족합니다!");
            return;
        }

        // 강화 시도
        bool success = selectedItem.Reinforce();
        
        // 강화 결과 알림
        if (success)
        {
            Debug.Log($"{selectedItem.GetDisplayName()} 강화 성공! 현재 레벨: +{selectedItem.currentReinforcementLevel}");
            // 이벤트 발생
            OnItemReinforced?.Invoke(selectedItem);
        }
        else
        {
            Debug.Log($"{selectedItem.GetDisplayName()} 강화 실패! 골드만 소모되었습니다.");
        }

        // UI 갱신
        UpdateUI();
        
        // 플레이어 스탯 갱신을 위해 옵저버 알림
        BattleFlowController.Instance?.NotifyObservers();
    }

    public void UpdateObserver()
    {
        UpdateUI();
    }
}