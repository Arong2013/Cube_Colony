using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowController : SerializedMonoBehaviour
{
    [Header("스테이지 큐브 설정")]
    [SerializeField] private Cube cube;
    private int currentStage = 1;

    private IGameSequenceState currentState;


    [Title("큐브 데이터")]
    [SerializeField]
    private Dictionary<int, CubeData> stageCubeDataMap;

    [Header("스테이지 필드 설정")]
    [SerializeField] private Field field;
    [SerializeField] public float stageTime;




    public int CurrentStage => currentStage;    
    public void ChangeState(IGameSequenceState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    private void Start()
    {
        ChangeState(new CountdownState(this,cube, stageCubeDataMap[currentStage]));
    }
    private void Update()
    {
        currentState?.Update();
    }
    public Field GetField() => field;   
    public void StartBattle()
    {

    }
    public void SetCountDwonState()
    {
        currentStage++;
        ChangeState(new CountdownState(this, cube, stageCubeDataMap[currentStage]));
    }
    public void SetGameOverState()
    {
        currentStage = 1;
        ChangeState(new CountdownState(this, cube, stageCubeDataMap[currentStage]));
    }
}
