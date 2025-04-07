using UnityEngine;

public class EntityMovementHandler
{
    private readonly Entity _entity;
    public EntityMovementHandler(Entity entity)
    {
        _entity = entity;
    }

    public void Move(Vector3 direction)
    {

        Vector3 moveDelta = direction.normalized * _entity.GetEntityStat(EntityStatName.SPD) * Time.deltaTime;
        _entity.transform.position += moveDelta;

        if (direction.x != 0)
        {
            Vector3 scale = _entity.transform.localScale;
            scale.x = direction.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            _entity.transform.localScale = scale;
        }
    }
}