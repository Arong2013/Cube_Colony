using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InteractableEntity : Entity, IInteractable
{
    [SerializeField] private List<BehaviorSequenceSO> behaviorSequencesSO;
    [SerializeField] private ScriptableInteractionStrategy strategyAsset;
    [SerializeField] private DropEntry dropEntry;
    [SerializeField] private float interactionDistance;
    private IInteractionStrategy _strategy;
    protected override void Awake()
    {
        base.Awake();
        _strategy = strategyAsset?.CreateStrategy();
        if(behaviorSequencesSO != null)
        {
            SetController(new AIController(behaviorSequencesSO,this));
        }
    }

    protected override void Update()
    {
        base.Update();
    }
    public bool CanInteract(Entity interactor) => _strategy?.CanInteract(this, interactor) ?? false;
    public void Interact(Entity interactor) => _strategy?.Interact(this, interactor);
    public string GetInteractionLabel() => _strategy?.GetLabel() ?? "상호작용";
    private void OnDamaged(int dmg) => Debug.Log($"[Entity] 피해 {dmg} 받음!");
    private void OnDeath()
    {
        Debug.Log("[Entity] 사망!");

        if (dropEntry != null)
            DropItems(dropEntry);
        Destroy(gameObject);
    }
    public void DropItems(DropEntry dropEntry)
    {
        int dropCount = Random.Range(dropEntry.MinDropItem, dropEntry.MaxDropItem + 1);

        for (int i = 0; i < dropCount; i++)
        {
            float roll = Random.value;
            float cumulative = 0f;

            foreach (var pair in dropEntry.DropChances.OrderByDescending(p => p.Key))
            {
                cumulative += pair.Key;
                if (roll <= cumulative)
                {
                    int itemId = pair.Value;
                    Debug.Log($"Dropped item ID: {itemId}");
                    break;
                }
            }
        }
    }

    public override void Initialize()
    {
        
    }

    public float GetInteractionDistance() => interactionDistance;
}
