using System.Collections.Generic;

public class ConsumableItem : Item
{
    public int consumePerUse = 1;
    public int cunamount;
    public int maxamount;
    public List<IitemAction> actions = new List<IitemAction>();
    public override void Use(PlayerEntity player)
    {
        foreach (var action in actions)
        {
            action.Execute(player, this);
        }
    }
}
