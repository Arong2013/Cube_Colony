using UnityEngine;
using System;
public class InBattleState : IBattleState
{
    private WaveController waveController;
    private BattleFlowController context;
    private Action OnWaveComplete;  
    public InBattleState(BattleFlowController context, WaveController waveController,Action onWaveComplete)
    {
        this.context = context;
        this.waveController = waveController;
        OnWaveComplete = onWaveComplete;    
    }

    public void Enter()
    {
        // 이미 StartNextWave()는 BattleFlow에서 호출했으므로 생략 가능
    }

    public void Update()
    {
        waveController.Tick(Time.deltaTime);
        if (waveController.IsComplete)
        {
            OnWaveComplete?.Invoke();   
        }
    }
    public void Exit() { }
}
