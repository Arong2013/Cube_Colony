using UnityEngine;

public class MovementComponent : IEntityComponent
{
    private Transform _transform;
    private float _speed = 5f;

    public void Start(Entity entity)
    {
        _transform = entity.transform;
    }

    public void Update(Entity entity) { }

    public void Exit(Entity entity) { }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;

        Vector3 moveDelta = direction.normalized * _speed * Time.deltaTime;
        _transform.position += moveDelta;
    }
}
