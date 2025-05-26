using UnityEngine;

public class CompleteState : IGameSequenceState
{
    private readonly BattleFlowController context;
    private bool isGameOver;

    public CompleteState(BattleFlowController context, bool gameOver)
    {
        this.context = context;
        this.isGameOver = gameOver;
    }

    public void Enter()
    {
        Debug.Log("[Battle] Stage completed! 🎉");

        // 베이스캠프 UI 활성화
        ShowBaseCampUI();

        // 게임 오버 상태에 따라 다른 리셋 로직 적용
        if (isGameOver)
        {
            FullResetPlayerData();
        }
    }
    private void ShowBaseCampUI()
    {
        Debug.Log("베이스캠프 UI 표시");
        // 실제 베이스캠프 UI 활성화 코드 추가
    }

    private void FullResetPlayerData()
    {
        var playerData = BattleFlowController.Instance?.playerData;
        if (playerData != null)
        {
            // 모든 데이터 완전 초기화 (인벤토리, 장비 포함)
            playerData.FullReset();
            Debug.Log("게임 오버로 인한 완전 초기화");
        }
    }

    public void Exit() { }
    public void Update() { }
}