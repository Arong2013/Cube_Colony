﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InteractableEntity : Entity, IInteractable
{
    [SerializeField] private List<BehaviorSequenceSO> behaviorSequencesSO;
    [SerializeField] private ScriptableInteractionStrategy strategyAsset;
    [SerializeField] private DropEntry dropEntry;
    [SerializeField] private float interactionDistance;
    private IInteractionStrategy _strategy;
    private bool initavle;
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
        Destroy(gameObject);    
    }
}
