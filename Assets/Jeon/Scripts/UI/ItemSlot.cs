using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [TitleGroup("슬롯 UI")]
    [LabelText("아이콘 이미지"), Required]
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_item != null)
        {
            ShowItemInfo();
        }
    }

    public void ShowItemInfo()
    {
        if (_item == null) return;

        // 아이템 정보 UI 가져오기
        ItemInfoUI infoUI = Utils.GetUI<ItemInfoUI>();
        if (infoUI == null)
        {
            Debug.LogWarning("ItemInfoUI를 찾을 수 없습니다.");
            return;
        }

        // 아이템 정보 UI 표시 (사용 콜백과 버리기 콜백 모두 전달)
        infoUI.Show(_item, UseItemCallback, DiscardItemCallback);

    }

    private void UseItemCallback(Item item)
    {
        if (item == null) return;

        if (BattleFlowController.Instance == null ||
            BattleFlowController.Instance.GetPlayerEntity() == null)
        {
            Debug.LogWarning("플레이어 엔티티를 찾을 수 없습니다.");
            return;
        }

        Debug.Log($"아이템 사용: {item.ItemName}");
        item.Use(BattleFlowController.Instance.GetPlayerEntity());

        // 소모품이 모두 소진되었는지 확인
        if (item is ConsumableItem consumable && consumable.cunamount <= 0)
        {
            // 인벤토리에서 제거
            BattleFlowController.Instance.playerData.RemoveItem(item);
        }

        // UI 업데이트
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.NotifyObservers();
        }
        Utils.GetUI<InventoryUI>()?.UpdateSlots();
    }

    private void DiscardItemCallback(Item item)
    {
        if (item == null) return;

        if (BattleFlowController.Instance == null ||
            BattleFlowController.Instance.playerData == null)
        {
            Debug.LogWarning("플레이어 데이터를 찾을 수 없습니다.");
            return;
        }

        Debug.Log($"아이템 버리기: {item.ItemName}");

        // 인벤토리에서 제거
        BattleFlowController.Instance.playerData.RemoveItem(item);

        // UI 업데이트
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.NotifyObservers();
        }

        Utils.GetUI<InventoryUI>()?.UpdateSlots();
    }
}