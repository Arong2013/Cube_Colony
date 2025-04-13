using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Behavior/Action/Wander")]
public class WanderActionSO : BehaviorActionSO
{
    public float arriveDistance = 0.3f;
    public float wanderRadius = 3f;

    public override BehaviorAction CreateAction()
    {
        var action = new WanderAction();
        typeof(WanderAction).GetField("arriveDistance", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(action, arriveDistance);
        typeof(WanderAction).GetField("wanderRadius", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(action, wanderRadius);

        return action;
    }
}
public class WanderAction : BehaviorAction
{
    [SerializeField] private float waitDuration = 1f;
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float arriveDistance = 0.3f;

    private Vector3 initPos;
    private Vector3 targetPos;
    private Vector3 currentDirection;

    private float timer = 0f;
    private bool isWaiting = true;
    private bool initialized = false;

    public override BehaviorState Execute()
    {
      
        if (!initialized)
        {
            initPos = entity.transform.position;
            initialized = true;
        }

        timer += Time.deltaTime;
        if (isWaiting)
        {
            if (timer >= waitDuration)
            {
                timer = 0f;
                isWaiting = false;
                Vector2 offset = Random.insideUnitCircle * wanderRadius;
                targetPos = initPos + new Vector3(offset.x, 0f, offset.y);
                currentDirection = (targetPos - entity.transform.position).normalized;
                entity.SetDir(currentDirection); // 회전/애니메이션
            }

            return BehaviorState.RUNNING;
        }
        else
        {
            float distance = Vector3.Distance(entity.transform.position, targetPos);
            if (distance <= arriveDistance)
            {
                isWaiting = true;
                timer = 0f;
                currentDirection = Vector3.zero;
                entity.SetDir(Vector3.zero); // 회전/애니메이션 정지
            }
            else
            {
                entity.SetDir(currentDirection); // ⚠️ 실제 이동 로직 (Rigidbody든 직접이든)
            }


            return BehaviorState.RUNNING;
        }
    }
}
