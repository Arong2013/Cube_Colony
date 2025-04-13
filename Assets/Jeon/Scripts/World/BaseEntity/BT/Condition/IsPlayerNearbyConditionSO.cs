using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Behavior/Condition/IsPlayerNearby")]
public class IsPlayerNearbyConditionSO : BehaviorConditionSO
{
    public float detectionRadius = 5f;

    public override BehaviorCondition CreateCondition()
    {
        var condition = new IsPlayerNearbyCondition();
        condition.GetType().GetField("detectionRadius", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(condition, detectionRadius);
        return condition;
    }
}

public class IsPlayerNearbyCondition : BehaviorCondition
{
    [SerializeField] private float detectionRadius = 5f;
    public override BehaviorState Execute()
    {
        Collider[] hits = Physics.OverlapSphere(entity.transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            PlayerEntity player = hit.GetComponent<PlayerEntity>();
            if (player != null)
            {
                entity.SetData("target",player); 
                return BehaviorState.SUCCESS;
            }
        }
        return BehaviorState.FAILURE;
    }
}
