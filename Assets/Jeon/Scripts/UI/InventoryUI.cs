using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour, IObserver, IPlayerUesableUI
{
    [SerializeField] private Transform _slotContainer;

    private List<ItemSlot> _slots = new();
    private InventoryComponent _inventory;
    private PlayerEntity _playerEntity;

    public void Initialize(PlayerEntity playerEntity)
    {
        this._playerEntity = playerEntity;
        _inventory = playerEntity.GetEntityComponent<InventoryComponent>();
        playerEntity.RegisterObserver(this);
    }
    public void OpenInventoryUI()
    {
        gameObject.SetActive(true);
        UpDateSlots();
    }
    public void ToggleInventoryUI()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            OpenInventoryUI();
        }
    }

    public void UpDateSlots()
    {
        foreach (Transform child in _slotContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in _inventory.items)
        {
            var curSlot = Instantiate(DataCenter.Instance.GetItemSlotPrefab().gameObject, _slotContainer);
           var slot =   curSlot.GetComponent<ItemSlot>();
            slot.SetItem(item, _playerEntity);
        }
    }

    public void UpdateObserver()
    {
        UpDateSlots();
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
