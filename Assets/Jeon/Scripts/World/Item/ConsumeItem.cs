using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ConsumableItem : Item
{
    [ShowInInspector] public int maxamount;
    [ShowInInspector] public List<int> ids = new List<int>();
    [ShowInInspector] public int cunamount = 1;

    // SO에서 추가로 가져온 정보들
    [ShowInInspector] public string description;
    [ShowInInspector] public ItemGrade grade;

    [ShowInInspector, ReadOnly]
    public List<itemAction> actions => ids
        .Select(id => DataCenter.Instance.CreateItemAction(id))
        .Where(action => action != null)
        .ToList();

    public override void Use(PlayerEntity player)
    {
        foreach (var action in actions)
        {
            action.Execute(player);
        }

        cunamount--;
        if (cunamount <= 0)
        {
            cunamount = 0;

            // 소모품이 모두 소진된 경우 인벤토리에서 제거
            if (BattleFlowController.Instance?.playerData != null)
            {
                BattleFlowController.Instance.playerData.RemoveItem(this);
            }
        }

        player.NotifyObservers();
    }

    public override Item Clone()
    {
        // DataCenter에서 새 인스턴스를 생성해서 반환
        return DataCenter.Instance.CreateConsumableItem(this.ID);
    }
}