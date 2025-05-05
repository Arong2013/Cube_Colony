using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Behavior/Action/ChaseTarget")]
public class ChaseTargetActionSO : BehaviorActionSO
{
    public float maxChaseTime = 5f;
   public float waitDuration = 0.5f;
    public override BehaviorAction CreateAction()
    {
        var action = new ChaseTargetAction();
        typeof(ChaseTargetAction).GetField("maxChaseTime", BindingFlags.NonPublic | BindingFlags.Instance)
    ?.SetValue(action, maxChaseTime);
        typeof(ChaseTargetAction).GetField("waitDuration", BindingFlags.NonPublic | BindingFlags.Instance)
?.SetValue(action, waitDuration);
        return action;
    }
}
public class ChaseTargetAction : BehaviorAction
{
    [SerializeField] private float waitDuration = 0.5f;
    [SerializeField] private float arriveDistance = 0.3f;
    [SerializeField] private float maxChaseTime = 5f;

    private NavMeshPath path;
    private Vector3 targetPos;
    private Vector3 currentDirection;
    private float chaseTimer = 0f;

    public override BehaviorState Execute()
    {
        if (!entity.TryGetData<PlayerEntity>("target", out var target))
        {
            
            return BehaviorState.FAILURE;
        }

        if (path == null)
            path = new NavMeshPath();

        chaseTimer += Time.deltaTime;

        // ⏰ 추적 시간 초과 시 중단
        if (chaseTimer >= maxChaseTime)
        {
            StopChase();
            return BehaviorState.FAILURE;
        }

        // 📍 경로 계산 실패 시 중단
        if (!NavMesh.CalculatePath(entity.transform.position, target.transform.position, NavMesh.AllAreas, path) ||
            path.status != NavMeshPathStatus.PathComplete || path.corners.Length < 2)
        {
            StopChase();
            return BehaviorState.SUCCESS;
        }

        // 🎯 이동 방향 설정
        targetPos = path.corners[1];
        currentDirection = (targetPos - entity.transform.position).normalized;
        
        float dist = Vector3.Distance(entity.transform.position, targetPos);
        if (dist <= arriveDistance)
        {
            StopChase();

            return BehaviorState.SUCCESS;
        }
        else
        {
            entity.SetDir(currentDirection);
        }
        
        return BehaviorState.RUNNING;
    }

    private void StopChase()
    {
        entity.RemoveData("target");
        entity.SetDir(Vector3.zero);
        currentDirection = Vector3.zero;
        chaseTimer = 0f;

    }
}