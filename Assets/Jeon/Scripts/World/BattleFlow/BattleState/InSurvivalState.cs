using UnityEngine;
using System;
public class InSurvivalState : IGameSequenceState
{
    private BattleFlowController context;
    private Action OnWaveComplete;  
    public InSurvivalState(BattleFlowController context,Action onWaveComplete)
    {
        this.context = context;
        OnWaveComplete = onWaveComplete;    
    }

    public void Enter()
    {
        
    }

    public void Update()
    {
    }
    public void Exit() { }
}
