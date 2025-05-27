using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class StorageUI : SerializedMonoBehaviour, IObserver
{
    [TitleGroup("창고 UI 요소")]
    [LabelText("인벤토리 슬롯 컨테이너"), Required]
    [SerializeField] private Transform inventorySlotContainer;

    [TitleGroup("창고 UI 요소")]
    [LabelText("창고 슬롯 컨테이너"), Required]
    [SerializeField] private Transform storageSlotContainer;

    [TitleGroup("창고 UI 요소")]
    [LabelText("닫기 버튼"), Required]
    [SerializeField] private Button closeButton;
    
    [TitleGroup("창고 정보")]
    [LabelText("창고 슬롯 최대 개수")]
    [SerializeField] private int maxStorageSlots = 100;


    [TitleGroup("창고 슬롯 프리펩펩")]
    [LabelText("창고 슬롯롯")]
    [SerializeField] private StorageSlot storageSlot;

    // [TitleGroup("창고 정보")]
    // [LabelText("인벤토리 아이템 개수 텍스트")]
    // [SerializeField] private TextMeshProUGUI inventoryCountText;

    // [TitleGroup("창고 정보")]
    // [LabelText("창고 아이템 개수 텍스트")]
    // [SerializeField] private TextMeshProUGUI storageCountText;

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private List<StorageSlot> inventorySlots = new List<StorageSlot>();

    [TitleGroup("디버그 정보")]
    [ReadOnly, ShowInInspector]
    private List<StorageSlot> storageSlots = new List<StorageSlot>();



    private void Awake()
    {
        // 닫기 버튼 이벤트 설정
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        // 초기 상태는 비활성화
        gameObject.SetActive(false);
    }

    private void Start()
    {
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.RegisterObserver(this);
        }
        
    }

    private void OnDestroy()
    {
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.UnregisterObserver(this);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateStorageUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        
    }

    public void UpdateObserver()
    {
        if (gameObject.activeInHierarchy)
        {
            UpdateStorageUI();
        }
    }

    public void UpdateStorageUI()
    {
        // 기존 슬롯 초기화
        ClearSlots();
        
        if (BattleFlowController.Instance == null)
            return;

        // 인벤토리 아이템 슬롯 생성
        CreateInventorySlots();
        
        // 창고 아이템 슬롯 생성
        CreateStorageSlots();
        
        // 개수 텍스트 업데이트
        UpdateCountTexts();
    }

    private void ClearSlots()
    {
        // 인벤토리 슬롯 제거
        foreach (Transform child in inventorySlotContainer)
        {
            Destroy(child.gameObject);
        }
        inventorySlots.Clear();
        
        // 창고 슬롯 제거
        foreach (Transform child in storageSlotContainer)
        {
            Destroy(child.gameObject);
        }
        storageSlots.Clear();
    }

    private void CreateInventorySlots()
    {
        var inventory = BattleFlowController.Instance.playerData.inventory;
        
        foreach (var item in inventory)
        {
            // 장비 아이템은 표시하지 않음
            if (item is EquipableItem)
                continue;
                
            // 소모품 아이템만 슬롯 생성
            GameObject slotObj = Instantiate(storageSlot.gameObject, inventorySlotContainer);
           var slot =  slotObj.GetComponent<StorageSlot>();
            slot.Initialize(item, false, TransferToStorage);
            inventorySlots.Add(slot);
        }
    }

    private void CreateStorageSlots()
    {
        var storage = BattleFlowController.Instance.storageItems;
        
        foreach (var item in storage)
        {
            GameObject slotObj = Instantiate(storageSlot.gameObject, storageSlotContainer);
           var slot =  slotObj.GetComponent<StorageSlot>();
            slot.Initialize(item, true, TransferToInventory);
            storageSlots.Add(slot);
        }
    }

    private void UpdateCountTexts()
    {
        // if (inventoryCountText != null)
        // {
        //     int inventoryCount = inventorySlots.Count;
        //     int maxInventorySlots = BattleFlowController.Instance.playerData.MaxInventorySlots;
        //     inventoryCountText.text = $"{inventoryCount}/{maxInventorySlots}";
        // }
        
        // if (storageCountText != null)
        // {
        //     int storageCount = storageSlots.Count;
        //     storageCountText.text = $"{storageCount}/{maxStorageSlots}";
        // }
    }

    // 인벤토리에서 창고로 아이템 이동
    public void TransferToStorage(Item item)
    {
        if (BattleFlowController.Instance == null)
            return;
            
        // 창고가 가득 찼는지 확인
        if (BattleFlowController.Instance.storageItems.Count >= maxStorageSlots)
        {
            Debug.LogWarning("창고가 가득 찼습니다.");
            return;
        }
        
        // 인벤토리에서 아이템 제거
        BattleFlowController.Instance.playerData.RemoveItem(item);
        
        // 창고에 아이템 추가
        BattleFlowController.Instance.storageItems.Add(item);
        
        // UI 업데이트
        UpdateStorageUI();
        
        // 인벤토리 UI 업데이트
        Utils.GetUI<InventoryUI>()?.UpdateSlots();
    }

    // 창고에서 인벤토리로 아이템 이동
    public void TransferToInventory(Item item)
    {
        if (BattleFlowController.Instance == null)
            return;
            
        // 인벤토리가 가득 찼는지 확인
        if (BattleFlowController.Instance.playerData.UsedInventorySlots >= BattleFlowController.Instance.playerData.MaxInventorySlots)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다.");
            return;
        }
        
        // 창고에서 아이템 제거
        BattleFlowController.Instance.storageItems.Remove(item);
        
        // 인벤토리에 아이템 추가
        BattleFlowController.Instance.playerData.AddItem(item);
        
        // UI 업데이트
        UpdateStorageUI();
        
        // 인벤토리 UI 업데이트
        Utils.GetUI<InventoryUI>()?.UpdateSlots();
    }
}