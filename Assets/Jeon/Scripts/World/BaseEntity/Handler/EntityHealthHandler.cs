using System;
using UnityEngine;

public class EntityHealthHandler
{
    private readonly Entity _entity;
    Action<int> onDamaged;
    Action onDeath;

    public EntityHealthHandler(Entity entity, Action<int> onDamaged = null, Action onDeath = null)
    {
        _entity = entity;
        this.onDamaged = onDamaged;
        this.onDeath = onDeath;
    }

public void TakeDamage(float amount)
{
    float currentHp = _entity.GetEntityStat(EntityStatName.HP);
    
    float newHp = Mathf.Max(0, currentHp - amount);
    
    Debug.Log($" damage: {amount}. Current HP: {currentHp} new HP: {newHp}");

    _entity.SetEntityBaseStat(EntityStatName.HP, newHp);
    
    _entity.SetAnimatorValue(EntityAnimTrigger.HitTrigger, null);
    onDamaged?.Invoke((int)amount);
    
    // HP가 0이 되면 게임 오버
    if (newHp <= 0)
    {
        Debug.Log($"[EntityHealthHandler] {_entity.name} is DEAD! Calling GameOver...");
        
        // 플레이어인 경우 게임 오버 상태로 전환
        if (_entity is PlayerEntity)
        {
            BattleFlowController.Instance?.SetCompleteState(true);
        }
        
        onDeath?.Invoke();
    }
}
    public void Heal(float amount)
    {
        float currentHp = _entity.GetEntityStat(EntityStatName.HP);
        float maxHp = _entity.GetEntityStat(EntityStatName.MaxHP);
        float newHp = Mathf.Min(currentHp + amount, maxHp);
        
        Debug.Log($"[EntityHealthHandler] {_entity.name} Heal: {amount}, CurrentHP: {currentHp} -> NewHP: {newHp}");
        
        // ⚠️ 수정: SetEntityBaseStat으로 절대값 설정
        _entity.SetEntityBaseStat(EntityStatName.HP, newHp);
    }

    public bool IsDead 
    {
        get
        {
            float hp = _entity.GetEntityStat(EntityStatName.HP);
            bool isDead = hp <= 0;
            Debug.Log($"[EntityHealthHandler] IsDead getter: HP={hp}, isDead={isDead}");
            return isDead;
        }
    }
}