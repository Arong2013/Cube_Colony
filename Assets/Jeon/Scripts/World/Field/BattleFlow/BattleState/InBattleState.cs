public class InBattleState : IBattleState
{
    private WaveController waveController;
    private BattleFlowController context;

    public InBattleState(BattleFlowController context)
    {
        this.context = context;
    }

    public void Enter()
    {
        
    }

    public void Exit() { }

    public void Update()
    {
        // 필요 시 전투 중 처리
    }
}
