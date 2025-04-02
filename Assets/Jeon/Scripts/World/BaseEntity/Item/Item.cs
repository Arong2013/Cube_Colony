using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using UnityEngine;
public abstract class Item
{
    public readonly int ID; 
    public readonly string ItemName;

    public abstract void Use(PlayerEntity player);

}
