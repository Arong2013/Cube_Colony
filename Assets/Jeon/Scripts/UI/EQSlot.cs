using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private EquipableItem equippedItem;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private InventoryUI parentInventoryUI;
   [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    public EquipmentType SlotType => slotType;
     [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
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
            // 장착된 아이템이 있으면 아이템 정보 UI 표시
            if (parentInventoryUI != null)
            {
                parentInventoryUI.ShowItemInfo(equippedItem);
            }
        }
        else
        {
            Debug.Log($"{slotType} 슬롯이 비어있습니다. 인벤토리에서 해당 타입의 장비를 장착하세요.");
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
        }
        else
        {
            // 빈 슬롯 표시
            if (equipmentIcon != null)
            {
                equipmentIcon.gameObject.SetActive(false);
            }
        }
    }
}