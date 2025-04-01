using UnityEngine;

public class CountdownState : IGameSequenceState
{
    private BattleFlowController context;
    private float timer;
    public CountdownState(BattleFlowController context, float countdownTime)
    {
        this.context = context;
        this.timer = countdownTime;
    }
    public void Enter()
    {
    }
    public void Exit()
    {
    }
    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            context.StartBattle();
        }
    }
}
