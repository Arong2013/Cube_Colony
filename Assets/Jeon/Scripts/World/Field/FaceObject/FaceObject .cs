using System;
using System.Collections.Generic;
using UnityEngine;

public class FaceObject : MonoBehaviour
{
    private CubieFace parentFace;
    private Action onDeath;



    [SerializeField] List<BehaviorSequenceSO> behaviorSequencesSO;
    private List<BehaviorSequence> behaviorSequences = new List<BehaviorSequence>();
    
    public void Init()
    {
        behaviorSequencesSO.ForEach(sequence => behaviorSequences.Add(sequence.CreateBehaviorSequence(this)));
    }


    public void AddOnDeathAction(Action action)
    {
        onDeath += action;
    }
    public void DestroySelf()
    {
        onDeath?.Invoke();  
        Destroy(gameObject);
    }
}