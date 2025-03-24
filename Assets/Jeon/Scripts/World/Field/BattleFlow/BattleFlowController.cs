using UnityEngine;

public class BattleFlowController : MonoBehaviour
{
    private IBattleState currentState;

    public void ChangeState(IBattleState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    private void Start()
    {
        ChangeState(new CountdownState(this, 5f));
    }
    private void Update()
    {
        currentState?.Update();
    }
    public void OnWaveComplete()
    {
        ChangeState(new CountdownState(this, 5f)); // 다음 웨이브 카운트다운
    }
}
