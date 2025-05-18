using System.Collections.Generic;
using UnityEngine;

public class CountdownState : IGameSequenceState
{
    private Cube cube;
    private CubeData cubeData;
    private InCountDownStateUI inCountDownStateUI;
    private List<Vector3> usedFacePositions;

    // 큐브 회전 시 소모될 에너지 양
    private float rotationEnergyCost = 10f;

    public CountdownState(Cube cube, CubeData cubeData, List<Vector3> usedFacePositions = null)
    {
        this.cube = cube;
        this.cubeData = cubeData;
        this.usedFacePositions = usedFacePositions ?? new List<Vector3>();
        inCountDownStateUI = Utils.GetUI<InCountDownStateUI>();
    }

    public void Enter()
    {
        // 큐브 사용 횟수에 따른 처리
        int usageCount = BattleFlowController.Instance.GetCubeUsageCount();

        if (usageCount == 0)
        {
            // 첫 번째 사용: 새로운 큐브 생성
            cube.Init(cubeData);
            Debug.Log("새로운 큐브 생성됨");
        }
        else
        {
            // 두 번째, 세 번째 사용: 기존 큐브 유지하고 사용된 페이스를 몬스터 타일로 변경
            cube.UpdateUsedFaces(usedFacePositions);
            Debug.Log($"기존 큐브 유지, 사용된 페이스 {usedFacePositions.Count}개를 몬스터 타일로 변경");
        }

        cube.gameObject.SetActive(true);

        // UI 초기화
        inCountDownStateUI.Initialize(SetSurvivalState, RotateCubeAction, CanRotate);

        // 진행률 표시 (현재 사용 횟수에 따라)
        float progressRatio = (float)usageCount / BattleFlowController.Instance.GetMaxCubeUsage();
        inCountDownStateUI.UpdateExplorationProgress(progressRatio);
    }

    public void Exit()
    {
        // 큐브는 제거하지 않고 비활성화만
        cube.gameObject.SetActive(false);
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
        BattleFlowController.Instance.SetInSurvivalState();
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