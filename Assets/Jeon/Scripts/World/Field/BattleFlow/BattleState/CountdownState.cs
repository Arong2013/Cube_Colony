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
        Utils.GetUI<CountdownUI>().OnEnableUI(context.StartBattle); 
        //UIManager.Instance.ShowCountdownUI(timer);
    }
    public void Exit()
    {
        Utils.GetUI<CountdownUI>().OnDisableUI();

        ///  UIManager.Instance.HideCountdownUI();
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
