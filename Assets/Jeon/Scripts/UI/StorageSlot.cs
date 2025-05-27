using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using Sirenix.OdinInspector;

public class StorageSlot : MonoBehaviour, IPointerClickHandler
{
    [TitleGroup("슬롯 UI")]
    [SerializeField, ShowInInspector]
    private Image icon;

    [TitleGroup("슬롯 UI")]
    [SerializeField, ShowInInspector]
    private TextMeshProUGUI amountText;

    [TitleGroup("아이템 정보")]
    [ReadOnly, ShowInInspector]
    private Item item;

    [TitleGroup("슬롯 정보")]
    [ReadOnly, ShowInInspector]
    private bool isStorageSlot; // true: 창고 슬롯, false: 인벤토리 슬롯

    private Action<Item> transferAction; // 아이템 이동 액션
    // 슬롯 초기화
    public void Initialize(Item item, bool isStorageSlot, Action<Item> transferAction)
    {
        this.item = item;
        this.isStorageSlot = isStorageSlot;
        this.transferAction = transferAction;

        UpdateUI();
    }

    // UI 업데이트
    private void UpdateUI()
    {
        if (item != null)
        {
            // 아이콘 설정
            if (icon != null)
            {
                icon.sprite = item.ItemIcon;
                icon.gameObject.SetActive(true);
            }

            // 수량 텍스트 설정 (소모품인 경우만)
            if (amountText != null)
            {
                if (item is ConsumableItem consumable)
                {
                    amountText.text = consumable.cunamount.ToString();
                    amountText.gameObject.SetActive(true);
                }
                else
                {
                    amountText.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // 아이템이 없는 경우
            if (icon != null)
            {
                icon.sprite = null;
                icon.gameObject.SetActive(false);
            }

            if (amountText != null)
            {
                amountText.gameObject.SetActive(false);
            }
        }
    }

    // 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;
        TransferItem();
    }

    // 아이템 이동
    private void TransferItem()
    {
        if (transferAction != null)
        {
            transferAction.Invoke(item);
        }
    }
}