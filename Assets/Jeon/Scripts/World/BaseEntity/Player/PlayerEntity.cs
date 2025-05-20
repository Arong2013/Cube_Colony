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
        // EquipmentComponent에서 보너스 값 가져오기
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            return equipmentComponent.GetTotalEquipmentBonus(stat);
        }

        // PlayerEquipmentHandler도 확인
        var playerEquipmentHandler = GetEntityComponent<PlayerEquipmentHandler>();
        if (playerEquipmentHandler != null)
        {
            // statBonuses 딕셔너리에서 값을 반환하는 메서드 호출
            // PlayerEquipmentHandler에 GetStatBonus 메서드가 있다고 가정
            if (playerEquipmentHandler.GetStatBonuses().TryGetValue(stat, out float bonus))
            {
                return bonus;
            }
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
        // 장비 효과 재적용 - EquipmentComponent 또는 PlayerEquipmentHandler 모두 확인
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            equipmentComponent.RefreshAllEquipmentEffects();
        }

        var playerEquipmentHandler = GetEntityComponent<PlayerEquipmentHandler>();
        if (playerEquipmentHandler != null)
        {
            playerEquipmentHandler.RefreshEquipmentEffects();
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

        // EntityStatName.ATK와 Attack은 동일하므로 어느 것을 사용해도 됩니다
        Debug.Log($"공격력: {GetEntityStat(EntityStatName.ATK)} (기본: {Stats.GetStat(EntityStatName.ATK)}, 장비: +{GetTotalEquipmentBonus(EntityStatName.ATK)})");
        Debug.Log($"방어력: {GetEntityStat(EntityStatName.DEF)} (기본: {Stats.GetStat(EntityStatName.DEF)}, 장비: +{GetTotalEquipmentBonus(EntityStatName.DEF)})");
        Debug.Log($"속도: {GetEntityStat(EntityStatName.SPD)}");

        // 산소 및 에너지 정보 추가
        Debug.Log($"산소: {GetEntityStat(EntityStatName.O2)}/{GetEntityStat(EntityStatName.MaxO2)}");
        Debug.Log($"에너지: {GetEntityStat(EntityStatName.Eng)}/{GetEntityStat(EntityStatName.MaxEng)}");

        // 장비 정보 출력
        var equipmentComponent = GetEntityComponent<EquipmentComponent>();
        if (equipmentComponent != null)
        {
            equipmentComponent.PrintEquipmentStatus();
        }

        var playerEquipmentHandler = GetEntityComponent<PlayerEquipmentHandler>();
        if (playerEquipmentHandler != null)
        {
            Debug.Log("=== 장비 효과 ===");
            var effects = playerEquipmentHandler.GetTotalEffects();
            if (effects.attackBonus > 0) Debug.Log($"공격력 보너스: +{effects.attackBonus}");
            if (effects.defenseBonus > 0) Debug.Log($"방어력 보너스: +{effects.defenseBonus}");
            if (effects.healthBonus > 0) Debug.Log($"체력 보너스: +{effects.healthBonus}");
            if (effects.maxOxygenBonus > 0) Debug.Log($"최대 산소 보너스: +{effects.maxOxygenBonus}");
            if (effects.maxEnergyBonus > 0) Debug.Log($"최대 에너지 보너스: +{effects.maxEnergyBonus}");

            // 특수 효과 출력
            if (effects.extraHitCount > 0) Debug.Log($"추가 타격: +{effects.extraHitCount}회");
            if (effects.fireRateBonus > 0) Debug.Log($"연사 속도: +{effects.fireRateBonus * 100}%");
            if (effects.oxygenConsumptionReduction > 0) Debug.Log($"산소 소모 감소: {effects.oxygenConsumptionReduction * 100}%");
            if (effects.energyConsumptionReduction > 0) Debug.Log($"에너지 소모 감소: {effects.energyConsumptionReduction * 100}%");
            if (effects.inventorySlotBonus > 0) Debug.Log($"인벤토리 슬롯: +{effects.inventorySlotBonus}칸");
            if (effects.damageReduction > 0) Debug.Log($"피해 감소: {effects.damageReduction * 100}%");
        }
    }

}