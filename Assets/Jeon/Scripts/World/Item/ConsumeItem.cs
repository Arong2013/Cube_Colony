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
      .Select(id => ItemDataCenter.Get<itemAction>(id))
      .Where(action => action != null)
      .ToList();
    public override void Use(PlayerEntity player)
    {
        foreach (var action in actions)
        {
            action.Execute(player);
        }
    }
}
