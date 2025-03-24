using System.Collections.Generic;
using UnityEngine;

public class BehaviorStep
{
    public FaceObject faceObject => parentSequence.faceObject;  // 오타 수정
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
        Debug.Log($"[BehaviorStep] Executing Step → ConditionsMet: {allMet}");

        return allMet ? taskAction.Execute() : BehaviorState.FAILURE;
    }

    public void SetData(string key, object value)
    {
        parentSequence.SetData(key, value);
    }

    public T GetData<T>(string key)
    {
        if (TryGetData<T>(key, out T result))
            return result;

        Debug.LogWarning($"[BehaviorStep] Key {key} not found or wrong type.");
        return default;
    }

    public bool TryGetData<T>(string key, out T value)
    {
        if (parentSequence.TryGetData(key, out var obj) && obj is T casted)
        {
            value = casted;
            return true;
        }
        value = default;
        return false;
    }

    public void SetParentSequence(BehaviorSequence sequence)
    {
        parentSequence = sequence;
    }
}
