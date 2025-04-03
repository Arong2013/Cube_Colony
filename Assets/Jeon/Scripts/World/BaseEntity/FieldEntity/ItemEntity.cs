using Unity.VisualScripting;

public class ItemEntity : Entity, IInteractable
{
    Item _item;
    public bool CanInteract(Entity interactor)
    {
        return interactor.HasEntityComponent<InventoryComponent>();
    }
    public string GetInteractionLabel()
    {
        throw new System.NotImplementedException();
    }
    public void Interact(Entity interactor)
    {
        var inventory = interactor.GetEntityComponent<InventoryComponent>();
        inventory.AddItem(_item);
    }
    protected override void Awake()
    {
        base.Awake();
    }
}