using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    private float arrivalThreshold = 0.3f; // 목표 지점에 도착했다고 판단할 거리 임계값
    public ChessTargetAcion() { }
    public override BehaviorState Execute()
    {
        if (IsAstarListEmpty)
        {
            astarList = FaceUnit.GetAstarList();
            if (IsAstarListEmpty)
            {
                return BehaviorState.FAILURE;
            }
        }
        var result = MoveToNextTarget();
        if (result == BehaviorState.RUNNING)
        {
            var dir = UnitMovementHelper.GetNextMoveDirection(FaceUnit.transform.position, astarList[0].transform.position);
            Debug.Log(astarList[0].transform.position + "       " + dir);
            FaceUnit.Move(dir);
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
    private bool HasReachedDestination(CubieFace target)
    {
        return Vector3.Distance(FaceUnit.transform.position, target.transform.position) <= arrivalThreshold;
    }
    private BehaviorState HandleReachedDestination()
    {
        astarList.RemoveAt(0);
        if (HasMoreTargets)
        {
            return BehaviorState.RUNNING;
        }
        return BehaviorState.SUCCESS;
    }
}
