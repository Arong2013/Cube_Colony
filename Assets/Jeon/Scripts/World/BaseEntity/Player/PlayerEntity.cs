using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class PlayerEntity : Entity, ISubject
{
    private Action returnAction;
    private Action gameOverAction;

    List<IObserver> observers = new List<IObserver>();
    public override void Init()
    {
        base.Init();
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
    public void Initialize()
    {
        AddEntityComponent(new AttackComponent(1f));  
        AddEntityComponent(new InventoryComponent());
        AddEntityComponent(new ReturnComponent());
        AddEntityComponent(new ChopComponent());
    }
    protected override void Update()
    {
        base.Update();
        DamageO2();

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
    }
    public void DamageO2()
    {
        Stats.UpdateStat(EntityStatName.O2, this,-Time.deltaTime);
        //NotifyObservers();  
    }
    public override void OnHit(int dmg)
    {
        NotifyObservers();
    }
    public override void OnDeath()
    {
        Stats = EntityStat.CreatePlayerData();
        gameOverAction?.Invoke();   
    }
    public void SetScurivalAction(Action returnAction, Action gameOverAction)
    {
        this.returnAction = returnAction;
        this.gameOverAction += gameOverAction;
    }
    public void SeReturnStageState() => returnAction?.Invoke();
}   

