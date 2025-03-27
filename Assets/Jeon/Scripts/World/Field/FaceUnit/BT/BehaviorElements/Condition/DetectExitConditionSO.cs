using System.Collections.Generic;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "DetectExitConditionSO", menuName = "Behavior/Conditions/DetectExitCondition")]
public class DetectExitConditionSO : BehaviorConditionSO
{
    public override BehaviorCondition CreateCondition()
    {
        return new DetectExitCondition();
    }
}
public class DetectExitCondition : BehaviorCondition, IBehaviorDatable
{
    public DetectExitCondition() { }
    public override BehaviorState Execute()
    {
        var detectedEnemies = GetDetectedEnemies();

        if (IsEnemyDetected(detectedEnemies))
        {
            return BehaviorState.SUCCESS;
        }
        detectedEnemies = UnitConditionHelper.GetExitGateObjects(FaceUnit);
        
        if(IsEnemyDetected(detectedEnemies))
        {
            FaceUnit.SetData<List<ExitGateObject>>(BehaviorDataType.TargetList, detectedEnemies);
            return BehaviorState.SUCCESS;
        }
        else
        {
            return BehaviorState.FAILURE;
        }
    }
    private bool IsEnemyDetected(List<ExitGateObject> enemies)
    {
        return enemies != null && enemies.Count > 0;
    }
    private List<ExitGateObject> GetDetectedEnemies()
    {
        return FaceUnit.GetUnitData<List<ExitGateObject>>(BehaviorDataType.TargetList);
    }
}
