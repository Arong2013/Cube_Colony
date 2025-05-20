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
        // 기존 컴포넌트들
        AddEntityComponent(new AttackComponent(1f));
        AddEntityComponent(new ReturnComponent());
        AddEntityComponent(new ChopComponent());

        // 새로운 장비 컴포넌트 추가
        AddEntityComponent(new EquipmentComponent());

        // PlayerData와 동기화
        if (BattleFlowController.Instance != null)
        {
            Stats = BattleFlowController.Instance.playerData.playerStat;
        }

        Debug.Log("PlayerEntity 초기화 완료 - 장비 시스템 포함");
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
        // 장비 시스템 초기화
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            // 모든 장착된 아이템을 인벤토리로 반환
            var equippedItems = equipmentComponent.GetAllEquippedItems();
            foreach (var item in equippedItems)
            {
                AddItem(item);
            }
        }

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

    /// <summary>
    /// 장비 장착
    /// </summary>
    public bool EquipItem(EquipableItem item)
    {
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            return equipmentComponent.EquipItem(item);
        }
        return false;
    }

    /// <summary>
    /// 장비 해제
    /// </summary>
    public EquipableItem UnequipItem(EquipmentType type)
    {
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            return equipmentComponent.UnequipItem(type);
        }
        return null;
    }

    /// <summary>
    /// 특정 타입의 장착된 아이템 가져오기
    /// </summary>
    public EquipableItem GetEquippedItem(EquipmentType type)
    {
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            return equipmentComponent.GetEquippedItem(type);
        }
        return null;
    }

    /// <summary>
    /// 모든 장착된 아이템 가져오기
    /// </summary>
    public List<EquipableItem> GetAllEquippedItems()
    {
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            return equipmentComponent.GetAllEquippedItems();
        }
        return new List<EquipableItem>();
    }

    /// <summary>
    /// 장비로 인한 총 스탯 보너스 가져오기
    /// </summary>
    public float GetTotalEquipmentBonus(EntityStatName stat)
    {
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            return equipmentComponent.GetTotalEquipmentBonus(stat);
        }
        return 0f;
    }

    /// <summary>
    /// 장비 효과를 포함한 최종 스탯 가져오기 (오버라이드)
    /// </summary>
    public override float GetEntityStat(EntityStatName stat)
    {
        float baseStat = base.GetEntityStat(stat);
        float equipmentBonus = GetTotalEquipmentBonus(stat);
        return baseStat + equipmentBonus;
    }

    /// <summary>
    /// 장비 상태 정보를 UI에 표시
    /// </summary>
    public void ShowEquipmentStatus()
    {
        var inventoryUI = Utils.GetUI<InventoryUI>();
        if (inventoryUI != null)
        {
            inventoryUI.OpenInventoryUI();
        }
    }

    /// <summary>
    /// 장비 강화 완료 후 호출되는 콜백
    /// </summary>
    public void OnEquipmentReinforced(EquipableItem item)
    {
        // 장비 효과 재적용
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            equipmentComponent.RefreshAllEquipmentEffects();
        }

        NotifyObservers();
        Debug.Log($"{item.GetDisplayName()} 강화 완료! 효과가 재적용되었습니다.");
    }

    /// <summary>
    /// 디버깅용: 현재 플레이어 상태 출력
    /// </summary>
    public void PrintPlayerStatus()
    {
        Debug.Log("=== 플레이어 상태 ===");
        Debug.Log($"레벨: {Stats.Level}");
        Debug.Log($"체력: {GetEntityStat(EntityStatName.HP)}/{GetEntityStat(EntityStatName.MaxHP)}");
        Debug.Log($"공격력: {GetEntityStat(EntityStatName.ATK)} (기본: {Stats.GetStat(EntityStatName.ATK)}, 장비: +{GetTotalEquipmentBonus(EntityStatName.ATK)})");
        Debug.Log($"방어력: {GetEntityStat(EntityStatName.DEF)} (기본: {Stats.GetStat(EntityStatName.DEF)}, 장비: +{GetTotalEquipmentBonus(EntityStatName.DEF)})");
        Debug.Log($"속도: {GetEntityStat(EntityStatName.SPD)}");

        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            equipmentComponent.PrintEquipmentStatus();
        }
    }
}