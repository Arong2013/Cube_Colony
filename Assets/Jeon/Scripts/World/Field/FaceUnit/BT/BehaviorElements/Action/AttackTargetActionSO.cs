using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackTargetActionSO", menuName = "Behavior/Actions/AttackTarget")]
public class AttackTargetActionSO : BehaviorActionSO
{
    public override BehaviorAction CreateAction()
    {
        return new AttackTargetAction();
    }
}
public class AttackTargetAction : BehaviorAction, IBehaviorDatable
{
    private FaceUnit target;
    bool HasValidTarget => target != null;
    public AttackTargetAction() { }
    public override BehaviorState Execute()
    {

        return PerformAttack();
    }

    private BehaviorState PerformAttack()
    {
        //// 공격 로직 수행 (예: 대상 체력 감소, 애니메이션 실행 등)
        //if (target.IsDefeated())
        //{
        //    FaceUnit.SetData<FaceUnit, AttackTargetAction>(null);
        //    return BehaviorState.SUCCESS;
        //}
        return BehaviorState.RUNNING;
    }
}
