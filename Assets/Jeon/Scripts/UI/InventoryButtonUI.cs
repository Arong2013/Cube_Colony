using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class InventoryButtonUI : MonoBehaviour
{
    [TitleGroup("참조")]
    [LabelText("버튼 컴포넌트"), Required]
    [SerializeField] private Button button;

    private void Awake()
    {
        // 버튼 컴포넌트가 할당되지 않았다면 GetComponent로 찾기
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        // 버튼에 클릭 이벤트 추가
        if (button != null)
        {
            button.onClick.AddListener(OpenInventory);
        }
        else
        {
            Debug.LogError("InventoryButton: Button 컴포넌트를 찾을 수 없습니다.");
        }
    }

    private void OpenInventory()
    {
        // InventoryUI 찾기
        var inventoryUI = Utils.GetUI<InventoryUI>();
        
        if (inventoryUI != null)
        {
            // 인벤토리 UI 열기
            inventoryUI.OpenInventoryUI();
        }
        else
        {
            Debug.LogWarning("InventoryButton: InventoryUI를 찾을 수 없습니다.");
        }
    }

    [Button("인벤토리 열기 테스트"), GUIColor(0.3f, 0.8f, 0.3f)]
    private void TestOpenInventory()
    {
        OpenInventory();
    }
}