using System.Collections.Generic;
using UnityEngine;

public class CountdownState : IGameSequenceState
{
    private Cube cube;
    private CubeData cubeData;
    private InCountDownStateUI inCountDownStateUI;

    public CountdownState(Cube cube, CubeData cubeData)
    {
        this.cube = cube;
        this.cubeData = cubeData;
        inCountDownStateUI = Utils.GetUI<InCountDownStateUI>();
    }

    public void Enter()
    {
        cube.Init(cubeData);
        cube.gameObject.SetActive(true);

        // UI 초기화
        inCountDownStateUI.Initialize(SetSurvivalState, cube.RotateCube, CanRotate);

        // 초기 진행률 설정
        inCountDownStateUI.UpdateExplorationProgress(0f);
    }

    public void Exit()
    {
        cube.RemoveCube();
        inCountDownStateUI.Disable();
    }

    public void RotateCubeAction(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        cube.RotateCube(selectedCubie, axis, isClock);
        if (CanRotate())
        {
            // BattleFlowController 싱글톤 참조
            BattleFlowController.Instance.playerData.UpdateEnergy(-10f);
            BattleFlowController.Instance.NotifyObservers();
        }
    }

    public void SetSurvivalState()
    {
        // BattleFlowController 싱글톤 참조
        BattleFlowController.Instance.ChangeState(new InSurvivalState(BattleFlowController.Instance, cubeData, cube.GetTopCubieFace()));
    }

    public bool CanRotate()
    {
        // BattleFlowController 싱글톤 참조
        return BattleFlowController.Instance.playerData.energy > 0;
    }

    // 진행률 업데이트 메서드 추가
    public void UpdateExplorationProgress(float progress)
    {
        inCountDownStateUI.UpdateExplorationProgress(progress);
    }

    public void Update()
    {
        // 필요한 업데이트 로직 추가
    }
}