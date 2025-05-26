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
    {
        var chopComponent = interactor.GetEntityComponent<ChopComponent>();
        Debug.Log($"[ChopStrategy] Interacting with {self.name} using {nameof(chopComponent)}");  
        chopComponent?.Chop(self);
    }
    public string GetLabel() => "나무 베기";

    public void Initialize(Entity self)
    {
        
    }
}
