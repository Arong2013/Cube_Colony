public class HealAction : IitemAction
{
    public int healAmount;
    public bool IsInstant => true;
    public HealAction(int amount)
    {
        healAmount = amount;
    }
    public void Execute(PlayerEntity player, Item item)
    {
       // player.Heal(healAmount);   
    }
}
