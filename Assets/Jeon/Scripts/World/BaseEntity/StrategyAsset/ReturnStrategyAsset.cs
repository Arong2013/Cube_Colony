using UnityEngine;

[CreateAssetMenu(menuName = "InteractionStrategy/Return")]
public class ReturnStrategyAsset : ScriptableInteractionStrategy
{
    public override IInteractionStrategy CreateStrategy()
        => new ReturnStrategy();
}
public class ReturnStrategy : IInteractionStrategy
{
    public bool CanInteract(Entity self, Entity interactor)
        => interactor.HasEntityComponent<ReturnComponent>();
    public void Interact(Entity self, Entity interactor)
        => interactor.GetEntityComponent<ReturnComponent>()?.ReturnStage(interactor);
    public string GetLabel() => "돌아가";

    public void Initialize(Entity self)
    {
        
    }
}
public class ReturnComponent : IEntityComponent
{
    public void Start(Entity entity) { }
    public void Update(Entity entity) { }
    public void Exit(Entity entity) { }
    public void ReturnStage(Entity entity)
    {
        if (entity == null) return;
        if(entity is PlayerEntity player)
        {
            player.SeReturnStageState();
            entity.SetAnimatorValue(EntityAnimBool.IsReturn, true);
        }

    }   
}   