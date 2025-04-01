using System;
using UnityEngine;

public class CombatComponent : IEntityComponent
{
    public int Damage { get; private set; } = 10;
    public float AttackCooldown { get; private set; } = 1.0f;

    private float _cooldownTimer = 0f;

    public Action<Entity> OnAttackSuccess;
    public CombatComponent(int damage = 10, float cooldown = 1.0f, Action<Entity> onAttack = null)
    {
        Damage = damage;
        AttackCooldown = cooldown;
        OnAttackSuccess = onAttack;
    }

    public void Start(Entity entity) { }
    public void Update(Entity entity)
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }
    public void Exit(Entity entity) { }
    public bool CanAttack => _cooldownTimer <= 0f;
    public void Attack(Entity target)
    {
        if (!CanAttack) return;

        var health = target.GetEntityComponent<HealthComponent>();
        if (health != null && !health.IsDead)
        {
            health.TakeDamage(Damage);
            _cooldownTimer = AttackCooldown;
            OnAttackSuccess?.Invoke(target);
        }
    }
}
