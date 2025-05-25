using UnityEngine;
using UnityEngine.TextCore.Text;
using Sirenix.OdinInspector;

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
    private float _currentTime;
    private PlayerEntity _playerEntity;
    private bool _wasInterrupted = false;

    public ReturnState(Entity _entity, float _returnTime) : base(_entity)
    {
        this._returnTime = _returnTime;
        _playerEntity = _entity.GetComponent<PlayerEntity>();
    }

    public override void Enter()
    {
        base.Enter();

        Utils.GetUI<InSurvivalStateUI>().EnterReturn();
        _currentTime = _returnTime;
        _wasInterrupted = false;

        // 귀환 바를 항상 100%에서 시작
        if (Utils.GetUI<InSurvivalStateUI>() is InSurvivalStateUI survivalStateUI)
            survivalStateUI.ResetReturnProgress();
    }

    public override void Execute()
    {
        _currentTime -= Time.deltaTime;
        Utils.GetUI<InSurvivalStateUI>().UpdateReturn(_returnTime, _currentTime);

        if (_currentTime <= 0)
        {
            CompleteReturn();
        }
    }

    public override void Exit()
    {
        base.Exit();
        Utils.GetUI<InSurvivalStateUI>().ExitReturn();
        
    }

    private void InterruptReturn()
    {
        _wasInterrupted = true;
        _entity.SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Idle);
    }

    private void CompleteReturn()
    {
        _entity.SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Idle);
BattleFlowController.Instance?.CheckStageCompletionOnReturn();
    }
}