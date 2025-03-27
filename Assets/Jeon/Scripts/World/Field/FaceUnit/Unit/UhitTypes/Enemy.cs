using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : FaceUnit
{
    [SerializeField] private List<BehaviorSequenceSO> behaviorSequencesSO;
    [SerializeField] private float moveSpeed;
    private List<BehaviorSequence> behaviorSequences = new List<BehaviorSequence>();
    public override void Init(CubieFace cubieFace)
    {
        base.Init(cubieFace);
        behaviorSequencesSO.ForEach(sequence => behaviorSequences.Add(sequence.CreateBehaviorSequence(this)));
    }
    public void Update()
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
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
