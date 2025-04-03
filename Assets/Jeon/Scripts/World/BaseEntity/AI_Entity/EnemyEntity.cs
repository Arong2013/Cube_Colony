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
        return "�����ϱ�";
    }
    private void OnDamaged(int dmg)
    {
        Debug.Log($"[Enemy] ���� {dmg} ����!");
    }
    private void OnDeath()
    {
        Debug.Log("[Enemy] ���!");
        Destroy(gameObject);
    }
}
