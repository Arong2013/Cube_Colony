using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Behavior/Action/Wander")]
public class WanderActionSO : BehaviorActionSO
{
    public float speed = 2f;
    public float arriveDistance = 0.3f;
    public float wanderRadius = 3f;

    public override BehaviorAction CreateAction()
    {
        var action = new WanderAction();

        typeof(WanderAction).GetField("speed", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(action, speed);
        typeof(WanderAction).GetField("arriveDistance", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(action, arriveDistance);
        typeof(WanderAction).GetField("wanderRadius", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(action, wanderRadius);

        return action;
    }
}

public class WanderAction : BehaviorAction
{
    private Vector3 target;
    private bool hasTarget = false;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float arriveDistance = 0.3f;
    [SerializeField] private float wanderRadius = 3f;

    public override BehaviorState Execute()
    {
        
        if (!hasTarget)
        {
            // 랜덤한 방향으로 새 위치 설정
            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = entity.transform.position + new Vector3(offset.x, 0, offset.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                target = hit.position;
                hasTarget = true;
            }
            else
            {
                return BehaviorState.FAILURE;
            }
        }
        Vector3 direction = (target - entity.transform.position).normalized;
        Debug.Log(direction);
        if (Vector3.Distance(entity.transform.position, target) < arriveDistance)
        {
            hasTarget = false;
            return BehaviorState.SUCCESS;
        }

        return BehaviorState.RUNNING;
    }
}
