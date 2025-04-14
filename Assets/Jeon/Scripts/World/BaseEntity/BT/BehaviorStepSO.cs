using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBehaviorStep", menuName = "Behavior/Step")]
public class BehaviorStepSO : ScriptableObject
{
    public List<BehaviorConditionSO> conditions;
    public BehaviorActionSO taskAction;

    public BehaviorStep CreateBehaviorStep()
    {
        return new BehaviorStep(conditions, taskAction);
    }
}