using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChessTargetAcionSO", menuName = "Behavior/Actions/ChessTarget")]
public class ChessTargetAcionSO : BehaviorActionSO
{
    public override BehaviorAction CreateAction()
    {
        return new ChessTargetAcion();
    }
}
public class ChessTargetAcion : BehaviorAction, IBehaviorDatable
{
    private List<CubieFace> astarList;
    bool IsAstarListEmpty => astarList == null || astarList.Count == 0;
    bool HasMoreTargets => astarList.Count > 0;
    public ChessTargetAcion() { }
    public override BehaviorState Execute()
    {
        astarList = FaceUnit.GetUnitData<List<CubieFace>, ChessTargetAcion>();
        if (IsAstarListEmpty)
        {
            astarList = FaceUnit.GetAstarList();
            FaceUnit.SetData<List<CubieFace>, ChessTargetAcion>(astarList);
            if (IsAstarListEmpty)
            {
                FaceUnit.SetMoveDirection(Vector3.zero); // 실패 시 이동 방향 초기화
                return BehaviorState.FAILURE;
            }
        }
        var result = MoveToNextTarget();
        if (result == BehaviorState.RUNNING)
        {
            FaceUnit.SetMoveDirection(UnitMovementHelper.GetMovementDirection(FaceUnit.ParentFace, astarList[0]));
        }
        else
        {
            FaceUnit.SetMoveDirection(Vector3.zero); 
        }

        return result;
    }


    private BehaviorState MoveToNextTarget()
    {
        if (HasReachedDestination(astarList[0]))
        {
            return HandleReachedDestination();
        }
        return BehaviorState.RUNNING;
    }

    private bool HasReachedDestination(CubieFace target) => FaceUnit.ParentFace == target;

    private BehaviorState HandleReachedDestination()
    {
        astarList.RemoveAt(0);
        FaceUnit.SetData<List<CubieFace>, ChessTargetAcion>(astarList);

        if (HasMoreTargets)
        {
            return BehaviorState.RUNNING;
        }
        FaceUnit.SetData<List<FaceUnit>, DetectEnemyCondition>(null);
        return BehaviorState.SUCCESS;
    }
}
