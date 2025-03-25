using UnityEngine;

public class CompleteState : IBattleState
{
    private readonly BattleFlowController context;

    public CompleteState(BattleFlowController context)
    {
        this.context = context;
    }

    public void Enter()
    {
        Debug.Log("[Battle] All waves completed! 🎉");

        // 전투 종료 UI 띄우기
        ShowResultUI();

        // 필요 시 자동 씬 전환 또는 게임 정지 등
        // Time.timeScale = 0f;
    }

    public void Exit()
    {
        // 필요 시 정리
    }

    public void Update()
    {
        // 전투 종료 후 입력 대기 등 처리
    }

    private void ShowResultUI()
    {
        // 예시: UI 매니저를 통한 결과 화면 띄우기
        // UIManager.Instance.ShowVictoryPanel();
    }
}
