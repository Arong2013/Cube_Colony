using UnityEngine;

/// <summary>
/// 전투 상태에서 웨이브 진행을 관리하는 상태 클래스.
/// </summary>
public class InBattleState : IBattleState
{
    private readonly BattleFlowController context;
    private readonly WaveController waveController;

    public InBattleState(BattleFlowController context, WaveController waveController)
    {
        this.context = context;
        this.waveController = waveController;
    }

    public void Enter()
    {
        StartWave();
    }

    public void Exit()
    {
        // 필요 시 정리 작업
    }

    public void Update()
    {
        waveController.Tick(Time.deltaTime);
    }

    private void StartWave()
    {
        waveController.StartWave(context.CurrentWaveIndex);
    }
}
