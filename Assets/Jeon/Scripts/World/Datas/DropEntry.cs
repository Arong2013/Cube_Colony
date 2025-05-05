using System.Collections.Generic;

[System.Serializable]
public class DropEntry 
{
    public int MinDropItem;
    public int MaxDropItem;

    public Dictionary<float, int> DropChances;
}
