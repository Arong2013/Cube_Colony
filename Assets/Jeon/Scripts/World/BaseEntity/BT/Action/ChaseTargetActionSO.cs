using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Behavior/Action/ChaseTarget")]
public class ChaseTargetActionSO : BehaviorActionSO
{
    public float speed = 3f;

    public override BehaviorAction CreateAction()
    {
        var action = new ChaseTargetAction();

        // 속도 설정
        typeof(ChaseTargetAction).GetField("speed", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(action, speed);

        return action;
    }
}
public class ChaseTargetAction : BehaviorAction
{
    [SerializeField] private float speed = 3f;

    private NavMeshPath path;

    public override BehaviorState Execute()
    {
        if (!step.TryGetData<PlayerEntity>("target", out var target))
        {
            Debug.LogWarning("[ChaseTargetAction] No target to chase.");
            return BehaviorState.FAILURE;
        }

        if (path == null)
            path = new NavMeshPath();
        bool success = NavMesh.CalculatePath(entity.transform.position, target.transform.position, NavMesh.AllAreas, path);

        if (!success || path.status != NavMeshPathStatus.PathComplete || path.corners.Length < 2)
        {
            return BehaviorState.FAILURE;
        }
        Vector3 nextCorner = path.corners[1]; // [0]은 현재 위치
        Vector3 direction = (nextCorner - entity.transform.position).normalized;

        return BehaviorState.RUNNING;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}

