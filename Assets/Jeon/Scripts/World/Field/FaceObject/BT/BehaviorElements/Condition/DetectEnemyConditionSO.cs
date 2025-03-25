using System.Collections.Generic;
using UnityEngine;

public class DetectEnemyConditionSO: BehaviorConditionSO
{
    public override BehaviorCondition CreateCondition()
    {
        return new DetectEnemyCondition();
    }
}

public class DetectEnemyCondition : BehaviorCondition
{
    public DetectEnemyCondition()
    {

    }
    public override BehaviorState Execute()
    {

        List<FaceUnit> detectedEnemies = UnitConditionHelper.GetEnemiesInRange(FaceUnit);
        if (detectedEnemies.Count > 0)
        {
            // 감지된 적 중 첫 번째를 우선 타겟으로 설정
          //  faceUnit.TargetEnemy = detectedEnemies[0];
            return BehaviorState.SUCCESS; // 행동 성공
        }
        else
        {
            return BehaviorState.FAILURE; // 적이 없으면 실패
        }

    }
}