using UnityEngine;

public class PlayerEntity : Entity
{
    protected override void Awake()
    {
        base.Awake();

        AddEntityComponent(new HealthComponent(100, OnPlayerDamaged, OnPlayerDeath));
        AddEntityComponent(new MovementComponent());
        AddEntityComponent(new ChopComponent());

        SetController(new PCController(OnMoveInput));
    }
    private void OnPlayerDamaged(int dmg)
    {
        Debug.Log($"[UI] Player took {dmg} damage!");
    }
    private void OnPlayerDeath()
    {
        Debug.Log("[UI] Player died!");
    }
    private void OnMoveInput(Vector3 direction)
    {
        var mover = GetEntityComponent<MovementComponent>();
        mover?.Move(direction);
    }
    private void Attack(Entity target)
    {
        var combat = GetEntityComponent<CombatComponent>();
        combat?.Attack(target);
    }
}
