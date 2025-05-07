using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
[System.Serializable]
public abstract class Item
{
    [ShowInInspector] public readonly int ID;
    [ShowInInspector] public readonly string ItemName;
    public Sprite ItemIcon => Resources.Load<Sprite>($"Sprites/Items/{ItemName}"); 
    public abstract void Use(PlayerEntity player);
}