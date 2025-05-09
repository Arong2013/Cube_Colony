public class EquipableItem : Item
{
    public override Item Clone()
    {
         var item = new EquipableItem();
        item.ID = this.ID;
        item.ItemName = this.ItemName;
        return item;
    }

    public override void Use(PlayerEntity player)
    {
        
    }
}
