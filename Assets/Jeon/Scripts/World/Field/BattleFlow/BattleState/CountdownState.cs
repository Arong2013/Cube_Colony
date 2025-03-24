using UnityEngine;

public class CountdownState : IBattleState
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
        //UIManager.Instance.ShowCountdownUI(timer);
    }
    public void Exit()
    {
      ///  UIManager.Instance.HideCountdownUI();
    }
    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
          //  context.ChangeState(new InBattleState(context));
        }
    }
}
