using System.Collections.Generic;
using UnityEngine;

public class FaceObject : MonoBehaviour
{
    private CubieFace parentFace;
    [SerializeField] List<BehaviorSequenceSO> behaviorSequencesSO;
    private List<BehaviorSequence> behaviorSequences = new List<BehaviorSequence>();
    
    public void Init()
    {
        behaviorSequencesSO.ForEach(sequence => behaviorSequences.Add(sequence.CreateBehaviorSequence(this)));
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}