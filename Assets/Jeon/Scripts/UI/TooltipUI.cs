using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private Transform fieldTileContainer;
    [SerializeField] private GameObject fieldTileSlotPrefab;

    public void ShowTooltip(ConsumableItemSO itemSO)
    {
        // 필드 타일 슬롯 초기화
        InitializeFieldTileSlots(itemSO);
    }

    private void InitializeFieldTileSlots(ConsumableItemSO itemSO)
    {
        foreach (Transform child in fieldTileContainer)
        {
            Destroy(child.gameObject);
        }

        // 필드 타일 슬롯 생성
        foreach (int fieldId in itemSO.acquirableFieldIds)
        {
            var fieldTileSO = DataCenter.Instance.GetFieldTileDataSO(fieldId);
            if (fieldTileSO != null)
            {
                GameObject slotObj = Instantiate(fieldTileSlotPrefab, fieldTileContainer);
                
                // 필드 타일 슬롯 컴포넌트 가져오기
                var fieldTileSlot = slotObj.GetComponent<FieldTileSlot>();
                if (fieldTileSlot != null)
                {
                    fieldTileSlot.Initialize(fieldTileSO);
                }
            }
        }
    }

    public void HideTooltip()
    {
        
    }
}