using UnityEngine;

public class AttackComponent : IEntityComponent
{
    private Entity _entity;
    private float _attackRange = 3f;
    public AttackComponent(float attackRange)
    {
        _attackRange = attackRange;
    }   
    public void Start(Entity entity) => _entity = entity;
    public void Update(Entity entity) { }
    public void Exit(Entity entity) { }
    public void Attack(Entity target)
    {
        if (target == null) return;

        _entity.SetTarget(target);

        // 회전 처리
        FaceTarget(target.transform.position);
        _entity.SetAnimatorValue(EntityAnimInt.ActionType, (int)EntityActionType.Attack);
    }
    public void DoHit()
    {
        var target = _entity.CurrentTarget; 
        if (target == null || !HasValidTarget(_attackRange)) return;
        float atk = _entity.GetEntityStat(EntityStatName.ATK);
        target.TakeDamage(atk);

        Debug.Log($"[{_entity.name}] hit {target.name} for {atk} damage");
        _entity.ClearTarget(); 
    }
    public bool HasValidTarget(float maxDistance)
    {
        if (_entity.CurrentTarget == null) return false;

        float dist = Vector3.Distance(_entity.transform.position, _entity.CurrentTarget.transform.position);
        return dist <= maxDistance;
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        Vector3 dir = targetPosition - _entity.transform.position;

        if (Mathf.Abs(dir.x) > 0.01f)
        {
            Vector3 scale = _entity.transform.localScale;
            scale.x = dir.x > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            _entity.transform.localScale = scale;
        }
    }
}