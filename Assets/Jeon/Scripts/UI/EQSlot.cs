using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Sirenix.OdinInspector;

/// <summary>
/// 장비 슬롯 UI - 특정 장비 타입을 관리하는 슬롯
/// </summary>
public class EQSlot : MonoBehaviour, IPointerClickHandler
{
    [TitleGroup("슬롯 설정")]
    [LabelText("장비 타입"), Required]
    [SerializeField] private EquipmentType slotType;

    [TitleGroup("슬롯 UI")]
    [LabelText("장비 아이콘"), Required]
    [SerializeField] private Image equipmentIcon;

    [TitleGroup("슬롯 UI")]
    [LabelText("장비 이름 텍스트")]
    [SerializeField] private TextMeshProUGUI equipmentNameText;

    [TitleGroup("슬롯 UI")]
    [LabelText("강화 레벨 표시")]
    [SerializeField] private TextMeshProUGUI reinforcementLevelText;

    [TitleGroup("슬롯 UI")]
    [LabelText("빈 슬롯 표시 이미지")]
    [SerializeField] private Image emptySlotImage;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private EquipableItem equippedItem;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private InventoryUI parentInventoryUI;

    public EquipmentType SlotType => slotType;
    public EquipableItem EquippedItem => equippedItem;

    /// <summary>
    /// 슬롯 초기화
    /// </summary>
    public void Initialize(InventoryUI inventoryUI)
    {
        parentInventoryUI = inventoryUI;
        UpdateSlotDisplay();
    }

    /// <summary>
    /// 슬롯 클릭 이벤트
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (equippedItem != null)
        {
            // 장착된 아이템이 있으면 장비 현황 UI 열기
            parentInventoryUI?.OpenEquipmentStatus(slotType);
        }
        else
        {
            // 빈 슬롯이면 인벤토리에서 해당 타입 아이템 보여주기
            parentInventoryUI?.ShowEquipableItemsForSlot(slotType);
        }
    }

    /// <summary>
    /// 아이템 장착
    /// </summary>
    public void EquipItem(EquipableItem item)
    {
        if (item.equipmentType != slotType)
        {
            Debug.LogWarning($"타입이 맞지 않습니다. 슬롯: {slotType}, 아이템: {item.equipmentType}");
            return;
        }

        equippedItem = item;
        UpdateSlotDisplay();

        Debug.Log($"{slotType} 슬롯에 {item.ItemName} 장착됨");
    }

    /// <summary>
    /// 아이템 해제
    /// </summary>
    public void UnequipItem()
    {
        if (equippedItem != null)
        {
            Debug.Log($"{slotType} 슬롯에서 {equippedItem.ItemName} 해제됨");
            equippedItem = null;
            UpdateSlotDisplay();
        }
    }

    /// <summary>
    /// 슬롯 표시 업데이트
    /// </summary>
    public void UpdateSlotDisplay()
    {
        if (equippedItem != null)
        {
            // 장착된 아이템 표시
            if (equipmentIcon != null)
            {
                equipmentIcon.sprite = equippedItem.ItemIcon;
                equipmentIcon.gameObject.SetActive(true);
            }

            if (equipmentNameText != null)
            {
                equipmentNameText.text = equippedItem.ItemName;
                equipmentNameText.gameObject.SetActive(true);
            }

            if (reinforcementLevelText != null)
            {
                if (equippedItem.currentReinforcementLevel > 0)
                {
                    reinforcementLevelText.text = $"+{equippedItem.currentReinforcementLevel}";
                    reinforcementLevelText.gameObject.SetActive(true);
                }
                else
                {
                    reinforcementLevelText.gameObject.SetActive(false);
                }
            }

            if (emptySlotImage != null)
            {
                emptySlotImage.gameObject.SetActive(false);
            }
        }
        else
        {
            // 빈 슬롯 표시
            if (equipmentIcon != null)
            {
                equipmentIcon.gameObject.SetActive(false);
            }

            if (equipmentNameText != null)
            {
                equipmentNameText.text = GetSlotTypeName();
                equipmentNameText.gameObject.SetActive(true);
            }

            if (reinforcementLevelText != null)
            {
                reinforcementLevelText.gameObject.SetActive(false);
            }

            if (emptySlotImage != null)
            {
                emptySlotImage.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 슬롯 타입에 따른 이름 반환
    /// </summary>
    public string GetSlotTypeName()
    {
        return slotType switch
        {
            EquipmentType.Sword => "검",
            EquipmentType.Gun => "총",
            EquipmentType.OxygenTank => "산소통",
            EquipmentType.Battery => "배터리",
            EquipmentType.Backpack => "가방",
            EquipmentType.Helmet => "헬멧",
            _ => "장비 슬롯"
        };
    }
}