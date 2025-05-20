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
        float newHp = Mathf.Max(0, hp - amount);
        _entity.SetEntityStatModifier(EntityStatName.HP, this, newHp);
        _entity.SetAnimatorValue(EntityAnimTrigger.HitTrigger, null);
        onDamaged?.Invoke((int)amount); 
        if (IsDead)
        {
            onDeath?.Invoke();  
        }
    }
    public void Heal(float amount)
    {
        float hp = _entity.GetEntityStat(EntityStatName.HP);
        float maxHp = _entity.GetEntityStat(EntityStatName.MaxHP);
        float healed = Mathf.Min(hp + amount, maxHp);
        _entity.SetEntityStatModifier(EntityStatName.HP, this, healed);
    }
    public bool IsDead => _entity.GetEntityStat(EntityStatName.HP) <= 0;
}
