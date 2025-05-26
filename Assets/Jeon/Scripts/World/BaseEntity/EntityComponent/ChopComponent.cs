using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ChopComponent : IEntityComponent
{
    private Entity _entity;
    public void Start(Entity entity) => _entity = entity;
    public void Update(Entity entity) { }
    public void Exit(Entity entity) { }
    public void Chop(Entity target)
    {     
        _entity.SetTarget(target);
        _entity.SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Attack);
    }
}
