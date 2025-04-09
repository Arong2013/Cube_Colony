using UnityEngine;

[CreateAssetMenu(menuName = "InteractionStrategy/Chop")]
public class ExitStrategyAsset : ScriptableInteractionStrategy
{
    public override IInteractionStrategy CreateStrategy()
        => new ChopStrategy();
}
public class ExitStrategy : IInteractionStrategy
{
    public bool CanInteract(Entity self, Entity interactor)
        => interactor.HasEntityComponent<ChopComponent>();
    public void Interact(Entity self, Entity interactor)
        => interactor.GetEntityComponent<ChopComponent>()?.Chop(self);
    public string GetLabel() => "나무 베기";
}
public class ExitComponent : IEntityComponent
{
    public void Start(Entity entity) { }
    public void Update(Entity entity) { }
    public void Exit(Entity entity) { }
}   