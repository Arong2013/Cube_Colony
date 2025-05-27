using Sirenix.OdinInspector;

public class BaseCampStorageOBJ : SerializedMonoBehaviour
{
    public void OnMouseDown()
    {
        Utils.GetUI<StorageUI>().Show();
    }
}