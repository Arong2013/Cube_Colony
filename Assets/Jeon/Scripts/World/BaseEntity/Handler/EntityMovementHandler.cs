using UnityEngine;

public class EntityMovementHandler
{
    private readonly Entity _entity;
    private readonly Rigidbody _rigidbody;

    public EntityMovementHandler(Entity entity)
    {
        _entity = entity;
        _rigidbody = entity.GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("[EntityMovementHandler] Rigidbody가 Entity에 없습니다.");
        }
    }

    public void Move(Vector3 direction)
    {
        if (_rigidbody == null) return;

        float speed = _entity.GetEntityStat(EntityStatName.SPD);
        Vector3 velocity = direction.normalized * speed;

        _rigidbody.linearVelocity = new Vector3(velocity.x, _rigidbody.linearVelocity.y, velocity.z); // y축은 중력 유지
        UpdateAnimatorParameters(direction);
    }

    private void UpdateAnimatorParameters(Vector3 direction)
    {
        // MoveX 파라미터 설정 (-1: 왼쪽, 1: 오른쪽)
        _entity.SetAnimatorValue(EntityAnimFloat.MoveX, direction.x);
        
        // MoveY 파라미터 설정 (-1: 아래쪽, 1: 위쪽)
        // 3D 공간에서는 z축이 앞/뒤 방향이므로 z값을 MoveY에 매핑
        _entity.SetAnimatorValue(EntityAnimFloat.MoveY, direction.z);
        
        // 이동 속도 파라미터 설정 (선택적)
        float moveSpeed = direction.magnitude;
        _entity.SetAnimatorValue(EntityAnimFloat.Speed, moveSpeed);
    }
}
