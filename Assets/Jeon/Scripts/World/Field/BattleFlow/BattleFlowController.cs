using System.Collections.Generic;
using UnityEngine;

public class BattleFlowController : MonoBehaviour
{
    [SerializeField] private List<WaveData> waveList;
    [SerializeField] private Cube cube;

    private IBattleState currentState;
    private int currentWaveIndex = 0;

    private int life = 20; // 플레이어의 초기 라이프
    private int currentLife;

    public void ChangeState(IBattleState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void Start()
    {
        currentLife = life; // 게임 시작 시 라이프 초기화
        ChangeState(new CountdownState(this, 100000f)); // 카운트다운 상태로 시작
        cube.Init();    
        cube.SpawnExitGate(DecreaseLife,CubeFaceType.Top); // 출구 게이트 생성
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
        ChangeState(new InBattleState(this, waveController, OnWaveComplete));
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
            ChangeState(new CompleteState(this)); // 모든 웨이브가 끝나면 완료 상태
        }
        else
        {
            ChangeState(new CountdownState(this, 100000f)); // 다음 웨이브 전 카운트다운 상태
        }
    }

    // 라이프 감소 메서드
    public void DecreaseLife()
    {
        currentLife -= 1;
        if (currentLife <= 0)
        {
            GameOver();
        }
    }

    // 라이프가 0이 되었을 때 게임 오버 처리
    private void GameOver()
    {
        //ChangeState(new GameOverState(this)); // 게임 오버 상태로 변경
        Debug.Log("Game Over!"); // 디버깅용 로그
    }

    // 라이프 확인 메서드
    public int GetCurrentLife()
    {
        return currentLife;
    }
}
