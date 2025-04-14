using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/Action/Attack")]
public class AttackActionSO : BehaviorActionSO
{
    public override BehaviorAction CreateAction()
    {
        return new AttackAction();
    }
}
public class AttackAction : BehaviorAction
{


    public override BehaviorState Execute()
    {
        if (entity.GetState().GetType() == typeof(AttackState))
        {
            return BehaviorState.RUNNING;
        }
        if (!entity.TryGetData<PlayerEntity>("target", out var target) && !entity.CanAttack)
        {
            return BehaviorState.FAILURE;
        }
   
        entity.SetTarget(target);   
        entity.SetAnimatorValue(EntityAnimTrigger.AttackTrigger,null); 
        entity.RemoveData("target");    
        return BehaviorState.SUCCESS;
    }
}   