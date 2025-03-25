using System.Collections.Generic;

public class DetectEnemyConditionSO : BehaviorConditionSO
{
    public override BehaviorCondition CreateCondition()
    {
        return new DetectEnemyCondition();
    }
}

public class DetectEnemyCondition : BehaviorCondition, IBehaviorDatable
{
    public DetectEnemyCondition() { }
    public override BehaviorState Execute()
    {
        var detectedEnemies = GetDetectedEnemies();

        if (IsEnemyDetected(detectedEnemies))
        {
            return BehaviorState.SUCCESS;
        }
        detectedEnemies = UnitConditionHelper.GetEnemiesInRange(FaceUnit);
        return IsEnemyDetected(detectedEnemies) ? BehaviorState.SUCCESS : BehaviorState.FAILURE;
    }
    private bool IsEnemyDetected(List<FaceUnit> enemies)
    {
        return enemies != null && enemies.Count > 0;
    }
    private List<FaceUnit> GetDetectedEnemies()
    {
        return FaceUnit.GetUnitData<List<FaceUnit>, DetectEnemyCondition>();
    }
}
