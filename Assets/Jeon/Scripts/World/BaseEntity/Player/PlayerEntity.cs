using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class PlayerEntity : Entity, ISubject
{
    public bool CanWalk => (Mathf.Abs(CurrentDir.x) > 0.1f || Mathf.Abs(CurrentDir.z) > 0.1f) && GetState().GetType() != typeof(MoveState);
    List<IObserver> observers = new List<IObserver>();
    protected override void Awake()
    {
        base.Awake();
        SetController(new PCController(OnMoveInput));
        Initialize();
        LinkUi();
    }
    private void OnPlayerDamaged(int dmg)
    {
        Debug.Log($"[UI] Player took {dmg} damage!");
    }
    private void OnPlayerDeath()
    {
        Debug.Log("[UI] Player died!");
    }
    private void OnMoveInput(Vector3 direction) => SetDir(direction);
    public override void Initialize()
    {
        AddEntityComponent(new AttackComponent());  
        AddEntityComponent(new InventoryComponent());
        AddEntityComponent(new ReturnComponent());
    }
    protected override void Update()
    {
        base.Update();
        DamageO2();
        if (CanWalk)
            SetAnimatorValue(EntityAnimBool.IsMoving, true);
    }
    void LinkUi() => Utils.SetPlayerMarcineOnUI().ForEach(x => x.Initialize(this));
    public void RegisterObserver(IObserver observer) => observers.Add(observer); 
    public void UnregisterObserver(IObserver observer) => observers.Remove(observer);
    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.UpdateObserver();
        }
    }


    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        NotifyObservers();  
    }

    public void DamageO2()
    {
        Stats.UpdateStat(EntityStatName.HP, this,-Time.deltaTime);
        NotifyObservers();  
    }
}   

