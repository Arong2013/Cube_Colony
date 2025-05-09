using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ConsumableItem : Item
{
    [ShowInInspector] public int maxamount;
    [ShowInInspector] public List<int> ids = new List<int>();
    public int cunamount = 1;


    [ShowInInspector, ReadOnly]
    public List<itemAction> actions => ids
      .Select(id => ItemDataCenter.GetRealData<itemAction>(id))
      .Where(action => action != null)
      .ToList();

    public override void Use(PlayerEntity player)
    {
        foreach (var action in actions)
        {
            action.Execute(player);
        }
        cunamount--;
        if (cunamount <= 0)
            cunamount = 0;

            player.NotifyObservers();
    }

    public override Item Clone()
    {
        var item = new ConsumableItem();
        item.ID = this.ID;
        item.ItemName = this.ItemName;
        item.maxamount = this.maxamount;
        item.cunamount = this.cunamount;
        item. ids = new List<int>(this.ids);
        return item;
    }
}
