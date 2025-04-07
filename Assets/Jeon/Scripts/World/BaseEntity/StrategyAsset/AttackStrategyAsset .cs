using UnityEngine;

[CreateAssetMenu(menuName = "InteractionStrategy/Attack")]
public class AttackStrategyAsset : ScriptableInteractionStrategy
{
    public override IInteractionStrategy CreateStrategy() => new AttackStrategy();
}

public class AttackStrategy : IInteractionStrategy
{
    public bool CanInteract(Entity self, Entity interactor) => interactor.HasEntityComponent<AttackComponent>();
    public void Interact(Entity self, Entity interactor) => interactor.GetEntityComponent<AttackComponent>()?.Attack(self);
    public string GetLabel() => "공격하기";
}
