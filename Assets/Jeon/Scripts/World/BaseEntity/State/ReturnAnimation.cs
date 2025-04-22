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
    private float currentTime;  
    private PlayerEntity playerEntity;
    public ReturnState(Entity _entity, float _returnTime) : base(_entity) { this._returnTime = _returnTime; playerEntity = _entity.GetComponent<PlayerEntity>(); }
    public override void Enter()
    {
        base.Enter();
        Utils.GetUI<InSurvivalStateUI>().EnterReturn();
        currentTime = _returnTime;  
    }
    public override void Execute()
    {
        currentTime -= Time.deltaTime;
        Utils.GetUI<InSurvivalStateUI>().UpdateReturn(_returnTime,currentTime);
        if (currentTime <= 0)
        {
            Utils.GetUI<InSurvivalStateUI>().ExitReturn();
            _entity.SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Idle);
            playerEntity.SeReturnStageState();   
        }
    }
}