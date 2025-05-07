using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountText;

    private Item _item;
    private PlayerEntity playerEntity;  
    public void SetItem(Item item,PlayerEntity playerEntity)
    {
        _item = item;
        this.playerEntity = playerEntity;   
        UpdateUI();
    }
    public void ClearSlot()
    {
        _item = null;
        _icon.sprite = null;
        _icon.enabled = false;
        _amountText.text = "";
    }
    private void UpdateUI()
    {
        if (_item != null)
        {
            _icon.enabled = true;
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

    }
    public void UseItem()
    {
        _item?.Use(playerEntity);      
    }
}
