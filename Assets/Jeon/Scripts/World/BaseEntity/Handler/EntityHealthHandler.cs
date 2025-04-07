using System;
using UnityEngine;

public class EntityHealthHandler
{
    private readonly Entity _entity;
    Action<int> onDamaged;
    Action onDeath;

    public EntityHealthHandler(Entity entity, Action<int> onDamaged = null, Action onDeath = null)
    {
        _entity = entity;
       this.onDamaged = onDamaged;
       this.onDeath = onDeath;
    }

    public void TakeDamage(float amount)
    {
        float hp = _entity.GetEntityStat(EntityStatName.HP);
        float newHp = MathF.Max(0, hp - amount);
        _entity.SetEntityBaseStat(EntityStatName.HP, newHp);
        _entity.SetAnimatorValue(EntityAnimTrigger.HitTrigger, null);
        Debug.Log("데미지 받음");
    }
    public void Heal(float amount)
    {
        float hp = _entity.GetEntityStat(EntityStatName.HP);
        float maxHp = _entity.GetEntityStat(EntityStatName.MaxHP);
        float healed = MathF.Min(hp + amount, maxHp);
        _entity.SetEntityBaseStat(EntityStatName.HP, healed);
    }
    public bool IsDead => _entity.GetEntityStat(EntityStatName.HP) <= 0;
}
