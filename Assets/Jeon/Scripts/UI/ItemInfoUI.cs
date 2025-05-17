using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class ItemInfoUI : SerializedMonoBehaviour
{
    [TitleGroup("정보창 UI 요소")]
    [LabelText("아이템 이름"), Required]
    [SerializeField] private TextMeshProUGUI nameText;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("아이템 설명"), Required]
    [SerializeField] private TextMeshProUGUI descriptionText;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("획득 타일"), Required]
    [SerializeField] private TextMeshProUGUI acquisitionText;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("아이템 이미지"), Required]
    [SerializeField] private Image itemImage;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("사용 버튼"), Required]
    [SerializeField] private Button useButton;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("닫기 버튼"), Required]
    [SerializeField] private Button closeButton;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector] private Item currentItem;

    private System.Action<Item> onUseItem;

    private void Awake()
    {
        // 닫기 버튼에 이벤트 연결
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        // 사용 버튼에 이벤트 연결
        if (useButton != null)
        {
            useButton.onClick.AddListener(UseItem);
        }

        // 초기 상태는 비활성화
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        Hide();
    }

    public void Show(Item item, System.Action<Item> useCallback = null)
    {
        if (item == null)
        {
            Debug.LogWarning("ItemInfoUI: 표시할 아이템이 없습니다.");
            return;
        }

        currentItem = item;
        onUseItem = useCallback;

        // UI 업데이트
        UpdateUI();

        // 정보창 활성화
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentItem = null;
        onUseItem = null;
    }

    private void UpdateUI()
    {
        if (currentItem == null) return;

        // 이름 설정
        if (nameText != null)
            nameText.text = currentItem.ItemName;

        // 설명 설정
        if (descriptionText != null)
            descriptionText.text = currentItem.Description;

        // 획득 타일 설정
        if (acquisitionText != null)
            acquisitionText.text = $"획득: {currentItem.AcquisitionTile}";

        // 이미지 설정
        if (itemImage != null)
            itemImage.sprite = currentItem.ItemIcon;

        // 사용 버튼 활성화 여부 설정
        if (useButton != null)
            useButton.interactable = currentItem is ConsumableItem;
    }

    private void UseItem()
    {
        if (currentItem == null) return;

        // 사용 콜백 호출
        onUseItem?.Invoke(currentItem);

        // UI 업데이트 (수량 변경 등이 있을 수 있음)
        UpdateUI();

        // 소모품인 경우 수량 체크
        if (currentItem is ConsumableItem consumable && consumable.cunamount <= 0)
        {
            Hide(); // 수량이 0이 되면 정보창 닫기
        }
    }
}