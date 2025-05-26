using UnityEngine;

[CreateAssetMenu(menuName = "InteractionStrategy/Heal")]
public class HealStrategyAsset : ScriptableInteractionStrategy
{
    [SerializeField] float hpHealAmount = 30f;
    [SerializeField] float o2HealAmount = 30f;
    [SerializeField] float energyHealAmount = 30f;

    public override IInteractionStrategy CreateStrategy() => 
        new HealStrategy(hpHealAmount, o2HealAmount, energyHealAmount);
}

public class HealStrategy : IInteractionStrategy
{
    private float _hpHealAmount;
    private float _o2HealAmount;
    private float _energyHealAmount;

    public HealStrategy(float hpHealAmount, float o2HealAmount, float energyHealAmount)
    {
        _hpHealAmount = hpHealAmount;
        _o2HealAmount = o2HealAmount;
        _energyHealAmount = energyHealAmount;
    }

    public bool CanInteract(Entity self, Entity interactor) => 
        interactor is PlayerEntity;

    public void Interact(Entity self, Entity interactor)
    {
        if (interactor is PlayerEntity player)
        {
            // HP 회복
            player.Heal(_hpHealAmount);

            // O2 회복
            float currentO2 = player.GetEntityStat(EntityStatName.O2);
            float maxO2 = player.GetEntityStat(EntityStatName.MaxO2);
            float newO2 = Mathf.Min(currentO2 + _o2HealAmount, maxO2);
            player.SetEntityBaseStat(EntityStatName.O2, newO2);

            // 에너지 회복
            if (BattleFlowController.Instance?.playerData != null)
            {
                float currentEnergy = BattleFlowController.Instance.playerData.energy;
                float maxEnergy = BattleFlowController.Instance.playerData.maxEnergy;
                float newEnergy = Mathf.Min(currentEnergy + _energyHealAmount, maxEnergy);
                BattleFlowController.Instance.playerData.SetEnergy(newEnergy);
            }

            // 옵저버에게 상태 변경 알림
            player.NotifyObservers();

            // 자기 자신 파괴
            if (self != null)
            {
                UnityEngine.Object.Destroy(self.gameObject);
            }
        }
    }

    public string GetLabel() => "회복하기";

    public void Initialize(Entity self)
    {
        // 필요한 경우 특별한 초기화 작업 수행
    }
}