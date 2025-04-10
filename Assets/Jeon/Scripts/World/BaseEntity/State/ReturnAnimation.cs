using UnityEngine;
using UnityEngine.TextCore.Text;

public class ReturnAnimation : StateMachineBehaviour
{
    private Entity _entity;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _entity ??= animator.GetComponent<Entity>();
        _entity.ChangePlayerState(new ReturnState(_entity));
    }
}
public class ReturnState : EntityState
{
    public ReturnState(Entity _entity) : base(_entity) { }
    public override void Execute()
    {

    }
}