using System.Collections.Generic;
using UnityEngine;

public class BattleFlowController : MonoBehaviour
{
    [SerializeField] int size;
    [SerializeField] private Cube cube;
    private IGameSequenceState currentState;

    public void ChangeState(IGameSequenceState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    private void Start()
    {
        ChangeState(new CountdownState(this, 100000f)); 
        cube.Init(size);    
    }
    private void Update()
    {
        currentState?.Update();
    }

    public void StartBattle()
    {
        
    }

    private void OnWaveComplete()
    {
    }

    private void GameOver()
    {
        Debug.Log("Game Over!"); 
    }
}
