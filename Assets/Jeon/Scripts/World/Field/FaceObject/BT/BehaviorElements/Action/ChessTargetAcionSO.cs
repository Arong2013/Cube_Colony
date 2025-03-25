using System.Collections.Generic;
using Unity.VisualScripting;
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
    private List<CubieFace> astarList;  // A* 경로 리스트
    bool IsAstarListEmpty => astarList == null || astarList.Count == 0;
    bool HasMoreTargets => astarList.Count > 0; 
    public ChessTargetAcion()
    { }

    public override BehaviorState Execute()
    {
        astarList = FaceUnit.GetUnitData<List<CubieFace>, ChessTargetAcion>();

        if (IsAstarListEmpty)
        {
            return BehaviorState.FAILURE;
        }
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
        FaceUnit.SetData<List<CubieFace>,ChessTargetAcion>(astarList); 
        if (HasMoreTargets)
        {
            return BehaviorState.RUNNING;  
        }
        FaceUnit.SetData<List<FaceUnit>, DetectEnemyCondition>(null);
        return BehaviorState.SUCCESS; 
    }
}
