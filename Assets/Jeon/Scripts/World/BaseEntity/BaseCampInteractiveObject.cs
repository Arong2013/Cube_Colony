using UnityEngine;
using Sirenix.OdinInspector;

public class BaseCampInteractiveObject : SerializedMonoBehaviour
{
    public enum InteractionType
    {
        HealthRecovery,
        EnergyRecovery,
        OxygenRecovery,
        FullRecovery
    }

    [TitleGroup("상호작용 설정")]
    [LabelText("상호작용 타입")]
    [SerializeField] private InteractionType interactionType = InteractionType.FullRecovery;

    [TitleGroup("상호작용 설정")]
    [LabelText("회복량")]
    [SerializeField] private float recoveryAmount = 50f;

    [TitleGroup("효과음")]
    [LabelText("상호작용 효과음")]
    [SerializeField] private AudioClip interactionSound;

    [TitleGroup("이펙트")]
    [LabelText("상호작용 이펙트")]
    [SerializeField] private GameObject interactionEffect;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public void OnMouseDown()
    {
        PlayerEntity player = Utils.GetPlayer();
        if (player != null)
        {
            bool interactionSuccessful = false;

            switch (interactionType)
            {
                case InteractionType.HealthRecovery:
                    interactionSuccessful = RecoverHealth(player);
                    Utils.GetUI<InBaseCampUI>().playerHealEffectUI.PlayHitFixedEffect();
                    break;
                case InteractionType.EnergyRecovery:
                    interactionSuccessful = RecoverEnergy(player);
                    break;
                case InteractionType.OxygenRecovery:
                    interactionSuccessful = RecoverOxygen(player);
                    break;
                case InteractionType.FullRecovery:
                    interactionSuccessful = RecoverAll(player);
                    break;
            }

            if (interactionSuccessful)
            {
                PlayInteractionEffect();
                PlayInteractionSound();
            }
        }
    }

    private bool RecoverHealth(PlayerEntity player)
    {
        float currentHP = player.GetEntityStat(EntityStatName.HP);
        float maxHP = player.GetEntityStat(EntityStatName.MaxHP);

        if (currentHP >= maxHP)
            return false;

        player.Heal(recoveryAmount);
        Debug.Log($"체력 {recoveryAmount} 회복");
        return true;
    }

    private bool RecoverEnergy(PlayerEntity player)
    {
        var playerData = BattleFlowController.Instance?.playerData;
        if (playerData == null)
            return false;

        if (playerData.energy >= playerData.maxEnergy)
            return false;

        playerData.UpdateEnergy(recoveryAmount);
        Debug.Log($"에너지 {recoveryAmount} 회복");
        player.NotifyObservers();
        return true;
    }

    private bool RecoverOxygen(PlayerEntity player)
    {
        float currentO2 = player.GetEntityStat(EntityStatName.O2);
        float maxO2 = player.GetEntityStat(EntityStatName.MaxO2);

        if (currentO2 >= maxO2)
            return false;

        // 산소 회복은 EntityStat을 통해 직접 업데이트
        player.SetEntityBaseStat(EntityStatName.O2, Mathf.Min(currentO2 + recoveryAmount, maxO2));
        Debug.Log($"산소 {recoveryAmount} 회복");
        player.NotifyObservers();
        return true;
    }

    private bool RecoverAll(PlayerEntity player)
    {
        bool healthRecovered = RecoverHealth(player);
        bool energyRecovered = RecoverEnergy(player);
        bool oxygenRecovered = RecoverOxygen(player);

        return healthRecovered || energyRecovered || oxygenRecovered;
    }

    private void PlayInteractionSound()
    {
        if (interactionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
    }

    private void PlayInteractionEffect()
    {
        if (interactionEffect != null)
        {
            Instantiate(interactionEffect, transform.position, Quaternion.identity);
        }
    }

    [Button("상호작용 테스트")]
    private void TestInteraction()
    {
        PlayerEntity player = Utils.GetPlayer();
        if (player != null)
        {
            OnMouseDown();
        }
        else
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다.");
        }
    }
}