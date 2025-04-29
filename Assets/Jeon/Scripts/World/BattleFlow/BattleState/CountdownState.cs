using System.Collections.Generic;
using UnityEngine;

public class CountdownState : IGameSequenceState
{
    private BattleFlowController context;
    private int rotateCount;
    private int cunrotateCount;
    Cube cube;
    CubeData cubeData;  
    public CountdownState(BattleFlowController context, Cube cube, CubeData cubeData)
    {
        this.context = context;
        this.cube = cube;   
        this.cubeData = cubeData;
        rotateCount = cubeData.requiredMatches;
    }
    public void Enter()
    {
        cube.Init(cubeData, CheckCount);
        cube.gameObject.SetActive(true);
    }
    public void Exit()
    {
        cube.RemoveCube();
    }
    public void CheckCount()
    {
        cunrotateCount++;
        if (cunrotateCount >= rotateCount)
        {
            context.ChangeState(new InSurvivalState(context, cubeData, cube.GetTopCubieFace()));
        }
    }
    public void Update()
    {
        
    }
}
