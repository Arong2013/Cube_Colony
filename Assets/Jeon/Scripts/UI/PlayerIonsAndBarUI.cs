using UnityEngine;
using UnityEngine.UI;

public class PlayerIonsAndBar : MonoBehaviour, IObserver, IPlayerUesableUI
{
    private PlayerEntity player;

    [SerializeField] private Slider HpBar, O2Bar, EngBar;

    private float currentHp, targetHp;
    private float currentO2, targetO2;
    private float currentEng, targetEng;

    public void Initialize(PlayerEntity player)
    {
        this.player = player;
        player.RegisterObserver(this);

        currentHp = targetHp = player.GetEntityStat(EntityStatName.HP);
        currentO2 = targetO2 = player.GetEntityStat(EntityStatName.O2);
        currentEng = targetEng = player.GetEntityStat(EntityStatName.Eng);

        UpdateBarsInstantly();

    }

    public void UpdateObserver()
    {
        targetHp = player.GetEntityStat(EntityStatName.HP);
        currentHp = targetHp;

        targetO2 = player.GetEntityStat(EntityStatName.O2);
        currentO2 = targetO2;

        targetEng = player.GetEntityStat(EntityStatName.Eng);
        currentEng = targetEng;

        UpdateBarsInstantly();
    }

    private void OnDestroy()
    {
        if (player != null)
            player.UnregisterObserver(this);
    }

    private void UpdateBarsInstantly()
    {
        UpdateBarValues();
    }

    private void UpdateBarValues()
    {
        HpBar.value = currentHp / player.GetEntityStat(EntityStatName.MaxHP);
        O2Bar.value = currentO2 / player.GetEntityStat(EntityStatName.MaxO2);
        EngBar.value = currentEng / player.GetEntityStat(EntityStatName.MaxEng);
    }
}
