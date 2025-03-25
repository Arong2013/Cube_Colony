using UnityEngine;

[CreateAssetMenu(fileName = "ChessTargetAcionSO", menuName = "Behavior/Actions/ChessTarget")]
public class ChessTargetAcionSO : BehaviorActionSO
{
    [SerializeField] float stopDistance = 4.0f; 

    public override BehaviorAction CreateAction()
    {
        return new ChessTargetAcion(stopDistance);
    }
}
public class ChessTargetAcion : BehaviorAction
{
    float stopDistance;
    public ChessTargetAcion(float stopDistance)
    {
        this.stopDistance = stopDistance;
    }
    public override BehaviorState Execute()
    {
        return BehaviorState.SUCCESS;
    }
}