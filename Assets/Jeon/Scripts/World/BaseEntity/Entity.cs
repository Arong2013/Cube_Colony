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

    // 디버그용 데미지 설정
    [TitleGroup("🛠️ 디버그 테스트")]
    [LabelText("테스트 데미지 양"), Range(1, 100)]
    [SerializeField] private float debugDamageAmount = 20f;

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

    /// <summary>
    /// 엔티티 스탯에 모디파이어 추가
    /// </summary>
    public void AddEntityStatModifier(EntityStatName statName, object source, float value)
    {
        // Stats 프로퍼티 사용
        if (Stats != null)
        {
            Stats.UpdateStat(statName, source, value);
        }
        else
        {
            Debug.LogWarning("엔티티 스탯이 초기화되지 않았습니다.");
        }
    }

    /// <summary>
    /// 엔티티 스탯 모디파이어 설정 (이전 값 덮어쓰기)
    /// </summary>
    public void SetEntityStatModifier(EntityStatName statName, object source, float value)
    {
        // Stats 프로퍼티 사용
        if (Stats != null)
        {
            Stats.ChangeStat(statName, source, value);
        }
        else
        {
            Debug.LogWarning("엔티티 스탯이 초기화되지 않았습니다.");
        }
    }

    /// <summary>
    /// 엔티티 기본 스탯 설정 (절대값 설정)
    /// </summary>
    public void SetEntityBaseStat(EntityStatName statName, float value)
    {
        if (Stats != null)
        {
            Stats.SetBaseStat(statName, value);
        }
        else
        {
            Debug.LogWarning("엔티티 스탯이 초기화되지 않았습니다.");
        }
    }

    /// <summary>
    /// 엔티티 스탯 가져오기 (최종 계산값)
    /// </summary>
    public virtual float GetEntityStat(EntityStatName stat)
    {
        // Stats 프로퍼티 사용
        if (Stats != null)
        {
            return Stats.GetStat(stat);
        }
        else
        {
            Debug.LogWarning("엔티티 스탯이 초기화되지 않았습니다.");
            return 0f;
        }
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
        _entityState?.Exit();
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

    // ===== 디버그 테스트 버튼들 =====

    [TitleGroup("🛠️ 디버그 테스트")]
    [Button("💔 데미지 받기", ButtonSizes.Medium), GUIColor(0.9f, 0.3f, 0.3f)]
    public void DebugTakeDamage()
    {
        float currentHP = GetEntityStat(EntityStatName.HP);
        Debug.Log($"[디버그] {name}이(가) {debugDamageAmount} 데미지를 받습니다. (현재 HP: {currentHP})");
        
        TakeDamage(debugDamageAmount);
        
        float newHP = GetEntityStat(EntityStatName.HP);
        Debug.Log($"[디버그] 데미지 후 HP: {newHP}");
    }

    [TitleGroup("🛠️ 디버그 테스트")]
    [Button("💚 체력 회복", ButtonSizes.Medium), GUIColor(0.3f, 0.9f, 0.3f)]
    public void DebugHeal()
    {
        float healAmount = 30f;
        float currentHP = GetEntityStat(EntityStatName.HP);
        Debug.Log($"[디버그] {name}이(가) {healAmount} 체력을 회복합니다. (현재 HP: {currentHP})");
        
        Heal(healAmount);
        
        float newHP = GetEntityStat(EntityStatName.HP);
        Debug.Log($"[디버그] 회복 후 HP: {newHP}");
    }

    [TitleGroup("🛠️ 디버그 테스트")]
    [Button("☠️ 즉사 데미지", ButtonSizes.Medium), GUIColor(0.7f, 0.1f, 0.1f)]
    public void DebugInstantKill()
    {
        float maxHP = GetEntityStat(EntityStatName.MaxHP);
        Debug.Log($"[디버그] {name}에게 즉사 데미지 ({maxHP * 2}) 를 줍니다!");
        
        TakeDamage(maxHP * 2); // 최대 체력의 2배 데미지
    }

    [TitleGroup("🛠️ 디버그 테스트")]
    [Button("📊 현재 상태 출력", ButtonSizes.Medium), GUIColor(0.3f, 0.3f, 0.9f)]
    public void DebugPrintStatus()
    {
        Debug.Log($"=== {name} 상태 정보 ===");
        Debug.Log($"체력: {GetEntityStat(EntityStatName.HP)} / {GetEntityStat(EntityStatName.MaxHP)}");
        Debug.Log($"공격력: {GetEntityStat(EntityStatName.ATK)}");
        Debug.Log($"방어력: {GetEntityStat(EntityStatName.DEF)}");
        Debug.Log($"속도: {GetEntityStat(EntityStatName.SPD)}");
        Debug.Log($"산소: {GetEntityStat(EntityStatName.O2)} / {GetEntityStat(EntityStatName.MaxO2)}");
        Debug.Log($"현재 상태: {GetState().GetType().Name}");
        Debug.Log($"현재 방향: {CurrentDir}");
        Debug.Log($"타겟: {(CurrentTarget != null ? CurrentTarget.name : "없음")}");
    }

    [TitleGroup("🛠️ 디버그 테스트")]
    [Button("🎯 가장 가까운 적 공격", ButtonSizes.Medium), GUIColor(0.9f, 0.6f, 0.2f)]
    public void DebugAttackNearestEnemy()
    {
        // 주변의 모든 엔티티 찾기
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);
        Entity nearestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            Entity entity = collider.GetComponent<Entity>();
            if (entity != null && entity != this && entity.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, entity.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestEnemy = entity;
                }
            }
        }

        if (nearestEnemy != null)
        {
            Debug.Log($"[디버그] {name}이(가) {nearestEnemy.name}을(를) 공격합니다! (거리: {closestDistance:F1})");
            OnAttackAnime();
            nearestEnemy.TakeDamage(GetEntityStat(EntityStatName.ATK));
        }
        else
        {
            Debug.Log($"[디버그] {name} 주변에 공격할 적이 없습니다.");
        }
    }

    [TitleGroup("🛠️ 디버그 테스트")]
    [Button("🔄 완전 회복", ButtonSizes.Medium), GUIColor(0.2f, 0.8f, 0.2f)]
    public void DebugFullRestore()
    {
        float maxHP = GetEntityStat(EntityStatName.MaxHP);
        float maxO2 = GetEntityStat(EntityStatName.MaxO2);
        
        Debug.Log($"[디버그] {name}을(를) 완전히 회복합니다!");
        
        // 체력과 산소를 최대치로 설정
        SetEntityStatModifier(EntityStatName.HP, this, maxHP);
        SetEntityStatModifier(EntityStatName.O2, this, maxO2);
        
        // 플레이어라면 에너지도 회복
        if (this is PlayerEntity && BattleFlowController.Instance?.playerData != null)
        {
            BattleFlowController.Instance.playerData.SetEnergy(BattleFlowController.Instance.playerData.maxEnergy);
        }
        
        Debug.Log($"[디버그] 완전 회복 완료! HP: {GetEntityStat(EntityStatName.HP)}, O2: {GetEntityStat(EntityStatName.O2)}");
    }
}