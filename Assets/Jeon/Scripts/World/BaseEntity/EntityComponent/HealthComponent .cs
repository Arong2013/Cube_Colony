using System;
using UnityEngine;

public class HealthComponent : IEntityComponent
{
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }

    public Action<int> OnDamaged;
    public Action OnDeath;
    public HealthComponent(int maxHP,Action<int> onDamaged = null,Action onDeath = null)
    {
        MaxHP = maxHP;
        CurrentHP = maxHP;

        OnDamaged = onDamaged;
        OnDeath = onDeath;
    }

    public void Start(Entity entity) { }

    public void Update(Entity entity) { }

    public void Exit(Entity entity) { }

    public void TakeDamage(int damage)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - damage);
        Debug.Log($"[Health] Took {damage} damage. HP: {CurrentHP}/{MaxHP}");

        OnDamaged?.Invoke(damage);

        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }
    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }
    public bool IsDead => CurrentHP <= 0;
}
