using System;
using UnityEngine;

public class ChopComponent : IEntityComponent
{
    public int ChopDamage { get; private set; } = 5;
    public float ChopCooldown { get; private set; } = 1.0f;

    private float _cooldownTimer = 0f;

    public Action<Entity> OnChopSuccess;

    public ChopComponent(int chopDamage = 5, float cooldown = 1.0f, Action<Entity> onChop = null)
    {
        ChopDamage = chopDamage;
        ChopCooldown = cooldown;
        OnChopSuccess = onChop;
    }

    public void Start(Entity entity) { }

    public void Update(Entity entity)
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    public void Exit(Entity entity) { }

    public bool CanChop => _cooldownTimer <= 0f;

    public void Chop(Entity target)
    {
        if (!CanChop) return;

        var health = target.GetEntityComponent<HealthComponent>();
        if (health != null)
        {
            health.TakeDamage(ChopDamage);
            _cooldownTimer = ChopCooldown;
            OnChopSuccess?.Invoke(target);
        }
    }
}
