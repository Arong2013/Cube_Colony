using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReinforcementUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI currentEffectsText;
    [SerializeField] private TextMeshProUGUI nextEffectsText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button reinforceButton;
    [SerializeField] private Button closeButton;

    public event Action<EquipableItem> OnReinforceClicked;

    private EquipableItem currentItem;

    private void Start()
    {
        if (reinforceButton != null)
        {
            reinforceButton.onClick.AddListener(() => OnReinforceClicked?.Invoke(currentItem));
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        gameObject.SetActive(false);
    }

    public void Show(EquipableItem item)
    {
        currentItem = item;
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentItem = null;
    }

    private void UpdateUI()
    {
        if (currentItem == null) return;

        // 아이템 이름
        if (itemNameText != null)
        {
            itemNameText.text = currentItem.GetDisplayName();
        }

        // 현재 레벨
        if (currentLevelText != null)
        {
            currentLevelText.text = $"강화 레벨: {currentItem.reinforcementLevel}/{currentItem.maxReinforcementLevel}";
        }

        // 현재 효과
        if (currentEffectsText != null)
        {
            currentEffectsText.text = GetEffectsText(currentItem.GetCurrentEffects());
        }

        // 다음 레벨 효과
        if (nextEffectsText != null)
        {
            if (currentItem.reinforcementLevel < currentItem.maxReinforcementLevel)
            {
                // 임시로 강화 레벨을 올려서 다음 효과 계산
                var tempItem = currentItem.Clone() as EquipableItem;
                tempItem.reinforcementLevel = currentItem.reinforcementLevel + 1;
                nextEffectsText.text = $"다음 레벨:\n{GetEffectsText(tempItem.GetCurrentEffects())}";
            }
            else
            {
                nextEffectsText.text = "최대 강화 달성";
            }
        }

        // 강화 비용
        if (costText != null)
        {
            if (currentItem.reinforcementLevel < currentItem.maxReinforcementLevel)
            {
                int cost = currentItem.reinforcementCosts[currentItem.reinforcementLevel];
                costText.text = $"비용: {cost} 골드";
            }
            else
            {
                costText.text = "강화 불가";
            }
        }

        // 강화 버튼
        if (reinforceButton != null)
        {
            reinforceButton.interactable = currentItem.reinforcementLevel < currentItem.maxReinforcementLevel;
        }
    }

    private string GetEffectsText(EquipmentEffects effects)
    {
        var text = "";

        if (effects.attackBonus > 0)
            text += $"공격력 +{effects.attackBonus}\n";
        if (effects.defenseBonus > 0)
            text += $"방어력 +{effects.defenseBonus}\n";
        if (effects.healthBonus > 0)
            text += $"체력 +{effects.healthBonus}\n";
        if (effects.maxOxygenBonus > 0)
            text += $"산소 +{effects.maxOxygenBonus}\n";
        if (effects.maxEnergyBonus > 0)
            text += $"에너지 +{effects.maxEnergyBonus}\n";
        if (effects.extraHitCount > 0)
            text += $"추가 타격 +{effects.extraHitCount}\n";
        if (effects.fireRateBonus > 0)
            text += $"연사속도 +{effects.fireRateBonus}\n";
        if (effects.oxygenConsumptionReduction > 0)
            text += $"산소 소모 -{effects.oxygenConsumptionReduction * 100}%\n";
        if (effects.energyConsumptionReduction > 0)
            text += $"에너지 소모 -{effects.energyConsumptionReduction * 100}%\n";
        if (effects.inventorySlotBonus > 0)
            text += $"인벤토리 +{effects.inventorySlotBonus}칸\n";
        if (effects.damageReduction > 0)
            text += $"피해 감소 -{effects.damageReduction * 100}%\n";

        return string.IsNullOrEmpty(text) ? "효과 없음" : text.TrimEnd('\n');
    }
}