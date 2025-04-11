using UnityEngine;
using UnityEngine.TextCore.Text;

public class MoveAnimation : StateMachineBehaviour
{
    private Entity _entity;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _entity ??= animator.GetComponent<Entity>();
        _entity.ChangePlayerState(new MoveState(_entity));
    }
}
public class MoveState : EntityState
{
    public MoveState(Entity _entity) : base(_entity) { }
    public override void Execute()
    {
        _entity.Move();
        if(_entity.CurrentDir == Vector3.zero)
        {
            _entity.SetAnimatorValue(EntityAnimBool.IsMoving, false);
            
        }   
    }
}