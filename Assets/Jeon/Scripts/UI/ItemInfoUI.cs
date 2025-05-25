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
    [LabelText("아이템 이미지"), Required]
    [SerializeField] private Image itemImage;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("사용 버튼"), Required]
    [SerializeField] private Button useButton;

        [TitleGroup("정보창 UI 요소")]
    [LabelText("버리기 버튼"), Required]
    [SerializeField] private Button discardButton;

    [TitleGroup("정보창 UI 요소")]
    [LabelText("닫기 버튼"), Required]
    [SerializeField] private Button closeButton;

    [TitleGroup("강화 시스템 UI")]
    [LabelText("강화 패널")]
    [SerializeField] private GameObject reinforcementPanel;

    [TitleGroup("강화 시스템 UI")]
    [LabelText("강화 버튼")]
    [SerializeField] private Button reinforceButton;

    [TitleGroup("강화 시스템 UI")]
    [LabelText("강화 재료 정보 텍스트")]
    [SerializeField] private TextMeshProUGUI reinforcementMaterialText;

    [TitleGroup("강화 시스템 UI")]
    [LabelText("재료 슬롯 컨테이너")]
    [SerializeField] private Transform materialSlotContainer;

    [TitleGroup("재료 슬롯 프리팹")]
    [LabelText("재료 슬롯 프리팹"), Required]
    [SerializeField] private MaterialSlot materialSlotPrefab;

    [TitleGroup("필드 타일 정보")]
    [LabelText("필드 타일 정보 패널")]
    [SerializeField] private GameObject fieldTileInfoPanel;

    [TitleGroup("필드 타일 정보")]
    [LabelText("필드 타일 슬롯 컨테이너")]
    [SerializeField] private Transform fieldTileSlotContainer;

    [TitleGroup("필드 타일 정보")]
    [LabelText("필드 타일 슬롯 프리팹"), Required]
    [SerializeField] private FieldTileSlot fieldTileSlotPrefab;


    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector] private Item currentItem;

private System.Action<Item> onUseItem;
    private System.Action<Item> onDiscardItem;


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

        // 강화 버튼에 이벤트 연결
        if (reinforceButton != null)
        {
            reinforceButton.onClick.AddListener(ReinforceItem);
        }

        // 버리기 버튼에 이벤트 연결
        if (discardButton != null)
        {
            discardButton.onClick.AddListener(DiscardItem);
        }
        // 초기 상태는 비활성화
        gameObject.SetActive(false);

        // 강화 패널 초기 비활성화
        if (reinforcementPanel != null)
        {
            reinforcementPanel.SetActive(false);
        }
    }

    public void Initialize()
    {
        Hide();
    }

    public void Show(Item item, System.Action<Item> useCallback = null, System.Action<Item> discardCallback = null)
    {
        if (item == null)
        {
            Debug.LogWarning("ItemInfoUI: 표시할 아이템이 없습니다.");
            return;
        }

        currentItem = item;
        onUseItem = useCallback;
        onDiscardItem = discardCallback;

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

        if (reinforcementPanel != null)
        {
            reinforcementPanel.SetActive(false);
        }
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


        // 이미지 설정
        if (itemImage != null)
            itemImage.sprite = currentItem.ItemIcon;

        // 사용 버튼 활성화 여부 설정
        if (useButton != null)
        {
            bool canUse = currentItem is ConsumableItem || currentItem is EquipableItem;
            useButton.interactable = canUse;
        }

        // 강화 시스템 UI 업데이트 (장비 아이템인 경우만)
        UpdateReinforcementUI();
        UpdateFieldTileInfoForConsumableItem();
    }
    private void UpdateReinforcementUI()
    {
        if (reinforcementPanel == null) return;

        // 장비 아이템인 경우에만 강화 패널 표시
        if (currentItem is EquipableItem equipableItem)
        {
            reinforcementPanel.SetActive(true);

            // 강화 재료 정보 표시
            if (reinforcementMaterialText != null)
            {
                reinforcementMaterialText.text = GetReinforcementMaterialInfo(equipableItem);
            }
            UpdateReinforcementDetails(equipableItem);
            UpdateReinforcementMaterialUI(equipableItem);
            // 강화 버튼 활성화 설정
            if (reinforceButton != null)
            {
                bool canReinforce = equipableItem.CanReinforce();
                reinforceButton.interactable = canReinforce;
            }


        }
        else
        {
            reinforcementPanel.SetActive(false);
        }
    }
    private void UpdateFieldTileInfoForConsumableItem()
    {
        // 필드 타일 정보 패널 초기화
        if (fieldTileInfoPanel != null)
        {
            fieldTileInfoPanel.SetActive(false);
        }

        // 소비 아이템인 경우에만 필드 타일 정보 표시
        if (currentItem is ConsumableItem consumableItem)
        {
            // 필드 타일 정보 패널 활성화
            if (fieldTileInfoPanel != null)
            {
                fieldTileInfoPanel.SetActive(true);
            }

            // 기존 슬롯 제거
            if (fieldTileSlotContainer != null)
            {
                foreach (Transform child in fieldTileSlotContainer)
                {
                    Destroy(child.gameObject);
                }
            }
            var itemSO = DataCenter.Instance.GetConsumableItemSO(currentItem.ID);
            var fieldTiles = itemSO.acquirableFieldIds;
            // 이 아이템을 얻을 수 있는 필드 ID들 가져오기

            if (fieldTiles != null)
            {
                foreach (int fieldId in fieldTiles)
                {
                    var fieldTileSO = DataCenter.Instance.GetFieldTileDataSO(fieldId);
                    if (fieldTileSO != null)
                    {
                        // 필드 타일 슬롯 생성
                        GameObject slotObj = Instantiate(fieldTileSlotPrefab.gameObject, fieldTileSlotContainer);
                        var fieldTileSlot = slotObj.GetComponent<FieldTileSlot>();

                        if (fieldTileSlot != null)
                        {
                            fieldTileSlot.Initialize(fieldTileSO);
                        }
                    }
                }
            }
        }
    }

    private void UpdateReinforcementDetails(EquipableItem equipableItem)
    {
        bool canReinforce = equipableItem.CanReinforce();

        // 강화 패널 활성화 (장비 아이템인 경우만)
        if (reinforcementPanel != null)
        {
            reinforcementPanel.SetActive(canReinforce);
        }

        // 강화 재료 정보 표시
        if (reinforcementMaterialText != null)
        {
            if (canReinforce)
            {
                // 장비 아이템에 강화 레시피가 없으므로, 기본 메시지 표시
                reinforcementMaterialText.text = "현재 강화 레시피가 없습니다.";
            }
            else
            {
                reinforcementMaterialText.text = "더 이상 강화할 수 없습니다.";
            }
        }

        // 강화 버튼 비활성화
        if (reinforceButton != null)
        {
            reinforceButton.interactable = false;
        }
    }

    private string GetReinforcementMaterialInfo(EquipableItem equipableItem)
    {
        // 강화 레벨 정보
        string materialInfo = $"강화 레벨: +{equipableItem.currentReinforcementLevel}/{equipableItem.maxReinforcementLevel}\n\n";

        // 현재는 강화 레시피가 없으므로 기본 메시지
        materialInfo += "현재 강화 레시피가 없습니다.\n";
        materialInfo += "강화에 필요한 재료와 비용이 구현되지 않았습니다.\n";

        return materialInfo;
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
        private void DiscardItem()
    {
        if (currentItem == null) return;

        // 버리기 콜백 호출
        onDiscardItem?.Invoke(currentItem);

        // UI 닫기
        Hide();
    }
    private void ReinforceItem()
    {
        if (currentItem == null || !(currentItem is EquipableItem equipableItem))
        {
            Debug.LogWarning("강화할 수 없는 아이템입니다.");
            return;
        }

        var player = Utils.GetPlayer();
        if (player == null)
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다.");
            return;
        }

        bool reinforceResult = equipableItem.Reinforce(player);

        if (reinforceResult)
        {
            Debug.Log($"{equipableItem.ItemName} 강화 성공!");

            // UI 업데이트
            UpdateUI();

            // 인벤토리 UI 업데이트
            var inventoryUI = Utils.GetUI<InventoryUI>();
            if (inventoryUI != null)
            {
                inventoryUI.UpdateSlots();
                inventoryUI.UpdateEquipmentSlots();
            }

            // InfoUI 닫기
            Hide();
        }
        else
        {
            Debug.Log($"{equipableItem.ItemName} 강화 실패!");
        }
    }


    private void UpdateReinforcementMaterialUI(EquipableItem item)
    {
        // 기존 강화 패널 초기화
        if (materialSlotContainer != null)
        {
            // 기존 슬롯 모두 제거
            foreach (Transform child in materialSlotContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // 강화 레시피 가져오기
        var recipe = DataCenter.Instance.GetReinforcementRecipeSO(item.reinforcementRecipeId);
        if (recipe == null)
        {
            Debug.LogWarning($"{item.ItemName}의 강화 레시피를 찾을 수 없습니다.");
            return;
        }

        // 각 재료에 대해 슬롯 생성
        for (int i = 0; i < recipe.requiredItemIDs.Count; i++)
        {
            int requiredItemId = recipe.requiredItemIDs[i];
            int requiredCount = recipe.requiredItemCounts[i];

            // 현재 보유 개수 계산
            int currentCount = BattleFlowController.Instance.playerData.GetItemCount(requiredItemId);

            // 재료 슬롯 생성
            GameObject slotObj = Instantiate(materialSlotPrefab.gameObject, materialSlotContainer);

            // MaterialSlot 컴포넌트 가져오기
            var materialSlot = slotObj.GetComponent<MaterialSlot>();
            if (materialSlot != null)
            {
                materialSlot.Initialize(requiredItemId, requiredCount, currentCount);
            }
        }

    }
}