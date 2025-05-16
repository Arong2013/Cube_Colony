using System.Collections.Generic;
using UnityEngine;

public class CountdownState : IGameSequenceState
{
    private BattleFlowController context;
    private Cube cube;
    private CubeData cubeData;
    private InCountDownStateUI inCountDownStateUI;
    public CountdownState(BattleFlowController context, Cube cube, CubeData cubeData)
    {
        this.context = context;
        this.cube = cube;   
        this.cubeData = cubeData;
        inCountDownStateUI = Utils.GetUI<InCountDownStateUI>(); 
    }
    public void Enter()
    {
        cube.Init(cubeData);
        cube.gameObject.SetActive(true);
        inCountDownStateUI.Initialize(SetSurvialState, cube.RotateCube,CanRotate); 
    }
    public void Exit()
    {
        cube.RemoveCube();
        inCountDownStateUI.Disable();   
    }
    public void RotateCubeAction(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        cube.RotateCube(selectedCubie, axis, isClock);
        if(CanRotate())
        {
            context.playerstat.UpdateStat(EntityStatName.Eng, this,-10);
        }
    } 
    public void SetSurvialState()
    {
        context.ChangeState(new InSurvivalState(context, cubeData, cube.GetTopCubieFace()));
    }
    public bool CanRotate() => context.playerstat.GetStat(EntityStatName.Eng) > 0;
    public void Update()
    {
        
    }
}
