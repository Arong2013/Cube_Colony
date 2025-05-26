using Sirenix.OdinInspector;

public class BaseCampReinforceOBJ : SerializedMonoBehaviour
{
    public void OnMouseDown()
    {
        Utils.GetUI<InventoryUI>().ToggleInventoryUI();
    }
}