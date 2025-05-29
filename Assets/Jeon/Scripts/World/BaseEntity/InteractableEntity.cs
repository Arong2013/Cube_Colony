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
    Field currentField = FindAnyObjectByType<Field>();
    
    if (currentField == null)
    {
        Debug.LogWarning("현재 필드를 찾을 수 없습니다.");
        return;
    }

    int dropCount = Random.Range(MinDropItem, MaxDropItem + 1);

    foreach (var pair in DropChances)
    {
        // 각 아이템에 대해 개별적으로 드롭 여부 결정
        float dropChance = pair.Key;
        int itemId = pair.Value;

        for (int i = 0; i < dropCount; i++)
        {
            if (Random.value <= dropChance)
            {
                var itemPre = Instantiate(
                    DataCenter.Instance.GetDropItemPrefab().gameObject, 
                    transform.position, 
                    Quaternion.identity, 
                    currentField.transform.Find("DisableField")
                );     
                itemPre.GetComponent<ItemEntity>().SetItem(itemId); 
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
