using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : IEntityComponent
{
    private int _maxSlot;
    private List<Item> _Items;

   private Action<InventoryComponent> _OnItemAdded;
   private Action<InventoryComponent> _OnItemUsed;
   private Action<InventoryComponent> _OnItemRemoved;
    public InventoryComponent(int maxSlots = 20)
    {
        _maxSlot = maxSlots;
        _Items = new List<Item>();
    }
    public void Start(Entity entity) { }
    public void Update(Entity entity) { }
    public void Exit(Entity entity) { }
    public Item GetItem(int index)
    {
        if (index >= 0 && index < _Items.Count) // 또는 items.Length, 타입에 따라
        {
            return _Items[index];
        }
        return null;
    }
    public bool AddItem(Item item)
    {
        bool isAdd = true;
        if (item is ConsumableItem consumable)
        {
            isAdd =  AddConsumableItem(consumable);
        }
        if (_Items.Count >= _maxSlot)
        {
            return false;
        }
        if(isAdd)
        _Items.Add(item);
        return true;
    }
    private bool AddConsumableItem(ConsumableItem consumable)
    {
        var matchingItems = _Items.FindAll(x => x.ID == consumable.ID);

        foreach (var matchingItem in matchingItems)
        {
            int excess = AddAmountAndGetExcess(matchingItem as ConsumableItem, consumable.cunamount);
            SetAmount(consumable, -excess);
            if (consumable.cunamount <= 0)
            {
                return true;
            }
        }
        return consumable.cunamount > 0;
    }
    public int AddAmountAndGetExcess(ConsumableItem consumable, int amount)
    {
        int total = consumable.cunamount + amount;
        int excess = Mathf.Max(0, total - consumable.maxamount);
        SetAmount(consumable, total - excess);
        return excess;
    }
    public void SetAmount(ConsumableItem consumable, int amount)
    {
        consumable.cunamount = Mathf.Clamp(amount, 0, consumable.maxamount);
    }
    public int SeparateItem(ConsumableItem consumable, int amount)
    {
        if (consumable.cunamount <= 1) return 0;

        int splitAmount = Mathf.Min(amount, consumable.cunamount - 1);
        consumable.cunamount -= splitAmount;

        return splitAmount;
    }
}