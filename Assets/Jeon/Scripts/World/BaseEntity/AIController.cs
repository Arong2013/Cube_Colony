using System.Collections.Generic;

public class AIController : IEntityController
{
    private List<BehaviorSequence> behaviorSequences = new List<BehaviorSequence>();
    public AIController(List<BehaviorSequenceSO> behaviorSequencesSO,Entity entity)
    {
        behaviorSequencesSO.ForEach(sequence => behaviorSequences.Add(sequence.CreateBehaviorSequence(entity)));

    }
    public void Update(Entity entity)
    {
        Execute(); 
    }
    public BehaviorState Execute()
    {
        foreach (var seq in behaviorSequences)
        {
            var behaviorState = seq.Execute();
            if (behaviorState == BehaviorState.SUCCESS || behaviorState == BehaviorState.RUNNING)
            {
                return behaviorState;
            }
        }
        return BehaviorState.FAILURE;
    }
}