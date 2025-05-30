using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;
public class PlayerEntity : Entity, ISubject
{
    [SerializeField] float baseDamageAmount;
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
        AddEntityComponent(new AttackComponent(2f));
        AddEntityComponent(new ReturnComponent());
        AddEntityComponent(new ChopComponent());

        // PlayerData와 동기화
        if (BattleFlowController.Instance != null)
        {
            Stats = BattleFlowController.Instance.playerData.playerStat;
        }

        Debug.Log("PlayerEntity 초기화 완료");
    }

    protected override void Update()
    {
        base.Update();
        DamageO2();
    }

    public override void OnAttackAnime()
    {if (Camera.main == null) return;

    Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
    bool isRightSide = Input.mousePosition.x > playerScreenPos.x;

    // 공격 방향 애니메이터 파라미터 설정
    SetAnimatorValue(EntityAnimBool.AttackRight, isRightSide);

        base.OnAttackAnime();
            // 마우스 위치에 따른 공격 방향 결정
    
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
        float currentO2 = GetEntityStat(EntityStatName.O2);
        

        // 장착된 장비의 산소 소모 감소 효과 계산
        float reductionMultiplier = 1f;
        var equippedItems = GetAllEquippedItems();
        foreach (var item in equippedItems)
        {
            var effects = item.GetCurrentEffects();
            reductionMultiplier -= effects.oxygenConsumptionReduction;
        }
        Debug.Log($"장착된 장비로 인한 산소 소모 감소: {Time.deltaTime}");
        // 최소 감소량 0.1f로 제한
        float finalDamageAmount = Mathf.Max(baseDamageAmount - reductionMultiplier, 0.1f);

        BattleFlowController.Instance.playerData.playerStat.UpdateStat(EntityStatName.O2, this, -baseDamageAmount);

        // O2가 0이 되면 게임 오버
        if (currentO2 <= 0)
        {
            // 게임 오버 상태로 전환
            if (BattleFlowController.Instance != null)
            {
                BattleFlowController.Instance.SetCompleteState(true);
            }
        }

        NotifyObservers();
    }
}

    public override void OnHit(int dmg)
    {
        NotifyObservers();
    }

    public override void OnDeath()
    {
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

    // 장비 관련 메서드
    public bool EquipItem(EquipableItem item)
    {
        if (BattleFlowController.Instance?.playerData != null)
        {
            bool result = BattleFlowController.Instance.playerData.EquipItem(item);
            NotifyObservers();
            return result;
        }
        return false;
    }

    public EquipableItem UnequipItem(EquipmentType type)
    {
        if (BattleFlowController.Instance?.playerData != null)
        {
            var unequippedItem = BattleFlowController.Instance.playerData.UnequipItem(type);
            NotifyObservers();
            return unequippedItem;
        }
        return null;
    }

    public EquipableItem GetEquippedItem(EquipmentType type)
    {
        return BattleFlowController.Instance?.playerData?.GetEquippedItem(type);
    }

    public List<EquipableItem> GetAllEquippedItems()
    {
        var equippedItems = BattleFlowController.Instance?.playerData?.GetAllEquippedItems();
        return equippedItems?.Values.ToList() ?? new List<EquipableItem>();
    }

    // 총 장비 보너스 가져오기
    public float GetTotalEquipmentBonus(EntityStatName stat)
    {
        // 장비로 인한 총 보너스 계산 로직
        float totalBonus = 0f;
        var equippedItems = GetAllEquippedItems();

        foreach (var item in equippedItems)
        {
            var effects = item.GetCurrentEffects();
            switch (stat)
            {
                case EntityStatName.ATK:
                    totalBonus += effects.attackBonus;
                    break;
                case EntityStatName.DEF:
                    totalBonus += effects.defenseBonus;
                    break;
                case EntityStatName.MaxHP:
                    totalBonus += effects.healthBonus;
                    break;
                case EntityStatName.MaxO2:
                    totalBonus += effects.maxOxygenBonus;
                    break;
                case EntityStatName.MaxEng:
                    totalBonus += effects.maxEnergyBonus;
                    break;
            }
        }

        return totalBonus;
    }

    // 장비 효과를 포함한 최종 스탯 가져오기 (오버라이드)
    public override float GetEntityStat(EntityStatName stat)
    {
        float baseStat = base.GetEntityStat(stat);
        float equipmentBonus = GetTotalEquipmentBonus(stat);
        return baseStat + equipmentBonus;
    }

    [Button("장착된 장비 출력")]
    public void PrintEquippedItems()
    {
        var equippedItems = GetAllEquippedItems();
        foreach (var item in equippedItems)
        {
            Debug.Log($"장착된 장비: {item.ItemName} (타입: {item.equipmentType}, 강화 레벨: +{item.currentReinforcementLevel})");
        }
    }
}