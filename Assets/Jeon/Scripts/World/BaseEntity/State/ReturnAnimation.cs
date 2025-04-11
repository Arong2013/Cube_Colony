using UnityEngine;
using UnityEngine.TextCore.Text;

public class ReturnAnimation : StateMachineBehaviour
{
    [SerializeField] float _returnTime = 1f;    
    private Entity _entity;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _entity ??= animator.GetComponent<Entity>();
        _entity.ChangePlayerState(new ReturnState(_entity, _returnTime));
    }
}
public class ReturnState : EntityState
{
    private float _returnTime;  
    public ReturnState(Entity _entity, float _returnTime) : base(_entity) { this._returnTime = _returnTime;}
    public override void Execute()
    {
        _returnTime -= Time.deltaTime;
        if (_returnTime <= 0)
        {
            _entity.SetAnimatorValue(EntityAnimBool.IsReturn, false);
            _entity.SeReturnStageState();   
        }
    }
}