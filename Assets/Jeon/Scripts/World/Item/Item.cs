using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ICloneableItem<T>
{
    T Clone();
}
[System.Serializable]
public abstract class Item : ICloneableItem<Item>
{
    [ShowInInspector] public int ID;
    [ShowInInspector] public string ItemName;
    public Sprite ItemIcon => Resources.Load<Sprite>($"Sprites/Items/{ItemName}"); 
    public abstract void Use(PlayerEntity player);
    public abstract Item Clone();
}