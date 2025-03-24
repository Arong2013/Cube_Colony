using System.Collections.Generic;
using UnityEngine;

public class BattleFlowController : MonoBehaviour
{
    [SerializeField] private List<WaveData> waveList;
    [SerializeField] private Cube cube;

    private IBattleState currentState;
    private int currentWaveIndex = 0;

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
    public void StartBattle()
    {
        var wave = GetCurrentWave();
        if (wave == null) return;

        var waveController = new WaveController(wave, cube);
        ChangeState(new InBattleState(this, waveController,OnWaveComplete));
    }

    private WaveData GetCurrentWave()
    {
        if (currentWaveIndex < waveList.Count)
            return waveList[currentWaveIndex];
        return null;
    }

    private void OnWaveComplete()
    {
        currentWaveIndex++;
        if (currentWaveIndex >= waveList.Count)
        {
            ChangeState(new CompleteState(this));
        }
        else
        {
            ChangeState(new CountdownState(this, 5f));
        }
    }
}
