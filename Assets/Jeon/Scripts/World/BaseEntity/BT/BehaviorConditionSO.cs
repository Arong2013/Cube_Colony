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
    protected Entity  entity  => step.entity;
    protected BehaviorStep step;

    public abstract BehaviorState Execute();

    public void SetParent(BehaviorStep step)
    {
        this.step = step;
    }
}
public abstract class BehaviorAction
{
    protected Entity  entity => step.entity;
    protected BehaviorStep step;
    public abstract BehaviorState Execute();
    public void SetParent(BehaviorStep step)
    {
        this.step = step;
    }
}