using UnityEngine;

public class CompleteState : IGameSequenceState
{
    private readonly BattleFlowController context;

    public CompleteState(BattleFlowController context)
    {
        this.context = context;
    }

    public void Enter()
    {
        Debug.Log("[Battle] All waves completed! 🎉");
    
        // 베이스캠프 UI 활성화
        ShowBaseCampUI();

        // 플레이어 데이터 부분 초기화 (아이템, 골드 유지)
        ResetPlayerData();
    }

    private void ShowBaseCampUI()
    {
        // 베이스캠프 UI 활성화 로직
        Debug.Log("베이스캠프 UI 표시");
        
        // 여기에 실제 베이스캠프 UI 활성화 코드 추가
        // 예: GameObject.Find("BaseCampUI")?.SetActive(true);
    }

    private void ResetPlayerData()
    {
        var playerData = BattleFlowController.Instance?.playerData;
        if (playerData != null)
        {
            // 체력, 산소 등만 초기화
            playerData.Reset(); 
            Debug.Log("스테이지 완료로 인한 부분 초기화");
        }
    }

    public void Exit() { }
    public void Update() { }
}