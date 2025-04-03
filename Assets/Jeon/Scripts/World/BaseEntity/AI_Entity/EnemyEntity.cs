using UnityEngine;

public class EnemyEntity : Entity, IInteractable
{
    protected override void Awake()
    {
        base.Awake();
        AddEntityComponent(new HealthComponent(50, OnDamaged, OnDeath));
    }
    public bool CanInteract(Entity interactor)
    {
        return interactor.HasEntityComponent<CombatComponent>();
    }
    public void Interact(Entity interactor)
    {
        var combat = interactor.GetEntityComponent<CombatComponent>();
        combat?.Attack(this);
    }
    public string GetInteractionLabel()
    {
        return "공격하기";
    }
    private void OnDamaged(int dmg)
    {
        Debug.Log($"[Enemy] 피해 {dmg} 받음!");
    }
    private void OnDeath()
    {
        Debug.Log("[Enemy] 사망!");
        Destroy(gameObject);
    }
}
