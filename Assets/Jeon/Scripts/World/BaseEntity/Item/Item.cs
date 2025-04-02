using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using UnityEngine;
public abstract class Item
{
    public readonly int ID; 
    public readonly string ItemName;
    public List<IitemAction> actions = new List<IitemAction>();
    public virtual void Use(PlayerEntity player)
    {
        foreach (var action in actions)
        {
            action.Execute(player, this);
        }
    }
}
