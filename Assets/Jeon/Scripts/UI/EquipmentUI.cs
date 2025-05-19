using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

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
    [SerializeField] private TextMeshProUGUI totalEffectsText;
    [SerializeField] private Button closeButton;

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

        gameObject.SetActive(false);
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
            { EquipmentType.Weapon, weaponSlot },
            { EquipmentType.OxygenTank, oxygenTankSlot },
            { EquipmentType.Battery, batterySlot },
            { EquipmentType.Backpack, backpackSlot },
            { EquipmentType.Helmet, helmetSlot }
        };

        // 각 슬롯에 이벤트 연결
        foreach (var kvp in slotUIMap)
        {
            var slot = kvp.Key;
            var slotUI = kvp.Value;

            slotUI.Initialize(slot);
            slotUI.OnItemClicked += OnEquipmentSlotClicked;
            slotUI.OnItemRightClicked += OnEquipmentSlotRightClicked;
        }
    }

    private void SetupButtons()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        if (reinforcementUI != null)
        {
            reinforcementUI.OnReinforceClicked += OnReinforceClicked;
        }
    }

    public void Show()
    {
        // 플레이어의 장비 핸들러 가져오기
        var player = Utils.GetPlayer();
        if (player != null)
        {
            equipmentHandler = player.GetEntityComponent<PlayerEquipmentHandler>();
        }

        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        selectedItem = null;

        if (reinforcementUI != null)
        {
            reinforcementUI.Hide();
        }
    }

    public void UpdateUI()
    {
        if (equipmentHandler == null) return;

        var equippedItems = equipmentHandler.GetAllEquippedItems();

        // 각 슬롯 업데이트
        foreach (var kvp in slotUIMap)
        {
            var slot = kvp.Key;
            var slotUI = kvp.Value;

            if (equippedItems.ContainsKey(slot))
            {
                slotUI.SetEquippedItem(equippedItems[slot]);
            }
            else
            {
                slotUI.ClearSlot();
            }
        }

        // 총 효과 표시
        UpdateTotalEffectsDisplay();
    }

    private void UpdateTotalEffectsDisplay()
    {
        if (totalEffectsText == null || equipmentHandler == null) return;

        var totalEffects = equipmentHandler.GetTotalEffects();
        var text = "";

        if (totalEffects.attackBonus > 0)
            text += $"공격력 +{totalEffects.attackBonus}\n";
        if (totalEffects.defenseBonus > 0)
            text += $"방어력 +{totalEffects.defenseBonus}\n";
        if (totalEffects.healthBonus > 0)
            text += $"체력 +{totalEffects.healthBonus}\n";
        if (totalEffects.maxOxygenBonus > 0)
            text += $"산소 +{totalEffects.maxOxygenBonus}\n";
        if (totalEffects.maxEnergyBonus > 0)
            text += $"에너지 +{totalEffects.maxEnergyBonus}\n";
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

        if (string.IsNullOrEmpty(text))
        {
            text = "장착된 장비 없음";
        }

        totalEffectsText.text = text.TrimEnd('\n');
    }

    private void OnEquipmentSlotClicked(EquipmentType slot, EquipableItem item)
    {
        selectedItem = item;

        if (reinforcementUI != null)
        {
            if (item != null)
            {
                reinforcementUI.Show(item);
            }
            else
            {
                reinforcementUI.Hide();
            }
        }
    }

    private void OnEquipmentSlotRightClicked(EquipmentType slot, EquipableItem item)
    {
        if (item != null && equipmentHandler != null)
        {
            equipmentHandler.UnequipItem(slot);
            UpdateUI();
        }
    }

    private void OnReinforceClicked(EquipableItem item)
    {
        var player = Utils.GetPlayer();
        if (player != null && item != null)
        {
            if (player.ReinforceEquipment(item))
            {
                UpdateUI();
            }
        }
    }

    public void UpdateObserver()
    {
        UpdateUI();
    }
}
