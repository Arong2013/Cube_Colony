using System.Collections.Generic;
using UnityEngine;

public class CountdownState : IGameSequenceState
{
    private Cube cube;
    private CubeData cubeData;
    private InCountDownStateUI inCountDownStateUI;

    // 큐브 회전 시 소모될 에너지 양
    private float rotationEnergyCost = 10f;

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
        inCountDownStateUI.Initialize(SetSurvivalState, RotateCubeAction, CanRotate);

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
        // 에너지가 충분한지 확인
        if (!CanRotate())
        {
            Debug.Log("에너지가 부족하여 큐브를 회전할 수 없습니다.");
            return;
        }

        // 큐브 회전
        cube.RotateCube(selectedCubie, axis, isClock);

        // 에너지 소모
        BattleFlowController.Instance.playerData.UpdateEnergy(-rotationEnergyCost);

        // UI 업데이트를 위해 옵저버에게 알림
        BattleFlowController.Instance.NotifyObservers();

        // 에너지가 모두 소진됐는지 확인
        if (!CanRotate())
        {
            Debug.Log("에너지가 모두 소진되었습니다. 더 이상 큐브를 회전할 수 없습니다.");
            // 에너지가 없을 때 회전 UI 비활성화
            inCountDownStateUI.DisableCubeController();
        }
    }

    public void SetSurvivalState()
    {
        // BattleFlowController 싱글톤 참조
        BattleFlowController.Instance.ChangeState(new InSurvivalState(BattleFlowController.Instance, cubeData, cube.GetTopCubieFace()));
    }

    public bool CanRotate()
    {
        // 에너지가 회전에 필요한 양보다 많은지 확인
        return BattleFlowController.Instance.playerData.energy >= rotationEnergyCost;
    }

    public void Update()
    {
        // 필요한 업데이트 로직 추가
    }
}