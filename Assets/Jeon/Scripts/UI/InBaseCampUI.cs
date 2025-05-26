using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class InBaseCampUI : MonoBehaviour, IObserver
{
    [TitleGroup("플레이어 정보 UI")]
    [LabelText("체력 바"), Required]
    [SerializeField] private BarUI hpBar;

    [TitleGroup("플레이어 정보 UI")]
    [LabelText("산소 바"), Required]
    [SerializeField] private BarUI o2Bar;

    [TitleGroup("플레이어 정보 UI")]
    [LabelText("에너지 바"), Required]
    [SerializeField] private BarUI engBar;

    [TitleGroup("베이스캠프 UI")]
    [LabelText("다음 스테이지 시작 버튼"), Required]
    [SerializeField] private Button startNextStageButton;


    private void Start()
    {
        // 버튼 이벤트 연결
        if (startNextStageButton != null)
        {
            startNextStageButton.onClick.AddListener(StartNextStage);
        }

        // 옵저버 등록
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.RegisterObserver(this);
        }

        // 바 타입 설정
        if (hpBar != null) hpBar.SetBarType(BarUI.BarType.Health);
        if (o2Bar != null) o2Bar.SetBarType(BarUI.BarType.Oxygen);
        if (engBar != null) engBar.SetBarType(BarUI.BarType.Energy);
    }

    private void OnDestroy()
    {
        // 옵저버 해제
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.UnregisterObserver(this);
        }
    }

    private void StartNextStage()
    {
        if (BattleFlowController.Instance != null)
        {
            BattleFlowController.Instance.StartNewGame();
        }
    }

    private void OpenInventory()
    {
        var inventoryUI = Utils.GetUI<InventoryUI>();
        if (inventoryUI != null)
        {
            inventoryUI.OpenInventoryUI();
        }
    }

    public void UpdateObserver()
    {
        // 플레이어 상태 UI 업데이트
        UpdateStatUI();
    }

    private void UpdateStatUI()
    {
        if (hpBar != null) hpBar.UpdateValueFromPlayerData();
        if (o2Bar != null) o2Bar.UpdateValueFromPlayerData();
        if (engBar != null) engBar.UpdateValueFromPlayerData();
    }
}