using UnityEngine;

public class EntityCombatHandler
{
    private readonly Entity _entity;

    public EntityCombatHandler(Entity entity)
    {
        _entity = entity;
    }
    public void Attack(Entity target)
    {
        float atk = _entity.GetEntityStat(EntityStatName.ATK);
        target.TakeDamage(atk);
    }
}
