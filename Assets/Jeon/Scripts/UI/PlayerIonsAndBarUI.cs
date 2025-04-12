using UnityEngine;
using UnityEngine.UI;

public class PlayerIonsAndBar : MonoBehaviour, IObserver, IPlayerUesableUI
{
    private PlayerEntity player;

    [SerializeField] private Slider HpBar;
    private float currentHp;
    private float targetHp;
    public void Initialize(PlayerEntity player)
    {
        this.player = player;
        player.RegisterObserver(this);
        currentHp = targetHp = player.GetEntityStat(EntityStatName.HP);
        UpdateBarsInstantly();
    }
    public void UpdateObserver()
    {
        targetHp = player.GetEntityStat(EntityStatName.HP);
        currentHp = targetHp;
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
    }
}
