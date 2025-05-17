using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class ItemSlot : MonoBehaviour
{
    [TitleGroup("슬롯 UI")]
    [LabelText("아이콘 이미지"), Required]
    [PreviewField(50)]
    [SerializeField] private Image _icon;

    [TitleGroup("슬롯 UI")]
    [LabelText("수량 텍스트"), Required]
    [SerializeField] private TextMeshProUGUI _amountText;

    [TitleGroup("아이템 정보")]
    [ReadOnly, ShowInInspector]
    private Item _item;

    [TitleGroup("아이템 정보")]
    [ReadOnly, ShowInInspector]
    private bool isConsumable => _item is ConsumableItem;

    [TitleGroup("아이템 정보")]
    [ReadOnly, ShowInInspector]
    private int amount => _item is ConsumableItem consumable ? consumable.cunamount : 0;

    public void SetItem(Item item)
    {
        _item = item;
        UpdateUI();
    }

    public void ClearSlot()
    {
        _item = null;
        _icon.sprite = null;
        _amountText.text = "";
    }

    private void UpdateUI()
    {
        if (_item != null)
        {
            _icon.sprite = _item.ItemIcon;
            if (_item is ConsumableItem consumable)
            {
                _amountText.text = consumable.cunamount.ToString();
            }
            else
            {
                _amountText.text = "";
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void ShowItemInfo()
    {
        // 아이템 정보 표시 로직 추가
    }

    public void UseItem()
    {
        if (_item == null)
        {
            Debug.LogWarning("아이템이 없습니다.");
            return;
        }

        if (BattleFlowController.Instance == null ||
            BattleFlowController.Instance.GetPlayerEntity() == null)
        {
            Debug.LogWarning("플레이어 엔티티를 찾을 수 없습니다.");
            return;
        }

        Debug.Log("아이템 사용");
        _item.Use(BattleFlowController.Instance.GetPlayerEntity());

        // 소모품이 모두 소진되었는지 확인
        if (_item is ConsumableItem consumable && consumable.cunamount <= 0)
        {
            // 인벤토리에서 제거
            BattleFlowController.Instance.playerData.RemoveItem(_item);
            BattleFlowController.Instance.NotifyObservers();
        }
        else
        {
            // 아이템 수량만 변경된 경우 UI 업데이트
            UpdateUI();
        }
    }
}