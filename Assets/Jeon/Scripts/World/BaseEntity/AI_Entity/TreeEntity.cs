using System.Linq;
using UnityEngine;

public class TreeEntity : Entity, IInteractable
{
    DropEntry _dropEntry;
    protected override void Awake()
    {
        base.Awake();
        AddEntityComponent(new HealthComponent(5, OnDamaged, OnDeath));
    }
    public bool CanInteract(Entity interactor)
    {
       
        return interactor.HasEntityComponent<ChopComponent>(); 
    }
    public string GetInteractionLabel()
    {
        return "";
    }
    public void Interact(Entity interactor)
    {
        
        var combat = interactor.GetEntityComponent<ChopComponent>();
        
        combat?.Chop(this);
    }
    private void OnDamaged(int dmg)
    {
        Debug.Log($"[Enemy] 피해 {dmg} 받음!");
    }
    private void OnDeath()
    {
        Debug.Log("[Enemy] 사망!");
        DropItems(_dropEntry);
        Destroy(gameObject);
    }
    public void DropItems(DropEntry dropEntry)
    {
        int dropCount = UnityEngine.Random.Range(dropEntry.MinDropItem, dropEntry.MaxDropItem + 1);

        for (int i = 0; i < dropCount; i++)
        {
            float roll = UnityEngine.Random.value;
            float cumulative = 0f;

            foreach (var pair in dropEntry.DropChances.OrderByDescending(p => p.Key))
            {
                cumulative += pair.Key;
                if (roll <= cumulative)
                {
                    int itemId = pair.Value;
                    Debug.Log(itemId);
                    break;
                }
            }
        }
    }

}