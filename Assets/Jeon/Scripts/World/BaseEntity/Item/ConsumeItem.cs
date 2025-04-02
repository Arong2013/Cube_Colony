public class ConsumableItem : Item
{
    public int consumePerUse = 1;
    public int cunamount;
    public int maxamount;
    public override void Use(PlayerEntity player)
    {
        base.Use(player);   

    }
}
