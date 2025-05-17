using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InteractableEntity : Entity, IInteractable
{
    [SerializeField] private List<BehaviorSequenceSO> behaviorSequencesSO;
    [SerializeField] private ScriptableInteractionStrategy strategyAsset;
    [SerializeField] private float interactionDistance;
    private IInteractionStrategy _strategy;
    private bool initavle;


    [SerializeField] int MinDropItem;
    [SerializeField] int MaxDropItem;

    [SerializeField] Dictionary<float, int> DropChances;
    public override void Init()
    {
        base.Init();
        _strategy = strategyAsset?.CreateStrategy();
        _strategy?.Initialize(this);    
        if (behaviorSequencesSO != null)
        {
            SetController(new AIController(behaviorSequencesSO,this));
        }
        initavle = true;    
    }
    protected override void Update()
    {
        if (!initavle) return;  
        base.Update();
    }
    public bool CanInteract(Entity interactor) => _strategy?.CanInteract(this, interactor) ?? false;
    public void Interact(Entity interactor) => _strategy?.Interact(this, interactor);
    public string GetInteractionLabel() => _strategy?.GetLabel() ?? "상호작용";
    public void DropItems()
    {
        int dropCount = Random.Range(MinDropItem, MaxDropItem + 1);

        for (int i = 0; i < dropCount; i++)
        {
            float roll = Random.value;
            float cumulative = 0f;

            foreach (var pair in DropChances.OrderByDescending(p => p.Key))
            {
                cumulative += pair.Key;
                if (roll <= cumulative)
                {
                    int itemId = DropChances[pair.Key];
                    var itemPre =  Instantiate(DataCenter.Instance.GetDropItemPrefab().gameObject, transform.position, Quaternion.identity);     
                    itemPre.GetComponent<ItemEntity>().SetItem(itemId); 
                    break;
                }
            }
        }
    }
    public float GetInteractionDistance() => interactionDistance;

    public override void OnHit(int dmg)
    {
        
    }

    public override void OnDeath()
    {
        DropItems();
        Destroy(gameObject);    
    }
}
