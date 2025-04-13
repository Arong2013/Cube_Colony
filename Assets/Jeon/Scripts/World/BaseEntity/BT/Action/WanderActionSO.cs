using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Behavior/Action/Wander")]
public class WanderActionSO : BehaviorActionSO
{
    public override BehaviorAction CreateAction() => new WanderAction();
}

public class WanderAction : BehaviorAction
{
    private Vector3 target;
    private bool hasTarget = false;
    private float speed = 2f;
    private float arriveDistance = 0.3f;
    private float wanderRadius = 3f;

    private NavMeshPath path;

    public override BehaviorState Execute()
    {
        if (path == null)
            path = new NavMeshPath();

        if (!hasTarget)
        {
            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = entity.transform.position + new Vector3(offset.x, 0, offset.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                NavMesh.CalculatePath(entity.transform.position, hit.position, NavMesh.AllAreas, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    target = hit.position;
                    hasTarget = true;
                }
                else
                {
                    return BehaviorState.FAILURE;
                }
            }
            else
            {
                return BehaviorState.FAILURE;
            }
        }

        // 현재 위치에서 다음 지점 방향으로 이동
        Vector3 direction = (target - entity.transform.position).normalized;
        entity.transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(entity.transform.position, target) < arriveDistance)
        {
            hasTarget = false;
            return BehaviorState.SUCCESS;
        }

        return BehaviorState.RUNNING;
    }
}
