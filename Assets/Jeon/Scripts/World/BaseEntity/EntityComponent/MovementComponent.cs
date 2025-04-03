using UnityEngine;

public class MovementComponent : IEntityComponent
{
    private Transform _transform;
    private float _speed = 5f;
    private Entity _entity;

    public void Start(Entity entity)
    {
        _transform = entity.transform;
        _entity = entity;   
    }

    public void Update(Entity entity) { }

    public void Exit(Entity entity) { }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            _entity.SetAnimatorValue(EntityAnimeBoolName.IsWalk, false);
            return;
        }
       
        Vector3 moveDelta = direction.normalized * _speed * Time.deltaTime;
        _transform.position += moveDelta;

        if (direction.x != 0)
        {
            Vector3 scale = _transform.localScale;
            scale.x = direction.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            _transform.localScale = scale;
            
        }
        _entity.SetAnimatorValue(EntityAnimeBoolName.IsWalk, true);
    }
}
