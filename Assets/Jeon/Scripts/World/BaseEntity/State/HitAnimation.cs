using UnityEngine;

public class HitAnimation : StateMachineBehaviour
{
    private Entity _entity;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _entity ??= animator.GetComponent<Entity>();

        // HitState로 전환
        _entity.ChangePlayerState(new HitState(_entity));
    }
}

public class HitState : EntityState
{
    private float _recoveryTime = 0.5f;
    private float _timer = 0f;

    public HitState(Entity entity) : base(entity) { }

    public override void Enter()
    {
        base.Enter();

        // 플레이어인 경우만 히트 효과 적용
        if (_entity is PlayerEntity player)
        {
            // 데미지 계산
            float maxHealth = player.GetEntityStat(EntityStatName.MaxHP);
            float currentHealth = player.GetEntityStat(EntityStatName.HP);
            float damageAmount = maxHealth - currentHealth;

            // 히트 효과 UI 요청
            var hitEffectUI = Utils.GetUI<PlayerHitEffectUI>();
            if (hitEffectUI != null)
            {
                hitEffectUI.PlayHitEffect(damageAmount);
            }
            else
            {
                Debug.LogWarning("HitState: PlayerHitEffectUI를 찾을 수 없습니다.");
            }

            // 플레이어 UI 업데이트
            player.NotifyObservers();
        }

        _timer = 0f;
    }

    public override void Execute()
    {
        _timer += Time.deltaTime;

        // 일정 시간 후 원래 상태로 복귀
        if (_timer >= _recoveryTime)
        {
            _entity.SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}