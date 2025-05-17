using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : SerializedMonoBehaviour
{
    private EntityComponentHandler _components;
    private EntityAnimatorHandler _animatorHandler;
    private EntityHealthHandler _healthHandler;
    private EntityCombatHandler _combatHandler;
    private EntityMovementHandler _movementHandler;
    private EntityState _entityState;
    private IEntityController _controller;
    private Animator _animator;

    private Action onDestoryAction;
    private Action onHitAction;

    private Dictionary<string, object> decisionContext = new();

    public EntityStat Stats { get; protected set; }
    public Vector3 CurrentDir { get; protected set; }
    public Entity CurrentTarget { get; private set; }
    public bool CanWalk => (Mathf.Abs(CurrentDir.x) > 0.1f || Mathf.Abs(CurrentDir.z) > 0.1f) && GetState().GetType() != typeof(MoveState);
    public bool CanAttack => GetState().GetType() != typeof(AttackState);

    public virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _animatorHandler = new EntityAnimatorHandler(_animator);
        _components = new EntityComponentHandler(this);
        _healthHandler = new EntityHealthHandler(this, OnHit, OnDeath);
        _combatHandler = new EntityCombatHandler(this);
        _movementHandler = new EntityMovementHandler(this);
        _entityState = new IdleState(this, _animator);
        Stats = EntityStat.CreatePlayerData();
    }

    public void SetController(IEntityController controller) => _controller = controller;

    public void AddEntityComponent<T>(T component) where T : IEntityComponent => _components.Add(component);

    public T GetEntityComponent<T>() where T : class, IEntityComponent => _components.Get<T>();

    public bool HasEntityComponent<T>() where T : class, IEntityComponent => _components.Has<T>();

    public void RemoveEntityComponent<T>() where T : class, IEntityComponent => _components.Remove<T>();

    public virtual float GetEntityStat(EntityStatName stat)
    {
        // 플레이어 엔티티인 경우 에너지는 PlayerData에서 가져오기
        if (stat == EntityStatName.Eng && this is PlayerEntity)
        {
            return BattleFlowController.Instance?.playerData?.energy ?? 0f;
        }
        return Stats?.GetStat(stat) ?? 0f;
    }

    public virtual void SetEntityBaseStat(EntityStatName stat, float val)
    {
        // 플레이어 엔티티인 경우 에너지는 PlayerData에 설정
        if (stat == EntityStatName.Eng && this is PlayerEntity)
        {
            if (BattleFlowController.Instance?.playerData != null)
            {
                BattleFlowController.Instance.playerData.SetEnergy(val);
            }
            return;
        }
        Stats?.SetBaseStat(stat, val);
    }

    public virtual void UpdateEntityBaseStat(EntityStatName stat, float delta)
    {
        // 플레이어 엔티티인 경우 에너지는 PlayerData에서 업데이트
        if (stat == EntityStatName.Eng && this is PlayerEntity)
        {
            if (BattleFlowController.Instance?.playerData != null)
            {
                BattleFlowController.Instance.playerData.UpdateEnergy(delta);
            }
            return;
        }
        Stats?.UpdateBaseStat(stat, delta);
    }

    public virtual void AddEntityStatModifier(EntityStatName stat, object source, float val)
    {
        // 플레이어 엔티티인 경우 에너지는 PlayerData에서 관리
        if (stat == EntityStatName.Eng && this is PlayerEntity)
        {
            if (BattleFlowController.Instance?.playerData != null)
            {
                BattleFlowController.Instance.playerData.UpdateEnergy(val);
            }
            return;
        }
        Stats?.UpdateStat(stat, source, val);
    }

    public virtual void SetEntityStatModifier(EntityStatName stat, object source, float val)
    {
        // 플레이어 엔티티인 경우 에너지는 PlayerData에서 관리
        if (stat == EntityStatName.Eng && this is PlayerEntity)
        {
            if (BattleFlowController.Instance?.playerData != null)
            {
                BattleFlowController.Instance.playerData.SetEnergy(val);
            }
            return;
        }
        Stats?.ChangeStat(stat, source, val);
    }

    public void SetAnimatorValue<T>(T param, object val) where T : Enum => _animatorHandler.SetAnimatorValue(param, val);

    public TResult GetAnimatorValue<T, TResult>(T param) where T : Enum => _animatorHandler.GetAnimatorValue<T, TResult>(param);

    public void SetTarget(Entity target) => CurrentTarget = target;

    public void ClearTarget() => CurrentTarget = null;

    public virtual void TakeDamage(float dmg)
    {
        _healthHandler?.TakeDamage(dmg);
        SetAnimatorValue(EntityAnimTrigger.HitTrigger, null);
    }

    public void Heal(float amount) => _healthHandler?.Heal(amount);

    public void Move() => _movementHandler?.Move(CurrentDir);

    public void OnAttackHit() => GetEntityComponent<AttackComponent>()?.DoHit();

    public void OnAttackAnime() => SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Attack);

    public void SetOnHitAction(Action action) => onHitAction += action;

    public abstract void OnHit(int dmg);

    public abstract void OnDeath();

    public void ChangePlayerState(EntityState newState)
    {
        newState?.Exit();
        _entityState = newState;
        newState.Enter();
    }

    public Type GetCharacterStateType() => _entityState.GetType();

    public EntityState GetState() => _entityState;

    public void SetDir(Vector3 dir) { CurrentDir = dir; }

    public void SetData(string key, object value) => decisionContext[key] = value;

    public bool TryGetData<T>(string key, out T value)
    {
        if (decisionContext.TryGetValue(key, out var obj) && obj is T casted)
        {
            value = casted;
            return true;
        }
        value = default;
        return false;
    }

    public T GetData<T>(string key)
    {
        if (TryGetData<T>(key, out T result)) return result;

        Debug.LogWarning($"[BehaviorSequence] Key '{key}' not found or wrong type.");
        return default;
    }

    public bool RemoveData(string key)
    {
        return decisionContext.Remove(key);
    }

    protected virtual void Update()
    {
        _controller?.Update(this);
        _components?.UpdateAll();
        _entityState?.Execute();
        if (CanWalk)
            SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Move);
    }

    private void OnDestroy() => _components.ExitAll();
}