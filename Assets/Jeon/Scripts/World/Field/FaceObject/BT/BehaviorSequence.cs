using System.Collections.Generic;
using UnityEngine;

public class BehaviorSequence
{
    private readonly List<BehaviorStep> steps;
    private readonly Dictionary<string, object> decisionContext = new();

    public FaceUnit FaceUnit { get; private set; }

    public BehaviorSequence(FaceUnit FaceUnit, List<BehaviorStepSO> stepSOs)
    {
        this.FaceUnit = FaceUnit;
        steps = new List<BehaviorStep>();

        foreach (var stepSO in stepSOs)
        {
            var step = stepSO.CreateBehaviorStep();
            step.SetParentSequence(this);
            steps.Add(step);
        }
    }

    public void SetData(string key, object value) => decisionContext[key] = value;

    public bool TryGetData(string key, out object value) => decisionContext.TryGetValue(key, out value);

    public bool TryGetData<T>(string key, out T value)
    {
        if (decisionContext.TryGetValue(key, out var obj) && obj is T casted)
        {
            value = casted;
            return true;
        }
        value = default;
        return false;
    }

    public T GetData<T>(string key)
    {
        if (TryGetData<T>(key, out T result)) return result;

        Debug.LogWarning($"[BehaviorSequence] Key '{key}' not found or wrong type.");
        return default;
    }

    public BehaviorState Execute()
    {
        foreach (BehaviorStep step in steps)
        {
            var result = step.Execute();
            Debug.Log($"[BehaviorSequence] Step executed → Result: {result}");

            if (result == BehaviorState.FAILURE) return BehaviorState.FAILURE;
            if (result == BehaviorState.RUNNING) return BehaviorState.RUNNING;
        }
        return BehaviorState.SUCCESS;
    }
}
