using System;
using UnityEngine;

public class ItemEntity : Entity, IInteractable
{
    [SerializeField] private Item _item;
    [SerializeField] private SpriteRenderer itemSprite;
    [Header("Drop Physics")]
    [SerializeField] private Vector3 dropForce = new Vector3(1f, 5f, 0f);
    [SerializeField] private float stopAfter = 1.5f;

    private Rigidbody _rb;
    public override void Init()
    {
        base.Init();
        _rb = GetComponent<Rigidbody>(); 
    }
    public bool CanInteract(Entity interactor)
    {
        
        return interactor.HasEntityComponent<InventoryComponent>();
    }
    public float GetInteractionDistance() => 1f;
    public string GetInteractionLabel()
    {
        return "줍기";
    }

    public void Interact(Entity interactor)
    {
        interactor.GetEntityComponent<InventoryComponent>()?.AddItem(_item);
        Destroy(gameObject);    
    } 
    public override void OnDeath()
    {
     
    }

    public override void OnHit(int dmg)
    {
        
    }
    private void ApplyDropPhysics()
    {
        _rb.AddForce(dropForce, ForceMode.Impulse);
        Invoke(nameof(FreezeDrop), stopAfter);
    }
    private void FreezeDrop()
    {
        if (_rb == null) return;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true; 
        Destroy(_rb);           
    }
    public void SetItem(int itemId)
    {
        _item = ItemDataCenter.Get<ConsumableItem>(itemId);
        itemSprite.sprite = _item.ItemIcon;
        Init();
    }
}
