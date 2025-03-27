using System.Collections.Generic;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "DetectEnemyConditionSO", menuName = "Behavior/Conditions/DetectEnemyCondition")]
public class DetectEnemyConditionSO : BehaviorConditionSO
{
    [SerializeField] bool isAttackDetect;
    public override BehaviorCondition CreateCondition()
    {
        return new DetectEnemyCondition(isAttackDetect);
    }
}
public class DetectEnemyCondition : BehaviorCondition, IBehaviorDatable
{
    bool isAttackDetect;
    public DetectEnemyCondition(bool isAttackDetect) { this.isAttackDetect = isAttackDetect;}
    public override BehaviorState Execute()
    {
        var detectedEnemies = GetDetectedEnemies();

        if (IsEnemyDetected(detectedEnemies))
        {
            return BehaviorState.SUCCESS;
        }
        detectedEnemies = UnitConditionHelper.GetEnemiesInRange(FaceUnit,isAttackDetect);
        return IsEnemyDetected(detectedEnemies) ? BehaviorState.SUCCESS : BehaviorState.FAILURE;
    }
    private bool IsEnemyDetected(List<FaceUnit> enemies)
    {
        return enemies != null && enemies.Count > 0;
    }
    private List<FaceUnit> GetDetectedEnemies()
    {
        return FaceUnit.GetUnitData<List<FaceUnit>>(BehaviorDataType.TargetList);
    }
}
