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
            // ������ �� �� ù ��°�� �켱 Ÿ������ ����
          //  faceUnit.TargetEnemy = detectedEnemies[0];
            return BehaviorState.SUCCESS; // �ൿ ����
        }
        else
        {
            return BehaviorState.FAILURE; // ���� ������ ����
        }

    }
}