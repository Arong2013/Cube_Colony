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
        ShowResultUI();

    }

    public void Exit()
    {

    }

    public void Update()
    {

    }

    private void ShowResultUI()
    {

    }
}
