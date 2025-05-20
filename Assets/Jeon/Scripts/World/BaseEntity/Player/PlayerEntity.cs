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

        // PlayerData와 동기화
        if (BattleFlowController.Instance != null)
        {
            Stats = BattleFlowController.Instance.playerData.playerStat;
        }
    }

    private void OnPlayerDamaged(int dmg)
    {
        Debug.Log($"[UI] Player took {dmg} damage!");
        NotifyObservers();
    }

    private void OnPlayerDeath()
    {
        Debug.Log("[UI] Player died!");
        gameOverAction?.Invoke();
    }

    private void OnMoveInput(Vector3 direction) => SetDir(direction);

    public void Initialize()
    {
        AddEntityComponent(new AttackComponent(1f));
        AddEntityComponent(new ReturnComponent());
        AddEntityComponent(new ChopComponent());

        // PlayerData와 동기화
        if (BattleFlowController.Instance != null)
        {
            Stats = BattleFlowController.Instance.playerData.playerStat;
        }
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

        // BattleFlowController의 옵저버도 업데이트
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.NotifyObservers();
        }
    }

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        NotifyObservers();
    }

    public void DamageO2()
    {
        if (BattleFlowController.Instance != null &&
            BattleFlowController.Instance.playerData != null)
        {
            BattleFlowController.Instance.playerData.playerStat.UpdateStat(EntityStatName.O2, this, -Time.deltaTime);
            NotifyObservers();
        }
    }

    public override void OnHit(int dmg)
    {
        NotifyObservers();
    }

    public override void OnDeath()
    {
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.playerData.Reset();
        }

        gameOverAction?.Invoke();
    }

    public void SetScurivalAction(Action returnAction, Action gameOverAction)
    {
        this.returnAction = returnAction;
        this.gameOverAction = gameOverAction;
    }

    public void SeReturnStageState() => returnAction?.Invoke();

    // 아이템 관련 메서드
    public bool AddItem(Item item)
    {
        if (BattleFlowController.Instance != null &&
            BattleFlowController.Instance.playerData != null)
        {
            BattleFlowController.Instance.playerData.AddItem(item);
            NotifyObservers();
            return true;
        }
        return false;
    }

    // 인벤토리 접근 메서드
    public List<Item> GetInventoryItems()
    {
        if (BattleFlowController.Instance != null &&
            BattleFlowController.Instance.playerData != null)
        {
            return BattleFlowController.Instance.playerData.inventory;
        }
        return new List<Item>();
    }


}