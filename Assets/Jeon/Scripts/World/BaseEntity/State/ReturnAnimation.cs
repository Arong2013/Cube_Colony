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
    }

    public override void Execute()
    {
        // 귀환 중 다른 입력이나 이동이 있으면 귀환 취소
        if (_entity.CurrentDir != Vector3.zero ||
            Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1))
        {
            InterruptReturn();
            return;
        }

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

        // 귀환 UI를 종료하고 바를 꺼줌 (항상 귀환바 끄기)
        Utils.GetUI<InSurvivalStateUI>().ExitReturn();
    }

    private void InterruptReturn()
    {
        _wasInterrupted = true;

        // 상태 변경
        _entity.SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Idle);
    }

    private void CompleteReturn()
    {
        _entity.SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Idle);
        _playerEntity.SeReturnStageState();
    }
}