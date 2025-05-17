using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InventoryUI : MonoBehaviour, IObserver
{
    [TitleGroup("인벤토리 UI")]
    [LabelText("슬롯 컨테이너"), Required]
    [SerializeField] private Transform _slotContainer;

    [TitleGroup("인벤토리 UI")]
    [LabelText("아이템 정보 UI"), Required]
    [SerializeField] private ItemInfoUI _itemInfoUI;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private List<ItemSlot> _slots = new();

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private int itemCount => BattleFlowController.Instance?.playerData?.inventory?.Count ?? 0;

    private void Start()
    {
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.RegisterObserver(this);
            Initialize();
        }

        // 아이템 정보 UI 찾기
        if (_itemInfoUI == null)
        {
            _itemInfoUI = GetComponentInChildren<ItemInfoUI>(true);
            if (_itemInfoUI == null)
            {
                _itemInfoUI = Utils.GetUI<ItemInfoUI>();
            }
        }

        // 아이템 정보 UI 초기화
        if (_itemInfoUI != null)
        {
            _itemInfoUI.Initialize();
        }
    }

    private void OnDestroy()
    {
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.UnregisterObserver(this);
        }
    }

    public void Initialize()
    {
        UpdateSlots();
    }

    public void OpenInventoryUI()
    {
        gameObject.SetActive(true);
        UpdateSlots();

        // 열 때 아이템 정보 UI 닫기
        if (_itemInfoUI != null)
        {
            _itemInfoUI.Hide();
        }
    }

    public void ToggleInventoryUI()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);

            // 닫을 때 아이템 정보 UI도 닫기
            if (_itemInfoUI != null)
            {
                _itemInfoUI.Hide();
            }
        }
        else
        {
            OpenInventoryUI();
        }
    }

    public void UpdateSlots()
    {
        if (BattleFlowController.Instance == null ||
            BattleFlowController.Instance.playerData == null ||
            BattleFlowController.Instance.playerData.inventory == null)
            return;

        // 기존 슬롯 제거
        foreach (Transform child in _slotContainer)
        {
            Destroy(child.gameObject);
        }

        _slots.Clear();

        // 새 슬롯 생성
        foreach (var item in BattleFlowController.Instance.playerData.inventory)
        {
            var curSlot = Instantiate(DataCenter.Instance.GetItemSlotPrefab().gameObject, _slotContainer);
            var slot = curSlot.GetComponent<ItemSlot>();
            slot.SetItem(item);
            _slots.Add(slot);
        }
    }

    public void UpdateObserver()
    {
        UpdateSlots();
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);

        // 인벤토리 UI를 닫을 때 아이템 정보 UI도 닫기
        if (_itemInfoUI != null)
        {
            _itemInfoUI.Hide();
        }
    }
}