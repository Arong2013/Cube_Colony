[System.Serializable]
public abstract class itemAction
{
    public int ID;
    public abstract itemAction CreatAction(params object[] objects);
    public abstract void Execute(PlayerEntity player);
}
[System.Serializable]
public class HealAction : itemAction
{
    public int healAmount;
    public override itemAction CreatAction(params object[] objects)
    {
        var action = new HealAction();  
        action.healAmount = (int)objects[0];
        return action;
    }
    public override void Execute(PlayerEntity player)
    {
       // player.Heal(healAmount);   
    }
}
