using UnityEngine;
using UnityEngine.TextCore.Text;

public class AttackAnimation : StateMachineBehaviour
{
    private Entity _entity;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _entity ??= animator.GetComponent<Entity>();
        _entity.ChangePlayerState(new AttackState(_entity));
    }
}
public class AttackState : EntityState
{
    public AttackState(Entity _entity) : base(_entity) { }
    public override void Execute()
    {

    }
}