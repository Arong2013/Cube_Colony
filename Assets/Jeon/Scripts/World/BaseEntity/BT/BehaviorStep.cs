using System.Collections.Generic;
using UnityEngine;

public class BehaviorStep
{
    public Entity  entity => parentSequence.entity;  // 오타 수정
    private BehaviorSequence parentSequence;
    private List<BehaviorCondition> conditions;
    private BehaviorAction taskAction;

    public BehaviorStep(List<BehaviorConditionSO> conditionSOs, BehaviorActionSO actionSO)
    {
        conditions = new List<BehaviorCondition>();
        foreach (var conditionSO in conditionSOs)
        {
            var condition = conditionSO.CreateCondition();
            condition.SetParent(this);
            conditions.Add(condition);
        }

        taskAction = actionSO.CreateAction();
        taskAction.SetParent(this);
    }

    public BehaviorState Execute()
    {
        bool allMet = conditions.TrueForAll(c => c.Execute() == BehaviorState.SUCCESS);

        return allMet ? taskAction.Execute() : BehaviorState.FAILURE;
    }

    public void SetParentSequence(BehaviorSequence sequence)
    {
        parentSequence = sequence;
    }
}