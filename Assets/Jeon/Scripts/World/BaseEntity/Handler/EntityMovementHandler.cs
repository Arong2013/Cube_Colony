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

        if (direction.x != 0)
        {
            Vector3 scale = _entity.transform.localScale;
            scale.x = direction.x > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            _entity.transform.localScale = scale;
        }
    }
}
