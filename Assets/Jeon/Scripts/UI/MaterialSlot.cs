using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MaterialSlot : UltimateClean.Tooltip
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TooltipUI tooltipUI;

    private ConsumableItemSO itemSO;

    public void Initialize(int itemId, int requiredCount, int currentCount)
    {
        // 아이템 SO 가져오기
        itemSO = DataCenter.Instance.GetConsumableItemSO(itemId);
        
        if (itemSO != null)
        {
            // 아이콘 설정
            itemIcon.sprite = itemSO.itemIcon;
        }

        // 텍스트 설정
        countText.text = $"{currentCount}/{requiredCount}";
        countText.color = currentCount >= requiredCount ? Color.white : Color.red;
        tooltip = tooltipUI.gameObject;
    }

    // 부모 클래스의 OnPointerEnter와 OnPointerExit 메서드 사용
    public override void OnPointerEnter(PointerEventData eventData)
    {
        // 아이템 SO가 있을 때만 툴팁 표시
        if (itemSO != null)
        {
            tooltipUI.ShowTooltip(itemSO);
            base.OnPointerEnter(eventData);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        tooltipUI.HideTooltip();
        base.OnPointerExit(eventData);
    }
}