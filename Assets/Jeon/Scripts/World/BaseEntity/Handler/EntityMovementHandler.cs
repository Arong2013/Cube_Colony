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
    // 대각선 방향일 때 x, z 값을 1 또는 -1로 정규화
    float moveX = direction.x == 0 ? 0 : Mathf.Sign(direction.x);
    float moveY = direction.z == 0 ? 0 : Mathf.Sign(direction.z);

    // MoveX 파라미터 설정 (-1: 왼쪽, 0: 정지, 1: 오른쪽)
    _entity.SetAnimatorValue(EntityAnimFloat.MoveX, moveX);
    
    // MoveY 파라미터 설정 (-1: 아래쪽, 0: 정지, 1: 위쪽)
    _entity.SetAnimatorValue(EntityAnimFloat.MoveY, moveY);

}


}
