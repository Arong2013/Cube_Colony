using System;
using UnityEngine;

public  abstract class Entity : MonoBehaviour
{
    private EntityComponentHandler _components;
    private EntityAnimatorHandler _animatorHandler;
    private EntityHealthHandler _healthHandler;
    private EntityCombatHandler _combatHandler;
    private EntityMovementHandler _movementHandler; 
    private EntityState _entityState;
    private IEntityController _controller;
    private Animator _animator;
    public EntityStat Stats { get; private set; }
    public Vector3 CurrentDir { get; protected set; }
    public Entity CurrentTarget { get; private set; }
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _animatorHandler = new EntityAnimatorHandler(_animator);
        _components = new EntityComponentHandler(this);
        _healthHandler = new EntityHealthHandler(this);
        _combatHandler = new EntityCombatHandler(this);
        _movementHandler = new EntityMovementHandler(this);
        _entityState = new IdleState(this, _animator); 
        Stats = EntityStat.CreatPlayerData(); 
    }
    public abstract void Initialize(); // Initialize the entity
    public void SetController(IEntityController controller) => _controller = controller;
    public void AddEntityComponent<T>(T component) where T : IEntityComponent => _components.Add(component);
    public T GetEntityComponent<T>() where T : class, IEntityComponent => _components.Get<T>();
    public bool HasEntityComponent<T>() where T : class, IEntityComponent => _components.Has<T>();
    public void RemoveEntityComponent<T>() where T : class, IEntityComponent => _components.Remove<T>();
    public float GetEntityStat(EntityStatName stat) => Stats?.GetStat(stat) ?? 0f;
    public void SetEntityBaseStat(EntityStatName stat, float val) => Stats?.SetBaseStat(stat, val);
    public void UpdateEntityBaseStat(EntityStatName stat, float delta) => Stats?.UpdateBaseStat(stat, delta);
    public void AddEntityStatModifier(EntityStatName stat, object source, float val) => Stats?.UpdateStat(stat, source, val);
    public void SetEntityStatModifier(EntityStatName stat, object source, float val) => Stats?.ChangeStat(stat, source, val);
    public void SetAnimatorValue<T>(T param, object val) where T : Enum => _animatorHandler.SetAnimatorValue(param, val);
    public TResult GetAnimatorValue<T, TResult>(T param) where T : Enum => _animatorHandler.GetAnimatorValue<T, TResult>(param);
    public void SetTarget(Entity target) => CurrentTarget = target;
    public void ClearTarget() => CurrentTarget = null;

    public void TakeDamage(float dmg) => _healthHandler?.TakeDamage(dmg);
    public void Heal(float amount) => _healthHandler?.Heal(amount); 
    public void Move() => _movementHandler?.Move(CurrentDir);
    public void OnAttackHit() => GetEntityComponent<AttackComponent>()?.DoHit();
    public void ChangePlayerState(EntityState newState)
    {
        newState?.Exit();
        _entityState = newState;
        newState.Enter();
    }
    public Type GetCharacterStateType() => _entityState.GetType();
    public EntityState GetState() => _entityState;
    public void SetDir(Vector3 dir) { CurrentDir = dir; }

    protected virtual void Update()
    {
        _controller?.Update(this);
        _components.UpdateAll();
        _entityState?.Execute(); 
    }
    private void OnDestroy() => _components.ExitAll();
}
