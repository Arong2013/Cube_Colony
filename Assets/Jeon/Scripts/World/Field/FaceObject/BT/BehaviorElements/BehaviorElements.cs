using UnityEngine;
public abstract class BehaviorConditionSO : ScriptableObject
{
    public abstract BehaviorCondition CreateCondition();
}
public abstract class BehaviorActionSO : ScriptableObject
{
    public abstract BehaviorAction CreateAction();
}
public abstract class BehaviorCondition
{
    protected FaceUnit FaceUnit => step.FaceUnit;
    protected BehaviorStep step;

    public abstract BehaviorState Execute();

    public void SetParent(BehaviorStep step)
    {
        this.step = step;
    }
}
public abstract class BehaviorAction
{
    protected FaceUnit FaceUnit => step.FaceUnit;
    protected BehaviorStep step;
    public abstract BehaviorState Execute();
    public void SetParent(BehaviorStep step)
    {
        this.step = step;
    }
}
