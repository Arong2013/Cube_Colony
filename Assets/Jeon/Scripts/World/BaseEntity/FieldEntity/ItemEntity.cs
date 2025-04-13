using UnityEngine;

public class ItemEntity : Entity, IInteractable
{
    [SerializeField] private Item _item;

    [Header("Drop Physics")]
    [SerializeField] private Vector3 dropForce = new Vector3(1f, 5f, 0f);
    [SerializeField] private float stopAfter = 1.5f;

    private Rigidbody _rb;

    public bool CanInteract(Entity interactor)
    {
        return interactor.HasEntityComponent<InventoryComponent>();
    }
    public float GetInteractionDistance() => 0.5f;
    public string GetInteractionLabel()
    {
        return "줍기";
    }
    public override void Initialize()
    {
        
    }
    public void Interact(Entity interactor) => interactor.GetEntityComponent<InventoryComponent>()?.AddItem(_item);
    public override void OnDeath()
    {
     
    }

    public override void OnHit(int dmg)
    {
        
    }

    protected override void Awake()
    {
        base.Awake();
        ApplyDropPhysics();
    }

    private void ApplyDropPhysics()
    {
        _rb = gameObject.AddComponent<Rigidbody>();
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
}
