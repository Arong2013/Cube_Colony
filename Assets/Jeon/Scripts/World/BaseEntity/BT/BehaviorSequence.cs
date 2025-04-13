using System.Collections.Generic;
using UnityEngine;

public class BehaviorSequence
{
    private readonly List<BehaviorStep> steps;
    public Entity entity { get; private set; }

    public BehaviorSequence(Entity entity, List<BehaviorStepSO> stepSOs)
    {
        this.entity = entity;
        steps = new List<BehaviorStep>();

        foreach (var stepSO in stepSOs)
        {
            var step = stepSO.CreateBehaviorStep();
            step.SetParentSequence(this);
            steps.Add(step);
        }
    }
    public BehaviorState Execute()
    {
        foreach (BehaviorStep step in steps)
        {
            var result = step.Execute();

            if (result == BehaviorState.FAILURE)
                return BehaviorState.FAILURE; // 하나라도 실패하면 시퀀스 실패

            if (result == BehaviorState.RUNNING)
                return BehaviorState.RUNNING; // 진행 중이면 그대로 유지
        }

        // 모든 Step이 SUCCESS인 경우
        return BehaviorState.SUCCESS;
    }
}