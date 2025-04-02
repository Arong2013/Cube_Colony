using System.Collections.Generic;
[System.Serializable]
public class DropEntry
{
    public int MinDropItem;
    public int MaxDropItem;

    // key: 확률(0~1), value: 아이템 ID
    public Dictionary<float, int> DropChances;
}
