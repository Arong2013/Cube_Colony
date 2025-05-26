using System;
using UnityEngine;

public class IdleAnimation : StateMachineBehaviour
{
    private Entity _entity;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _entity ??= animator.GetComponent<Entity>();
        _entity.ChangePlayerState(new IdleState(_entity, animator));
    }
}
public class IdleState : EntityState
{
    Animator animator;
    public IdleState(Entity _entity, Animator animator) : base(_entity) { this.animator = animator; }
    public override void Enter()
    {
        base.Enter();
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(param.name, false);
                    break;

                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(param.name, 0f);
                    break;

                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(param.name, 0);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(param.name);
                    break;
            }
        }
    }
    public override void Execute()
    {
        
    }
}