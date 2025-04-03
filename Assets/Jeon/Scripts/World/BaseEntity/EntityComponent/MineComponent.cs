using System;
using UnityEngine;

public class MineComponent : IEntityComponent
{
    public int mineDamage = 1;
    public float mineCooldown = 1f;

    private float _cooldownTimer = 0f;
    public Action<Entity> OnMineSuccess;

    public MineComponent(int damage = 1, float cooldown = 1f, Action<Entity> onMine = null)
    {
        mineDamage = damage;
        mineCooldown = cooldown;
        OnMineSuccess = onMine;
    }

    public void Start(Entity entity) { }

    public void Update(Entity entity)
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    public void Exit(Entity entity) { }

    public bool CanMine => _cooldownTimer <= 0f;

    public void Mine(Entity target)
    {
        if (!CanMine) return;

        //var mineable = target.GetEntityComponent<MineableComponent>();
        //if (mineable != null && !mineable.IsDepleted)
        //{
        //    mineable.TakeMineDamage(mineDamage);
        //    _cooldownTimer = mineCooldown;
        //    OnMineSuccess?.Invoke(target);
        //}
    }
}
