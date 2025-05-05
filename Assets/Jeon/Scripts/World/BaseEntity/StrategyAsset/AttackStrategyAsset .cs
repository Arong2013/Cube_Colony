using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "InteractionStrategy/Attack")]
public class AttackStrategyAsset : ScriptableInteractionStrategy
{
    [SerializeField] float _attackRange = 3f;
    public override IInteractionStrategy CreateStrategy() => new AttackStrategy(_attackRange);
}
public class AttackStrategy : IInteractionStrategy
{
    private float _attackRange = 3f;
    public AttackStrategy(float _attackRange)
    {
        this._attackRange = _attackRange;
    }
    public bool CanInteract(Entity self, Entity interactor) => interactor.HasEntityComponent<AttackComponent>();
    public void Interact(Entity self, Entity interactor) => interactor.GetEntityComponent<AttackComponent>()?.Attack(self);
    public string GetLabel() => "공격하기";
    public void Initialize(Entity self)
    {
        self.AddEntityComponent<AttackComponent>(new AttackComponent(_attackRange)); 
    }
}
