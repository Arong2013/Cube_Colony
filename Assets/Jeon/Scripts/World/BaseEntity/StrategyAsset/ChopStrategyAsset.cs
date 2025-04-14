using UnityEngine;

[CreateAssetMenu(menuName = "InteractionStrategy/Chop")]
public class ChopStrategyAsset : ScriptableInteractionStrategy
{
    public override IInteractionStrategy CreateStrategy()
        => new ChopStrategy();
}
public class ChopStrategy : IInteractionStrategy
{
    public bool CanInteract(Entity self, Entity interactor)
        => interactor.HasEntityComponent<ChopComponent>();
    public void Interact(Entity self, Entity interactor)
        => interactor.GetEntityComponent<ChopComponent>()?.Chop(self);
    public string GetLabel() => "나무 베기";

    public void Initialize(Entity self)
    {
        
    }
}
