public class ThrowAction : IitemAction
{
    public float damage;
    public float range;

    public bool IsInstant => true;
    public ThrowAction(float damage, float range)
    {
        this.damage = damage;
        this.range = range;
    }
    public void Execute(PlayerEntity player, Item item)
    {
        //player.Throw(item, damage, range);
    }
}
